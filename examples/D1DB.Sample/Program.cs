// Copyright 2020-2022 CYBERCRYPT

using D1DB.Sample.Data;
using Microsoft.EntityFrameworkCore;
using CyberCrypt.D1.Client;
using CyberCrypt.D1.Client.Credentials;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables("D1DB_");

// Configure database connection.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");
if (String.IsNullOrWhiteSpace(connectionString))
{
    throw new Exception("Database connection string not initialized");
}
var d1Url = builder.Configuration.GetValue<string>("D1Generic:Url");
if (String.IsNullOrWhiteSpace(d1Url))
{
    throw new Exception("D1 Generic URL not defined");
}
var d1Username = builder.Configuration.GetValue<string>("D1Generic:Username");
if (String.IsNullOrWhiteSpace(d1Username))
{
    throw new Exception("D1 Generic username not defined");
}
var d1Password = builder.Configuration.GetValue<string>("D1Generic:Password");
if (String.IsNullOrWhiteSpace(d1Password))
{
    throw new Exception("D1 Generic password not defined");
}
var oidcEnabled = builder.Configuration.GetValue<bool>("D1Generic:OidcEnabled");

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
if (oidcEnabled)
{
    builder.Services.AddScoped<Func<ID1Generic>>(x =>
    {
        var accessor = x.GetRequiredService<IHttpContextAccessor>();
        return () =>
        {
            var token = accessor.HttpContext?.Request.Headers["Authorization"].ToString().Split(' ').LastOrDefault();
            return new D1GenericClient(d1Url, new TokenCredentials(d1Url));
        };
    });
}
else
{
    builder.Services.AddScoped<Func<ID1Generic>>(x => () => new D1GenericClient(d1Url, new UsernamePasswordCredentials(d1Url, d1Username, d1Password)));
}

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
if (!oidcEnabled)
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<StorageContext>();
        context.Database.EnsureCreated();
    }
}
else
{
    Console.BackgroundColor = ConsoleColor.Red;
    Console.WriteLine("WARNING: Database will not be created, because OIDC is enabled and dependency injection fails this early.");
    Console.ResetColor();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
