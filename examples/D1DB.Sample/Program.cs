// Copyright 2020-2022 CYBERCRYPT

using D1DB.Sample.Data;
using Microsoft.EntityFrameworkCore;
using CyberCrypt.D1.Client;
using CyberCrypt.D1.Client.Credentials;
using Microsoft.OpenApi.Models;

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

var oidcAuthzEndpoint = builder.Configuration.GetValue<string>("D1Generic:Oidc:AuthorizationEndpoint");
var oidcEnabled = !string.IsNullOrEmpty(oidcAuthzEndpoint);

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
if (oidcEnabled)
{
    builder.Services.AddScoped<Func<ID1Generic>>(x =>
    {
        var accessor = x.GetRequiredService<IHttpContextAccessor>();
        return () =>
        {
            var token = accessor.HttpContext?.Request.Headers["Authorization"].ToString().Split(' ').LastOrDefault();
            return new D1GenericClient(d1Url, new TokenCredentials(token));
        };
    });
}
else
{
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
builder.Services.AddSwaggerGen(c =>
{
    var schemeId = "OIDC";

    c.AddSecurityDefinition(
        schemeId,
        new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                Implicit = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri(oidcAuthzEndpoint, UriKind.Absolute),
                }
            },
        }
    );

    c.AddSecurityRequirement(new OpenApiSecurityRequirement() {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = schemeId,
                }
            },
            new string[] { }
        }
    });
});

var app = builder.Build();

// Initialize database.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<StorageContext>();
    context.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI(

    options =>
    {
        if (oidcEnabled)
        {
            var clientId = builder.Configuration.GetValue<string>("D1Generic:Oidc:ClientId");
            if (!string.IsNullOrEmpty(oidcAuthzEndpoint))
            {
                options.OAuthClientId(clientId);
            }
        }
    }
);

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
