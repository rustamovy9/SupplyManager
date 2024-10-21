using Infrustructure.ExtansionMethods;
using MainApp.Middleware;

string filePath = @"C:\Users\VICTUS\Desktop\SupplyManager\MainApp\appsettings.json";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.Register(filePath);

var app = builder.Build();
app.MapControllers();
app.UseRouting();
app.UseMiddleware<CustomMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.Run();

