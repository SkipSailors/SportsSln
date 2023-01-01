using Microsoft.EntityFrameworkCore;
using SportsStore.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<StoreDbContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration["ConnectionStrings:SportsStoreConnection"]);
});
builder.Services.AddScoped<IStoreRepository, EFStoreRepository>();
builder.Services.AddScoped<IOrderRepository, EFOrderRepository>();
builder.Services.AddRazorPages();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddScoped(SessionCart.GetCart);
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
WebApplication app = builder.Build();

app.UseStaticFiles();
app.UseSession();
app.MapControllerRoute(
    "catpage",
    "{category}/Page{productPage:int}",
    new { Controller = "Home", action = "Index" });
app.MapControllerRoute(
    "page",
    "Page{productPage:int}",
    new { Controller = "Home", action = "Index", productPage = 1 });
app.MapControllerRoute(
    "category",
    "{category}",
    new { Controller = "Home", action = "Index", productPage = 1 });
app.MapControllerRoute(
    "pagination",
    "Products/Page{productPage}",
    new { Controller = "Home", action = "Index", productPage = 1 });
app.MapDefaultControllerRoute();
app.MapRazorPages();

SeedData.EnsurePopulated(app);
app.Run();