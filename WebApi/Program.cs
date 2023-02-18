using Microsoft.EntityFrameworkCore;
using WebSpaceManager.DbAccess;
using WebSpaceManager.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var dbConfig = builder.Configuration.GetSection("DbConfig");

string? connection = Convert.ToBoolean(dbConfig.GetValue(typeof(bool), "UseLocalDb")) 
    ? dbConfig.GetSection("ConnectionStrings").GetValue(typeof(string), "LocalDbConnection").ToString() :
      dbConfig.GetSection("ConnectionStrings").GetValue(typeof(string), "AzureDbConnection").ToString() ;

builder.Services.AddDbContext<SpaceManagerDb>(options => options.UseSqlServer(connection));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IContractsHelper, ContractsHelper>();
builder.Services.AddScoped<IContractsDevHelper, ContractsHelper>();
builder.Services.AddScoped<IAuthoriztionValidator, AuthoriztionValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();
