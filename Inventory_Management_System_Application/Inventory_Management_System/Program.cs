using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Inventory_Management_System.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Inventory_Management_System.Service;
using WebPWrecover.Services;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

using Inventory_Management_System.Helpers;
using Inventory_Management_System.Interfaces;
using CloudinaryDotNet;
using Microsoft.Data.SqlClient;
using System.Configuration;




var builder = WebApplication.CreateBuilder(args);


var keyVaultEndpoint = new Uri(Environment.GetEnvironmentVariable("VaultUri"));
builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());

//builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());
//var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IPhotoService, PhotoService>();

builder.Services.AddSingleton(new Cloudinary(new Account(
    "dup5hdi05",
    "159694719361822",
    "fnnNRbmmRg2qhv2PD3yoc0GTaZE"
)));

builder.Services.AddScoped<CartService>();

builder.Services.AddHttpContextAccessor();

builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = true; // Require email confirmation
}).AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddEntityFrameworkStores<ApplicationDbContext>();// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration.GetSection("SendGrid"));
builder.Services.AddControllersWithViews();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.UseCors(); // Enable CORS
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "ProductItems",
    pattern: "Product/ProductItems/{id:int}",
    defaults: new { controller = "Product", action = "ProductItems" }
);

app.MapControllerRoute(
    name: "Products",
    pattern: "Product/Products",
    defaults: new { controller = "Product", action = "Products" }
);
app.MapControllerRoute(
    name: "Products",
    pattern: "Product/Products",
    defaults: new { controller = "Product", action = "Products" }
);

app.MapControllerRoute(
    name: "aboutUs",
    pattern: "about-us",
    defaults: new { controller = "Home", action = "AboutUs" }
);

app.MapControllerRoute(
    name: "contactUs",
    pattern: "contact-us",
    defaults: new { controller = "Home", action = "ContactUs" }
);

app.MapControllerRoute(
    name: "TermsAndServices",
    pattern: "TermsAndServices",
    defaults: new { controller = "Home", action = "TermsAndServices" }
);

app.MapControllerRoute(
    name: "FAQs",
    pattern: "FAQs",
    defaults: new { controller = "Home", action = "FAQs" }
);

app.MapControllerRoute(
    name: "RefundPolicy",
    pattern: "RefundPolicy",
    defaults: new { controller = "Home", action = "RefundPolicy" }
);
app.MapControllerRoute(
    name: "Register",
    pattern: "Register",
    defaults: new { controller = "Home", action = "Register" }
);


// to access the identity pages- add razor support
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    await DbSeeder.SeedRolesAndAdminAsync(scope.ServiceProvider);
}


app.Run();
