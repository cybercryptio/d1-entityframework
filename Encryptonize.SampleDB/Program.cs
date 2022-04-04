// Copyright 2020-2022 CYBERCRYPT

using EncryptonizeDBSample.Data;
using EncryptonizeDBSample.Services;
using Encryptonize.Client;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables("ENCDB_");

// Add services to the container.
builder.Services.AddScoped<DocumentService>();

// Configure database connection.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");
if (String.IsNullOrWhiteSpace(connectionString))
{
    throw new Exception("Database connection string not initialized");
}
var encryptonizeUrl = builder.Configuration.GetValue<string>("Encryptonize:Url");
if (String.IsNullOrWhiteSpace(encryptonizeUrl))
{
    throw new Exception("Encryptonize URL not defined");
}
var encryptonizeUsername = builder.Configuration.GetValue<string>("Encryptonize:Username");
if (String.IsNullOrWhiteSpace(encryptonizeUsername))
{
    throw new Exception("Encryptonize username not defined");
}
var encryptonizePassword = builder.Configuration.GetValue<string>("Encryptonize:Password");
if (String.IsNullOrWhiteSpace(encryptonizePassword))
{
    throw new Exception("Encryptonize password not defined");
}
builder.Services.AddSingleton<IEncryptonizeClient>(new EncryptonizeClient(encryptonizeUrl, encryptonizeUsername, encryptonizePassword));
builder.Services.AddDbContext<StorageContext>(options =>
    options.UseSqlServer(connectionString,
    sqlServerOptionsAction: sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
        maxRetryCount: 10,
        maxRetryDelay: TimeSpan.FromSeconds(30),
        errorNumbersToAdd: null);
    })
);
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Initialize database.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<StorageContext>();
    context.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
