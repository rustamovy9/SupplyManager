using System.Xml.Linq;
using Infrustructure.DTOs;
using Infrustructure.Enums;
using Microsoft.Extensions.Configuration;

namespace Infrustructure.Services.OrderService;

public class OrderService : IOrderService
{
    private readonly string? _pathData;

    public OrderService(IConfiguration configuration)
    {
        _pathData = configuration[Values.PathData];
        if (!File.Exists(_pathData) || new FileInfo(_pathData).Length == 0)
        {
            XDocument xDocument = new XDocument();
            xDocument.Declaration = new XDeclaration(Values.VersionXml, Values.Utf, Values.Yes);
            XElement xElement = new XElement(Values.DataSource, new XElement(Values.Orders));
            xDocument.Add(xElement);
            xDocument.Save(_pathData);
        }
    }

    public async Task<bool> CreateOrderAsync(CreateOrderDto order)
    {
        try
        {
            XDocument doc = XDocument.Load(_pathData);
            XElement sourceElement = doc.Element(Values.DataSource);

            if (sourceElement is null)
            {
                return false;
            }

            XElement ordersElement = sourceElement.Element(Values.Orders);
            if (ordersElement is null)
            {
                ordersElement = new XElement(Values.Orders);
                sourceElement.Add(ordersElement);
            }

            int maxId = 0;
            if (doc.Element(Values.DataSource)!.Element(Values.Orders)!.HasElements)
            {
                maxId = (int)doc.Element(Values.DataSource)!.Element(Values.Orders)!.Elements(Values.Order)
                    .Select(x => x.Element(Values.Id)).LastOrDefault()!;
            }

            XElement newOrder = new XElement(Values.Order,
                new XElement(Values.Id, maxId + 1),
                new XElement(Values.ProductId, order.ProductId),
                new XElement(Values.Quantity, order.Quantity),
                new XElement(Values.SupplierId, order.SupplierId),
                new XElement(Values.Status, (int)Status.InProgress),
                new XElement(Values.OrderDate, DateTime.UtcNow),
                new XElement(Values.CreatedAt, DateTime.UtcNow),
                new XElement(Values.Version, 1),
                new XElement(Values.IsDeleted, false)
            );
            ordersElement.Add(newOrder);
            doc.Save(_pathData);
            return await Task.FromResult(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public async Task<bool> UpdateOrderAsync(UpdateOrderDto order)
    {
        try
        {
            XDocument doc = XDocument.Load(_pathData);
            XElement updatedOrder = doc.Root?.Elements(Values.Orders)?.Elements(Values.Order)
                .FirstOrDefault(x => (int)x.Element(Values.Id) == order.Id);
            if (updatedOrder is null)
            {
                return await Task.FromResult(false);
            }

            updatedOrder.SetElementValue(Values.ProductId, order.ProductId);
            updatedOrder.SetElementValue(Values.Quantity, order.Quantity);
            updatedOrder.SetElementValue(Values.SupplierId, order.SupplierId);
            updatedOrder.SetElementValue(Values.Version, (long)updatedOrder.Element(Values.Version) + 1);
            updatedOrder.SetElementValue(Values.UpdatedAt, DateTime.UtcNow);
            updatedOrder.SetElementValue(Values.Status, (int)Status.Completed);
            doc.Save(_pathData);
            return await Task.FromResult(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        try
        {
            XDocument doc = XDocument.Load(_pathData);
            XElement deleteOrder = doc.Root?.Elements(Values.Orders)?.Elements(Values.Order)
                .FirstOrDefault(x => (int)x.Element(Values.Id) == id);
            if (deleteOrder is null)
            {
                return await Task.FromResult(false);
            }

            deleteOrder.SetElementValue(Values.IsDeleted, true);
            deleteOrder.SetElementValue(Values.DeletedAt, DateTime.UtcNow);
            deleteOrder.SetElementValue(Values.Status, (int)Status.Cancelled);
            doc.Save(_pathData);
            return await Task.FromResult(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public async Task<GetOrderDto?> GetOrderByIdAsync(int id)
    {
        try
        {
            XDocument xDocument = XDocument.Load(_pathData);
            XElement? order = xDocument.Root!.Element(Values.Orders)!.Elements(Values.Order)
                .Where(x => (bool)(x.Element(Values.IsDeleted)!) == false)
                .FirstOrDefault(x => (int)x.Element(Values.Id)! == id);

            if (order is null)
            {
                return null;
            }

            return await Task.FromResult(new GetOrderDto
            {
                Id = (int)order.Element(Values.Id)!,
                OrderDate = (DateTime)order.Element(Values.OrderDate)!,
                ProductId = (int)order.Element(Values.ProductId)!,
                Quantity = (int)order.Element(Values.Quantity)!,
                Status = (Status)Enum.Parse(typeof(Status), (string)order.Element(Values.Status)!),
                SupplierId = (int)order.Element(Values.SupplierId)!,
            });

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public async Task<IEnumerable<GetOrderDto>> GetAllOrdersAsync()
    {
        try
        {
            XDocument doc = XDocument.Load(_pathData);
            IEnumerable<GetOrderDto> orders = doc.Root?.Elements(Values.Orders)?.Elements(Values.Order)
                .Where(x => (bool)x.Element(Values.IsDeleted) is false)
                .Select(x => new GetOrderDto
                {
                    Id = (int)x.Element(Values.Id)!,
                    OrderDate = (DateTime)x.Element(Values.OrderDate)!,
                    ProductId = (int)x.Element(Values.ProductId)!,
                    Quantity = (int)x.Element(Values.Quantity)!,
                    Status = (Status)Enum.Parse(typeof(Status), (string)x.Element(Values.Status)!),
                    SupplierId = (int)x.Element(Values.SupplierId)!,
                });
            return await Task.FromResult(orders ?? new List<GetOrderDto>());
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public async Task<IEnumerable<GetOrderBySupplierAndStatusDto>> GetOrdersBySupplierAndStatusAsync(int supplierId,
        Status status)
    {
        try
        {
            XDocument doc = XDocument.Load(_pathData);
            string? supplierName = doc.Root?.Element(Values.Suppliers)?
                .Elements(Values.Supplier)
                .Where(x => (int)x.Element(Values.Id)! == supplierId)
                .Select(x => (string)x.Element(Values.Name)!)
                .FirstOrDefault();

            IEnumerable<GetOrderBySupplierAndStatusDto> orders = doc.Root?.Elements(Values.Orders)
                ?.Elements(Values.Order)
                .Where(x => (int)x.Element(Values.SupplierId)! == supplierId)
                .Where(x => (int)x.Element(Values.Status)! == (int)status)
                .Select(x =>
                {
                    var statusInt = (int)x.Element(Values.Status)!;
                    var statusEnum = (Status)Enum.ToObject(typeof(Status), statusInt);

                    return new GetOrderBySupplierAndStatusDto()
                    {
                        Id = (int)x.Element(Values.Id)!,
                        OrderDate = (DateTime)x.Element(Values.OrderDate)!,
                        ProductId = (int)x.Element(Values.ProductId)!,
                        Quantity = (int)x.Element(Values.Quantity)!,
                        Status = statusEnum,
                        SupplierId = supplierId,
                        SupplierName = supplierName
                    };
                });

            return await Task.FromResult(orders ?? new List<GetOrderBySupplierAndStatusDto>());


        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public async Task<IEnumerable<GetOrderDto>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            XDocument doc = XDocument.Load(_pathData);

            IEnumerable<GetOrderDto>? orders = doc.Root?.Element(Values.Orders)?
                .Elements(Values.Order)
                .Where(x =>
                    (DateTime)x.Element(Values.OrderDate)! >= startDate &&
                    (DateTime)x.Element(Values.OrderDate)! <= endDate)
                .Select(x => new GetOrderDto
                {
                    Id = (int)x.Element(Values.Id)!,
                    OrderDate = (DateTime)x.Element(Values.OrderDate)!,
                    ProductId = (int)x.Element(Values.ProductId)!,
                    Quantity = (int)x.Element(Values.Quantity)!,
                    Status = (Status)(int)x.Element(Values.Status)!,
                    SupplierId = (int)x.Element(Values.SupplierId)!
                });

            return await Task.FromResult(orders ?? new List<GetOrderDto>());
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }
    
    public async Task<IEnumerable<GetOrderDto>> GetOrdersWithPaginationAsync(int pageNumber, int pageSize)
    {
        try
        {
            XDocument doc = XDocument.Load(_pathData);

            var orders = doc.Root?.Element(Values.Orders)?
                .Elements(Values.Order)
                .Where(x => (bool)x.Element(Values.IsDeleted) == false)
                .Select(x => new GetOrderDto
                {
                    Id = (int)x.Element(Values.Id)!,
                    OrderDate = (DateTime)x.Element(Values.OrderDate)!,
                    ProductId = (int)x.Element(Values.ProductId)!,
                    Quantity = (int)x.Element(Values.Quantity)!,
                    Status = (Status)(int)x.Element(Values.Status)!,
                    SupplierId = (int)x.Element(Values.SupplierId)!
                });

            return await Task.FromResult(orders?.Skip((pageNumber - 1) * pageSize).Take(pageSize));
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
    public const string Name = "name";
    public const string PathData = "PathData";
    public const string DataSource = "source";
    public const string Orders = "orders";
    public const string Utf = "utf-8";
    public const string VersionXml = "1.0";
    public const string Yes = "yes";
    public const string Order = "order";
    public const string Id = "id";
    public const string ProductId = "productId";
    public const string SupplierId = "supplierId";
    public const string Status = "status";
    public const string OrderDate = "orderDate";
    public const string Quantity = "quantity";
    public const string CreatedAt = "createdAt";
    public const string UpdatedAt = "updatedAt";
    public const string DeletedAt = "deletedAt";
    public const string IsDeleted = "isDeleted";
    public const string Version = "version";
   
}