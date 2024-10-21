using System.Xml.Linq;
using Infrustructure.DTOs;
using Infrustructure.Entities;
using Microsoft.Extensions.Configuration;

namespace Infrustructure.Services.ProductService;

public class ProductService : IProductService
{
    private readonly string? _pathData;

    public ProductService(IConfiguration configuration)
    {

        _pathData = configuration[Values.PathData];
        if (!File.Exists(_pathData) || new FileInfo(_pathData).Length == 0)
        {
            XDocument xDocument = new XDocument();
            xDocument.Declaration = new XDeclaration(Values.VersionXml, Values.Utf, Values.Yes);
            XElement xElement = new XElement(Values.DataSource, new XElement(Values.Products));
            xDocument.Add(xElement);
            xDocument.Save(_pathData);
        }
    }

    public async Task<bool> CreateProductAsync(CreateProductDto product)
    {
        try
        {
            XDocument doc = XDocument.Load(_pathData);

            XElement sourceElement = doc.Element(Values.DataSource);
            if (sourceElement is null)
            {
                return false;
            }

            XElement? productElement = sourceElement.Element(Values.Products);
            if (productElement is null)
            {
                productElement = new XElement(Values.Products);
                sourceElement.Add(productElement);
            }

            int maxId = 0;
            if (doc.Element(Values.DataSource)!.Element(Values.Products)!.HasElements)
            {
                maxId = (int)doc.Element(Values.DataSource)!.Element(Values.Products)!.Elements(Values.Product)
                    .Select(x => x.Element(Values.Id)).LastOrDefault()!;
            }

            bool isName = doc.Element(Values.DataSource)!.Element(Values.Products)!.Elements(Values.Product)
                .Any(x => (string)x.Element(Values.Name)! == product.Name);
            if (isName) return await Task.FromResult(false);

            XElement newProduct = new XElement(Values.Product,
                new XElement(Values.Id, maxId + 1),
                new XElement(Values.Name, product.Name),
                new XElement(Values.Description, product.Description),
                new XElement(Values.Price, product.Price),
                new XElement(Values.Quantity, product.Quantity),
                new XElement(Values.CategoryId, product.CategoryId),
                new XElement(Values.CreatedAt, DateTime.UtcNow),
                new XElement(Values.Version, 1),
                new XElement(Values.IsDeleted, false)
            );
            productElement.Add(newProduct);
            doc.Save(_pathData);
            return await Task.FromResult(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public async Task<bool> UpdateProductAsync(UpdateProductDto products)
    {
        try
        {
            XDocument doc = XDocument.Load(_pathData);
            XElement updatedProduct = doc.Root?.Elements(Values.Products)!.Elements(Values.Product)
                .FirstOrDefault(x => (int)x.Element(Values.Id) == products.Id);
            if (updatedProduct is null)
            {
                return await Task.FromResult(false);
            }

            updatedProduct.SetElementValue(Values.Name, products.Name);
            updatedProduct.SetElementValue(Values.Description, products.Description);
            updatedProduct.SetElementValue(Values.Price, products.Price);
            updatedProduct.SetElementValue(Values.Quantity, products.Quantity);
            updatedProduct.SetElementValue(Values.CategoryId, products.CategoryId);
            updatedProduct.SetElementValue(Values.Version, (long)updatedProduct.Element(Values.Version)! + 1);
            updatedProduct.SetElementValue(Values.UpdatedAt, DateTime.UtcNow);
            doc.Save(_pathData);
            return await Task.FromResult(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        try
        {
            XDocument doc = XDocument.Load(_pathData);
            XElement productToDelete = doc.Root?.Elements(Values.Products)!.Elements(Values.Product)
                .FirstOrDefault(x => (int)x.Element(Values.Id) == id);
            if (productToDelete is null)
            {
                return await Task.FromResult(false);
            }

            productToDelete.SetElementValue(Values.IsDeleted, true);
            productToDelete.SetElementValue(Values.DeletedAt, DateTime.UtcNow);
            doc.Save(_pathData);
            return await Task.FromResult(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public async Task<GetProductDto?> GetProductByIdAsync(int id)
    {
        try
        {
            XDocument doc = XDocument.Load(_pathData);
            XElement product = doc.Root!.Element(Values.Products)!.Elements(Values.Product)
                .Where(x => (bool)(x.Element(Values.IsDeleted)!) == false)
                .FirstOrDefault(x => (int)x.Element(Values.Id)! == id)!;
            return await Task.FromResult(new GetProductDto
            {
                Id = (int)product.Element(Values.Id)!,
                Name = (string)product.Element(Values.Name)!,
                Description = (string)product.Element(Values.Description)!,
                Price = (decimal)product.Element(Values.Price)!,
                Quantity = (int)product.Element(Values.Quantity)!,
                CategoryId = (int)product.Element(Values.CategoryId)!,
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public async Task<IEnumerable<GetProductDto>> GetAllProductsAsync()
    {
        try
        {
            XDocument doc = XDocument.Load(_pathData);
            IEnumerable<GetProductDto> products = doc.Root?.Elements(Values.Products)?.Elements(Values.Product)
                .Where(x => (bool)x.Element(Values.IsDeleted) is false)
                .Select(x => new GetProductDto
                {
                    Id = (int)x.Element(Values.Id)!,
                    Name = (string)x.Element(Values.Name)!,
                    Description = (string)x.Element(Values.Description)!,
                    Price = (decimal)x.Element(Values.Price)!,
                    Quantity = (int)x.Element(Values.Quantity)!,
                    CategoryId = (int)x.Element(Values.CategoryId)!
                });
            return await Task.FromResult(products ?? new List<GetProductDto>());

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public async Task<IEnumerable<GetProductWithCategoryDto>> GetProductByFilterCategoryAndOrderByPrice(int? categoryId,
        bool sortByAsc = true)
    {
        try
        {
            XDocument doc = XDocument.Load(_pathData);
            IEnumerable<XElement> products = doc.Root!.Element(Values.Products)!.Elements(Values.Product)
                .Where(x => (bool)(x.Element(Values.IsDeleted)!) == false);
            if (categoryId.HasValue)
            {
                products = products.Where(x => (int)x.Element(Values.CategoryId)! == categoryId.Value);
            }

            products = sortByAsc is false
                ? products.OrderByDescending(x => (decimal)x.Element(Values.Price)!)
                : products.OrderBy(x => (decimal)x.Element(Values.Price)!);

            var categories = doc.Root!.Element(Values.Categories)!.Elements(Values.Category)
                .ToDictionary(x => (int)x.Element(Values.Id)!, x => (string)x.Element(Values.Name)!);

            IEnumerable<GetProductWithCategoryDto> result = products.Select(x => new GetProductWithCategoryDto
            {
                Id = (int)x.Element(Values.Id)!,
                Name = (string)x.Element(Values.Name)!,
                Description = (string)x.Element(Values.Description)!,
                Price = (decimal)x.Element(Values.Price)!,
                Quantity = (int)x.Element(Values.Quantity)!,
                CategoryId = (int)x.Element(Values.CategoryId)!,
                CategoryName = categories[(int)x.Element(Values.CategoryId)!]
            });
            return await Task.FromResult(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public async Task<IEnumerable<GetProductDto>> GetProductsByMaxQuantityAsync(int maxQuantity)
    {
        try
        {
            XDocument doc = XDocument.Load(_pathData);

            IEnumerable<XElement> products = doc.Root!.Element(Values.Products)!.Elements(Values.Product)
                .Where(x => (bool)(x.Element(Values.IsDeleted)!) == false)
                .Where(x => (int)x.Element(Values.Quantity)! < maxQuantity);


            var result = products.Select(x => new GetProductDto
            {
                Id = (int)x.Element(Values.Id)!,
                Name = (string)x.Element(Values.Name)!,
                Description = (string)x.Element(Values.Description)!,
                Price = (decimal)x.Element(Values.Price)!,
                Quantity = (int)x.Element(Values.Quantity)!,
                CategoryId = (int)x.Element(Values.CategoryId)!,
            });

            return await Task.FromResult(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public async Task<GetProductDetailsDto?> GetProductDetailsAsync(int productId)
    {
        try
        {
            XDocument doc = XDocument.Load(_pathData);

            XElement? product = doc.Root?.Element(Values.Products)?
                .Elements(Values.Product)
                .FirstOrDefault(x =>
                    (int)x.Element(Values.Id)! == productId && (bool)x.Element(Values.IsDeleted)! == false);


            int categoryId = (int)product.Element(Values.CategoryId)!;
            string? categoryName = doc.Root?.Element(Values.Categories)?
                .Elements(Values.Category)
                .Where(x => (int)x.Element(Values.Id)! == categoryId)
                .Select(x => (string)x.Element(Values.Name)!)
                .FirstOrDefault();

            int supplierId = (int)product.Element(Values.SupplierId)!;
            string? supplierName = doc.Root?.Element(Values.Suppliers)?
                .Elements(Values.Supplier)
                .Where(x => (int)x.Element(Values.Id)! == supplierId)
                .Select(x => (string)x.Element(Values.Name)!)
                .FirstOrDefault();

            return await Task.FromResult(new GetProductDetailsDto
            {
                ProductId = (int)product.Element(Values.Id)!,
                ProductName = (string)product.Element(Values.Name)!,
                ProductDescription = (string)product.Element(Values.Description)!,
                ProductPrice = (decimal)product.Element(Values.Price)!,
                ProductQuantity = (int)product.Element(Values.Quantity)!,
                CategoryName = categoryName!,
                SupplierName = supplierName!
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }


    }
}

file class Values
{
    public const string Suppliers = "suppliers";
    public const string Supplier = "supplier";
    public const string SupplierId = "supplierId";
    public const string Categories = "categories";
    public const string Category = "category";
    public const string PathData = "PathData";
    public const string DataSource = "source";
    public const string Products = "products";
    public const string Product = "product";
    public const string Utf = "utf-8";
    public const string VersionXml = "1.0";
    public const string Yes = "yes";
    public const string Id = "id";
    public const string Name = "name";
    public const string Description = "description";
    public const string CategoryId = "categoryId";
    public const string Price = "price";
    public const string Quantity = "quantity";
    public const string CreatedAt = "createdAt";
    public const string UpdatedAt = "updatedAt";
    public const string DeletedAt = "deletedAt";
    public const string IsDeleted = "isDeleted";
    public const string Version = "version";
}