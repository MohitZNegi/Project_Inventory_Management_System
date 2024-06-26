﻿using Inventory_Management_System.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Inventory_Management_System.Models;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inventory_Management_System.Areas.Identity.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>().ToTable("Users", "security");
        builder.Entity<IdentityRole>().ToTable("Roles", "security");
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", "security");
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims", "security");
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins", "security");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", "security");
        builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens", "security");
        builder.Entity<Product>()
        .Property(p => p.ProductID)
        .ValueGeneratedOnAdd();
    }

    public DbSet<Client> Client_Model { get; set; }
    public DbSet<Product> Product_Model { get; set; }
    public DbSet<Supplier> Supplier_Model { get; set; }
    public DbSet<ProductSupplier> ProductSupplier_Model { get; set;}
    public DbSet<CartItem> CartItem_Model { get; set; }
    public DbSet<Order> Order_Model { get; set; }
    public DbSet<OrderItem> OrderItem_Model { get; set; }
    public DbSet<Invoice> Invoice_Model { get; set; }
}
