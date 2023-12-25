using WalletMobileApp.API.Contracts;
using WalletMobileApp.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.WindowsServices;

WebApplicationOptions options = new()
{
    Args = args,
    // If hosted as windows service, set the path from the assembly resolver
    ContentRootPath = WindowsServiceHelpers.IsWindowsService()
                      ? AppContext.BaseDirectory
                      : default
};

var builder = WebApplication.CreateBuilder(options);
builder.Host.UseWindowsService();
builder.WebHost.ConfigureKestrel((context, serverOptions) =>
{
    var kestrelSection = context.Configuration.GetSection("Kestrel");

    serverOptions.Configure(kestrelSection);
});
builder.WebHost.UseUrls("http://127.0.0.1:5000;https://127.0.0.1:5001");
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var connectionString = builder.Configuration.GetConnectionString("BusMobileApp");
builder.Services.AddDbContext<BusAppDbContext>(x => x.UseSqlServer(connectionString));

builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
