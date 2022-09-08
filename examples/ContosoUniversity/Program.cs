using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using Grpc.Core;
using CyberCrypt.D1.Client;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables("CONTOSO_");

// Add services to the container.
builder.Services.AddRazorPages();

var connectionString = builder.Configuration.GetConnectionString("SchoolContext") ?? throw new InvalidOperationException("Connection string 'SchoolContext' not found.");

# region Changes

var insecureChannel = builder.Configuration.GetValue<bool>("D1:Insecure");
var channelCredentials = insecureChannel ? ChannelCredentials.Insecure : ChannelCredentials.SecureSsl;
var d1Url = builder.Configuration.GetValue<string>("D1:Url") ?? throw new InvalidOperationException("D1 Generic URL not found.");
var d1Username = builder.Configuration.GetValue<string>("D1:Username") ?? throw new InvalidOperationException("D1 Generic username not found.");
var d1Password = builder.Configuration.GetValue<string>("D1:Password") ?? throw new InvalidOperationException("D1 Generic password not found.");

builder.Services.AddScoped<Func<ID1Generic>>(x =>
{
    var channel = new D1Channel(new Uri(d1Url), d1Username, d1Password) { ChannelCredentials = channelCredentials };
    return () => new D1GenericClient(channel);
});

builder.Services.AddDbContext<SchoolContext>(options => options.UseSqlite(connectionString));

#endregion

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<SchoolContext>();
    DbInitializer.Initialize(context);
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
