using System.Windows.Markup;
using System.Xml.Linq;
using Infrustructure.DTOs;
using Infrustructure.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;

namespace Infrustructure.Services.CategoryService;

public class CategoryService:ICategoryService
{
    private readonly string? _pathData;

    public CategoryService(IConfiguration configuration)
    {
        _pathData = configuration[Values.PathData];
    if (!File.Exists(_pathData)|| new FileInfo(_pathData).Length==0)
        {
            XDocument xDocument = new XDocument();
            xDocument.Declaration = new XDeclaration(Values.VersionXml, Values.Utf, Values.Yes);
            XElement xElement = new XElement(Values.DataSource,new XElement(Values.Categories));
            xDocument.Add(xElement);
            xDocument.Save(_pathData);
        }
    }

    public async Task<bool> CreateCategoryAsync(CreateCategoryDto categories)
    {
        try
        {
            XDocument doc =XDocument.Load(_pathData);
            XElement sourceElement = doc.Element(Values.DataSource);
    
            if (sourceElement is null)
            {
                return false;
            }
            
            XElement categoryElement = sourceElement.Element(Values.Categories);
            if (categoryElement is null)
            {
                categoryElement = new XElement(Values.Categories);
                sourceElement.Add(categoryElement);
            }
            
            int maxId = 0;
            if (doc.Element(Values.DataSource)!.Element(Values.Categories)!.HasElements)
            {
                maxId = (int)doc.Element(Values.DataSource)!.Element(Values.Categories)!.Elements(Values.Category)
                    .Select(x => x.Element(Values.Id)).LastOrDefault()!;
            }

            bool isName = doc.Element(Values.DataSource)!.Element(Values.Categories)!.Elements(Values.Category)
                .Any(x => (string)x.Element(Values.Name)! == categories.Name);
            if (isName) return await Task.FromResult(false); 
            
            XElement newCategory = new XElement(Values.Category,
                new XElement(Values.Id,maxId+1),
                new XElement(Values.Name, categories.Name),
                new XElement(Values.Description, categories.Description),
                new XElement(Values.CreatedAt,DateTime.UtcNow),
                new XElement(Values.Version,1),
                new XElement(Values.IsDeleted, false)
            );
            categoryElement.Add(newCategory); 
            doc.Save(_pathData); 
            return await Task.FromResult(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public async Task<bool> UpdateCategoryAsync(UpdateCategoryDto categories)
    {
        try
        {
            XDocument doc = XDocument.Load(_pathData);
            XElement updatedCategory = doc.Root?.Elements(Values.Categories)?.Elements(Values.Category)
                .FirstOrDefault(x => (int)x.Element(Values.Id) == categories.Id);
            if (updatedCategory is null)
            {
                return await Task.FromResult(false);
            }
            updatedCategory.SetElementValue(Values.Name,categories.Name);
            updatedCategory.SetElementValue(Values.Description,categories.Description);
            updatedCategory.SetElementValue(Values.Version,(long)updatedCategory.Element(Values.Version)+1);
            updatedCategory.SetElementValue(Values.UpdatedAt,DateTime.UtcNow);
            doc.Save(_pathData);
            return await Task.FromResult(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        try
        {
            XDocument doc = XDocument.Load(_pathData);
            XElement deleteCategory = doc.Root?.Elements(Values.Categories)?.Elements(Values.Category)
                .FirstOrDefault(x => (int)x.Element(Values.Id) == id);
            if (deleteCategory is null)
            {
                return await Task.FromResult(false);
            }
            deleteCategory.SetElementValue(Values.IsDeleted,true);
            deleteCategory.SetElementValue(Values.DeletedAt,DateTime.UtcNow);
            doc.Save(_pathData);
            return await Task.FromResult(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public async Task<GetCategoryDto?> GetCategoryByIdAsync(int id)
    {
        try
        {
            XDocument xDocument = XDocument.Load(_pathData);
            XElement? category = xDocument.Root!.Element(Values.Categories)!.Elements(Values.Category)
                .Where(x=>(bool)(x.Element(Values.IsDeleted)!)==false)
                .FirstOrDefault(x => (int)x.Element(Values.Id)! == id);
            
            if (category is null)
            { 
                return null;
            }
            return await Task.FromResult(new GetCategoryDto
            {
                    Id = (int)category.Element(Values.Id)!,
                    Name = (string)category.Element(Values.Name)!,
                    Description = (string)category.Element(Values.Description)!
            });

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public async Task<IEnumerable<GetCategoryDto>> GetAllCategoriesAsync()
    {
        try
        {
            XDocument doc = XDocument.Load(_pathData);
            IEnumerable<GetCategoryDto> categories = doc.Root?.Elements(Values.Categories)?.Elements(Values.Category)
                .Where(x => (bool)x.Element(Values.IsDeleted) is false)
                .Select(x => new GetCategoryDto()
                {
                    Id = (int)x.Element(Values.Id)!,
                    Name = (string)x.Element(Values.Name)!,
                    Description = (string)x.Element(Values.Description)!
                });
            return await Task.FromResult(categories ?? new List<GetCategoryDto>());
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }
    
    public async Task<IEnumerable<CategoryWithProductCountDto>> GetCategoriesWithProductCountAsync()
    {
        try
        {
            XDocument doc = XDocument.Load(_pathData);

            var categories = doc.Root?.Element(Values.Categories)?
                .Elements(Values.Category)
                .Select(category => new
                {
                    CategoryId = (int)category.Element(Values.Id)!,
                    CategoryName = (string)category.Element(Values.Name)!,
                    ProductCount = doc.Root.Element(Values.Products)?
                        .Elements(Values.Product)
                        .Count(product => (int)product.Element(Values.CategoryId)! == (int)category.Element(Values.Id)!)
                })
                .Select(x => new CategoryWithProductCountDto(
                    x.CategoryId,
                    x.CategoryName,
                    x.ProductCount ?? 0));

            return await Task.FromResult(categories ?? Enumerable.Empty<CategoryWithProductCountDto>());
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
    public const string Product = "product";
    public const string CategoryId = "categoryId";
    public const string PathData = "PathData";
    public const string DataSource = "source";
    public const string Categories = "categories";
    public const string Products = "products";
    public const string Utf = "utf-8";
    public const string VersionXml = "1.0";
    public const string Yes = "yes";
    public const string Category = "category";
    public const string Id = "id";
    public const string Name = "name";
    public const string Description = "description";
    public const string CreatedAt = "createdAt";
    public const string UpdatedAt = "updatedAt";
    public const string DeletedAt = "deletedAt";
    public const string IsDeleted = "isDeleted";
    public const string Version = "version";
}