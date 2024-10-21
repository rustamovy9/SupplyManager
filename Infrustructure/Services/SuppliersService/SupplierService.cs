using System.Windows.Markup;
using System.Xml.Linq;
using Infrustructure.DTOs;
using Infrustructure.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;

namespace Infrustructure.Services.SuppliersService;

public class SupplierService:ISupplierService
{
    private readonly string? _pathData;

    public SupplierService(IConfiguration configuration)
    {
        _pathData = configuration[Values.PathData];
    if (!File.Exists(_pathData)|| new FileInfo(_pathData).Length==0)
        {
            XDocument xDocument = new XDocument();
            xDocument.Declaration = new XDeclaration(Values.VersionXml, Values.Utf, Values.Yes);
            XElement xElement = new XElement(Values.DataSource,new XElement(Values.Suppliers));
            xDocument.Add(xElement);
            xDocument.Save(_pathData);
        }
    }

    public async Task<bool> CreateSupplierAsync(CreateSupplierDto supplier)
    {
        try
        {
            XDocument doc =XDocument.Load(_pathData);
            XElement sourceElement = doc.Element(Values.DataSource);
    
            if (sourceElement is null)
            {
                return false;
            }
            
            XElement supplierElement = sourceElement.Element(Values.Suppliers);
            if (supplierElement is null)
            {
                supplierElement = new XElement(Values.Suppliers);
                sourceElement.Add(supplierElement);
            }
            
            int maxId = 0;
            if (doc.Element(Values.DataSource)!.Element(Values.Suppliers)!.HasElements)
            {
                maxId = (int)doc.Element(Values.DataSource)!.Element(Values.Suppliers)!.Elements(Values.Supplier)
                    .Select(x => x.Element(Values.Id)).LastOrDefault()!;
            }

            bool isEmail = doc.Element(Values.DataSource)!.Element(Values.Suppliers)!.Elements(Values.Supplier)
                .Any(x => (string)x.Element(Values.Email)! == supplier.Email);
            if (isEmail) return await Task.FromResult(false); 
            
            XElement newCategory = new XElement(Values.Supplier,
                new XElement(Values.Id,maxId+1),
                new XElement(Values.Name, supplier.Name),
                new XElement(Values.Email, supplier.Email),
                new XElement(Values.ContactPerson, supplier.ContactPerson),
                new XElement(Values.Phone, supplier.Phone),
                new XElement(Values.CreatedAt,DateTime.UtcNow),
                new XElement(Values.Version,1),
                new XElement(Values.IsDeleted, false)
            );
            supplierElement.Add(newCategory); 
            doc.Save(_pathData); 
            return await Task.FromResult(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }
    

    public async Task<bool> UpdateSupplierAsync(UpdateSupplierDto supplier)
    {
        try
        {
            XDocument doc = XDocument.Load(_pathData);
            XElement updatedCategory = doc.Root?.Elements(Values.Suppliers)?.Elements(Values.Supplier)
                .FirstOrDefault(x => (int)x.Element(Values.Id) == supplier.Id);
            if (updatedCategory is null)
            {
                return await Task.FromResult(false);
            }
            updatedCategory.SetElementValue(Values.Name,supplier.Name);
            updatedCategory.SetElementValue(Values.Email,supplier.Email);
            updatedCategory.SetElementValue(Values.Phone,supplier.Phone);
            updatedCategory.SetElementValue(Values.ContactPerson,supplier.ContactPerson);
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

    public async Task<bool> DeleteSupplierAsync(int id)
    {
        try
        {
            XDocument doc = XDocument.Load(_pathData);
            XElement deleteCategory = doc.Root?.Elements(Values.Suppliers)?.Elements(Values.Supplier)
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

    public async Task<GetSupplierDto?> GetSupplierByIdAsync(int id)
    {
        try
        {
            XDocument xDocument = XDocument.Load(_pathData);
            XElement? category = xDocument.Root!.Element(Values.Suppliers)!.Elements(Values.Supplier)
                .Where(x=>(bool)(x.Element(Values.IsDeleted)!)==false)
                .FirstOrDefault(x => (int)x.Element(Values.Id)! == id);
            
            if (category is null)
            { 
                return null;
            }
            return await Task.FromResult(new GetSupplierDto
            {
                    Id = (int)category.Element(Values.Id)!,
                    Name = (string)category.Element(Values.Name)!,
                    Email = (string)category.Element(Values.Email)!,
                    ContactPerson = (string)category.Element(Values.ContactPerson)!,
                    Phone = (string)category.Element(Values.Phone)!,
            });

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public async Task<IEnumerable<GetSupplierDto>> GetAllSuppliersAsync()
    {
        try
        {
            XDocument doc = XDocument.Load(_pathData);
            IEnumerable<GetSupplierDto> suppliers = doc.Root?.Elements(Values.Suppliers)?.Elements(Values.Supplier)
                .Where(x => (bool)x.Element(Values.IsDeleted) is false)
                .Select(x => new GetSupplierDto
                {
                    Id = (int)x.Element(Values.Id)!,
                    Name = (string)x.Element(Values.Name)!,
                    Email = (string)x.Element(Values.Email)!,
                    ContactPerson = (string)x.Element(Values.ContactPerson)!,
                    Phone = (string)x.Element(Values.Phone)!,
                });
            return await Task.FromResult(suppliers ?? new List<GetSupplierDto>());
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
    public const string Quantity = "quantity";
    public const string Product = "product";
    public const string Products = "products";
    public const string PathData = "PathData";
    public const string DataSource = "source";
    public const string Suppliers = "suppliers";
    public const string Utf = "utf-8";
    public const string VersionXml = "1.0";
    public const string Yes = "yes";
    public const string Supplier = "supplier";
    public const string Id = "id";
    public const string Name = "name";
    public const string Email = "email";
    public const string Phone = "phone";
    public const string ContactPerson = "contactPerson";
    public const string CreatedAt = "createdAt";
    public const string UpdatedAt = "updatedAt";
    public const string DeletedAt = "deletedAt";
    public const string IsDeleted = "isDeleted";
    public const string Version = "version";
}