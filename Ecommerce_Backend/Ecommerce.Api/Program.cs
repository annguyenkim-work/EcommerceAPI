using Ecommerce.Api.Middlewares;
using Ecommerce.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString("DefaultConnection")!,
    builder.Configuration.GetConnectionString("Redis")!
);
// Add services to the container.
// 1. Cấu hình Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information() // Ghi từ mức Info trở lên
    .WriteTo.Console()          // Hiện ở màn hình đen (Console)
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day) // Lưu vào folder logs, mỗi ngày 1 file
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // Tạo file JSON v1
    app.UseSwaggerUI(options =>
    {
        // Trỏ giao diện vào file JSON mà MapOpenApi vừa tạo ra
        options.SwaggerEndpoint("/openapi/v1.json", "Ecommerce API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
