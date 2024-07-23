using WebApplicationTestLib.Extensions;
using WebApplicationTestLib.Repositories;
using DatabaseAdapter.Domain.Enums;


var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DB");
// Add services to the container.
builder.Services.AddCategoryRepository(options =>
{
    options.ConnectionString = connectionString;
    options.DatabaseType = DatabaseType.SqlServer;
    options.DataServiceHandlerType = DataServiceHandlerType.AdoNet;
});  
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
