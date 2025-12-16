var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient("ProfileApi", httpClientConfiguration =>
{
    var baseAddress = builder.Configuration["ProfileApiBaseAddress"];
    ArgumentNullException.ThrowIfNullOrWhiteSpace(baseAddress);
    httpClientConfiguration.BaseAddress = new Uri(baseAddress);
});

var app = builder.Build();

app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Profiles}/{action=Index}/{id?}");

app.Run();
