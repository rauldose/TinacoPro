using Microsoft.EntityFrameworkCore;
using TinacoPro.Domain.Entities;

namespace TinacoPro.Infrastructure.Data;

public class TinacoProDbContext : DbContext
{
    public TinacoProDbContext(DbContextOptions<TinacoProDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<RawMaterial> RawMaterials => Set<RawMaterial>();
    public DbSet<ProductMaterial> ProductMaterials => Set<ProductMaterial>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<ProductionOrder> ProductionOrders => Set<ProductionOrder>();
    public DbSet<FinishedGood> FinishedGoods => Set<FinishedGood>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Product configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Model).HasMaxLength(100);
            entity.Property(e => e.Size).HasMaxLength(50);
            entity.Property(e => e.Capacity).HasPrecision(18, 2);
        });

        // RawMaterial configuration
        modelBuilder.Entity<RawMaterial>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.Unit).HasMaxLength(50);
            entity.Property(e => e.CurrentStock).HasPrecision(18, 2);
            entity.Property(e => e.MinimumStock).HasPrecision(18, 2);
            entity.Property(e => e.UnitCost).HasPrecision(18, 2);
        });

        // ProductMaterial configuration
        modelBuilder.Entity<ProductMaterial>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.QuantityRequired).HasPrecision(18, 2);
            
            entity.HasOne(e => e.Product)
                .WithMany(p => p.ProductMaterials)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.RawMaterial)
                .WithMany(r => r.ProductMaterials)
                .HasForeignKey(e => e.RawMaterialId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // StockMovement configuration
        modelBuilder.Entity<StockMovement>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Quantity).HasPrecision(18, 2);
            entity.Property(e => e.PreviousStock).HasPrecision(18, 2);
            entity.Property(e => e.NewStock).HasPrecision(18, 2);
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.Reference).HasMaxLength(100);
            
            entity.HasOne(e => e.RawMaterial)
                .WithMany(r => r.StockMovements)
                .HasForeignKey(e => e.RawMaterialId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Supplier)
                .WithMany(s => s.StockMovements)
                .HasForeignKey(e => e.SupplierId)
                .OnDelete(DeleteBehavior.SetNull);
            
            entity.HasOne(e => e.ProductionOrder)
                .WithMany(o => o.StockMovements)
                .HasForeignKey(e => e.ProductionOrderId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ProductionOrder configuration
        modelBuilder.Entity<ProductionOrder>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.OrderNumber).IsUnique();
            entity.Property(e => e.Notes).HasMaxLength(1000);
            
            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // FinishedGood configuration
        modelBuilder.Entity<FinishedGood>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CurrentStock).HasPrecision(18, 2);
            entity.Property(e => e.BatchNumber).HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(500);
            
            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.ProductionOrder)
                .WithMany(o => o.FinishedGoods)
                .HasForeignKey(e => e.ProductionOrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Supplier configuration
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ContactName).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Address).HasMaxLength(500);
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(200);
        });

        // Seed initial data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        // Seed admin user (password: admin123)
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                FullName = "Administrator",
                Email = "admin@tinacopro.com",
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = seedDate
            }
        );

        // Seed sample products
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Tinaco Estándar", Model = "TE-500", Size = "Pequeño", Capacity = 500, Description = "Tinaco de 500 litros", IsActive = true, CreatedAt = seedDate },
            new Product { Id = 2, Name = "Tinaco Grande", Model = "TG-1000", Size = "Mediano", Capacity = 1000, Description = "Tinaco de 1000 litros", IsActive = true, CreatedAt = seedDate },
            new Product { Id = 3, Name = "Tinaco Premium", Model = "TP-2000", Size = "Grande", Capacity = 2000, Description = "Tinaco premium de 2000 litros", IsActive = true, CreatedAt = seedDate }
        );

        // Seed sample raw materials
        modelBuilder.Entity<RawMaterial>().HasData(
            new RawMaterial { Id = 1, Name = "Polietileno", Code = "PE-001", Unit = "kg", CurrentStock = 1000, MinimumStock = 200, UnitCost = 25.50m, IsActive = true, CreatedAt = seedDate },
            new RawMaterial { Id = 2, Name = "Colorante Negro", Code = "CN-001", Unit = "kg", CurrentStock = 50, MinimumStock = 10, UnitCost = 45.00m, IsActive = true, CreatedAt = seedDate },
            new RawMaterial { Id = 3, Name = "Aditivo UV", Code = "UV-001", Unit = "liters", CurrentStock = 30, MinimumStock = 5, UnitCost = 120.00m, IsActive = true, CreatedAt = seedDate }
        );

        // Seed sample suppliers
        modelBuilder.Entity<Supplier>().HasData(
            new Supplier { Id = 1, Name = "Proveedora de Plásticos SA", ContactName = "Juan Pérez", Phone = "555-1234", Email = "ventas@plasticos.com", Address = "Calle Principal 123", IsActive = true, CreatedAt = seedDate },
            new Supplier { Id = 2, Name = "Químicos Industriales", ContactName = "María García", Phone = "555-5678", Email = "contacto@quimicos.com", Address = "Av. Industrial 456", IsActive = true, CreatedAt = seedDate }
        );
    }
}
