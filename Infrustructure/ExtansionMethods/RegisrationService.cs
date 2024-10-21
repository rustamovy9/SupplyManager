using Infrustructure.Services.CategoryService;
using Infrustructure.Services.OrderService;
using Infrustructure.Services.ProductService;
using Infrustructure.Services.SuppliersService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrustructure.ExtansionMethods;

public static class RegisrationService
{
    public static void Register(this IServiceCollection serviceCollection,string filePath)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(filePath)
            .Build();
        serviceCollection.AddSingleton<IConfiguration>(configuration);
        serviceCollection.AddScoped<ICategoryService, CategoryService>();
        serviceCollection.AddScoped<IProductService, ProductService>();
        serviceCollection.AddScoped<IOrderService, OrderService>();
        serviceCollection.AddScoped<ISupplierService, SupplierService>();
    }
}