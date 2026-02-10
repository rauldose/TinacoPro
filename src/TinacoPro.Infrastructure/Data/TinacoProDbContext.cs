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
    public DbSet<Machine> Machines => Set<Machine>();
    public DbSet<AssemblyLog> AssemblyLogs => Set<AssemblyLog>();
    public DbSet<ProductTemplate> ProductTemplates => Set<ProductTemplate>();
    public DbSet<TemplatePart> TemplateParts => Set<TemplatePart>();
    public DbSet<MaterialConsumptionLog> MaterialConsumptionLogs => Set<MaterialConsumptionLog>();

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
            entity.Property(e => e.Color).HasMaxLength(50);
            entity.Property(e => e.Capacity).HasPrecision(18, 2);
            entity.Property(e => e.Weight).HasPrecision(18, 2);
            entity.Property(e => e.MaterialCost).HasPrecision(18, 2);
            entity.Property(e => e.LaborCost).HasPrecision(18, 2);
            entity.Property(e => e.OverheadCost).HasPrecision(18, 2);
            entity.Property(e => e.SellingPrice).HasPrecision(18, 2);
            entity.Property(e => e.ProfitMargin).HasPrecision(18, 2);
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
            
            entity.HasOne(e => e.AssignedMachine)
                .WithMany()
                .HasForeignKey(e => e.AssignedMachineId)
                .OnDelete(DeleteBehavior.SetNull);
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

        // Machine configuration
        modelBuilder.Entity<Machine>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CurrentModel).HasMaxLength(50);
            entity.Property(e => e.Temperature).HasPrecision(18, 2);
            entity.Property(e => e.RPM).HasPrecision(18, 2);
            
            entity.HasOne(e => e.CurrentProductionOrder)
                .WithMany()
                .HasForeignKey(e => e.CurrentProductionOrderId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // AssemblyLog configuration
        modelBuilder.Entity<AssemblyLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SerialNumber).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Operator).HasMaxLength(200);
            entity.Property(e => e.Station).HasMaxLength(50);
            entity.Property(e => e.DefectNotes).HasMaxLength(1000);
            
            entity.HasOne(e => e.ProductionOrder)
                .WithMany(o => o.AssemblyLogs)
                .HasForeignKey(e => e.ProductionOrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ProductTemplate configuration
        modelBuilder.Entity<ProductTemplate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ModelType).HasMaxLength(100);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(1000).HasDefaultValue(string.Empty);
            entity.Property(e => e.TotalMaterialCost).HasPrecision(18, 2);
            entity.Property(e => e.TotalLaborCost).HasPrecision(18, 2);
        });

        // TemplatePart configuration
        modelBuilder.Entity<TemplatePart>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PartType).HasMaxLength(50);
            entity.Property(e => e.Unit).HasMaxLength(50);
            entity.Property(e => e.Quantity).HasPrecision(18, 2);
            entity.Property(e => e.UnitCost).HasPrecision(18, 2);
            entity.Property(e => e.LaborCost).HasPrecision(18, 2);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            
            entity.HasOne(e => e.Template)
                .WithMany(t => t.Parts)
                .HasForeignKey(e => e.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.ParentPart)
                .WithMany(p => p.Children)
                .HasForeignKey(e => e.ParentPartId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.RawMaterial)
                .WithMany()
                .HasForeignKey(e => e.RawMaterialId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Product-Template relationship
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Template)
            .WithMany(t => t.Products)
            .HasForeignKey(p => p.TemplateId)
            .OnDelete(DeleteBehavior.SetNull);

        // MaterialConsumptionLog configuration
        modelBuilder.Entity<MaterialConsumptionLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.QuantityConsumed).HasPrecision(18, 2);
            entity.Property(e => e.UnitCostAtConsumption).HasPrecision(18, 2);
            entity.Property(e => e.TotalCost).HasPrecision(18, 2);
            entity.Property(e => e.Notes).HasMaxLength(500);
            
            entity.HasOne(e => e.ProductionOrder)
                .WithMany()
                .HasForeignKey(e => e.ProductionOrderId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.RawMaterial)
                .WithMany()
                .HasForeignKey(e => e.RawMaterialId)
                .OnDelete(DeleteBehavior.Restrict);
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

        // Seed sample products - 5 models as per tinaco-system.jsx
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Tinaco 450L", Model = "T-450", Size = "Pequeño", Capacity = 450, Color = "Negro", Layers = 3, Weight = 8.5m, Description = "Tinaco de 450 litros", IsActive = true, CreatedAt = seedDate },
            new Product { Id = 2, Name = "Tinaco 750L", Model = "T-750", Size = "Mediano", Capacity = 750, Color = "Negro", Layers = 3, Weight = 12.2m, Description = "Tinaco de 750 litros", IsActive = true, CreatedAt = seedDate },
            new Product { Id = 3, Name = "Tinaco 1100L", Model = "T-1100", Size = "Grande", Capacity = 1100, Color = "Negro", Layers = 3, Weight = 16.8m, Description = "Tinaco de 1100 litros", IsActive = true, CreatedAt = seedDate },
            new Product { Id = 4, Name = "Tinaco 2500L", Model = "T-2500", Size = "Extra Grande", Capacity = 2500, Color = "Negro", Layers = 3, Weight = 28.5m, Description = "Tinaco de 2500 litros", IsActive = true, CreatedAt = seedDate },
            new Product { Id = 5, Name = "Tinaco 5000L", Model = "T-5000", Size = "Industrial", Capacity = 5000, Color = "Negro", Layers = 3, Weight = 45.0m, Description = "Tinaco industrial de 5000 litros", IsActive = true, CreatedAt = seedDate }
        );

        // Seed comprehensive raw materials - matching tinaco-system.jsx
        modelBuilder.Entity<RawMaterial>().HasData(
            // Resinas
            new RawMaterial { Id = 1, Name = "Polietileno HD (Virgen)", Code = "RM-PE-HD", Unit = "kg", Category = MaterialCategory.Resina, CurrentStock = 12500, MinimumStock = 5000, UnitCost = 28.50m, IsActive = true, CreatedAt = seedDate },
            new RawMaterial { Id = 2, Name = "Polietileno Reciclado", Code = "RM-PE-R", Unit = "kg", Category = MaterialCategory.Resina, CurrentStock = 8200, MinimumStock = 3000, UnitCost = 15.00m, IsActive = true, CreatedAt = seedDate },
            // Aditivos
            new RawMaterial { Id = 3, Name = "Masterbatch Negro UV", Code = "RM-MB-N", Unit = "kg", Category = MaterialCategory.Aditivo, CurrentStock = 450, MinimumStock = 200, UnitCost = 85.00m, IsActive = true, CreatedAt = seedDate },
            new RawMaterial { Id = 4, Name = "Masterbatch Blanco", Code = "RM-MB-B", Unit = "kg", Category = MaterialCategory.Aditivo, CurrentStock = 320, MinimumStock = 150, UnitCost = 92.00m, IsActive = true, CreatedAt = seedDate },
            new RawMaterial { Id = 5, Name = "Estabilizador UV", Code = "RM-EST", Unit = "kg", Category = MaterialCategory.Aditivo, CurrentStock = 180, MinimumStock = 100, UnitCost = 120.00m, IsActive = true, CreatedAt = seedDate },
            // Accesorios
            new RawMaterial { Id = 6, Name = "Conector 1/2\" PVC", Code = "RM-CONN-1", Unit = "pz", Category = MaterialCategory.Accesorio, CurrentStock = 2400, MinimumStock = 1000, UnitCost = 8.50m, IsActive = true, CreatedAt = seedDate },
            new RawMaterial { Id = 7, Name = "Conector 3/4\" PVC", Code = "RM-CONN-2", Unit = "pz", Category = MaterialCategory.Accesorio, CurrentStock = 1800, MinimumStock = 800, UnitCost = 12.00m, IsActive = true, CreatedAt = seedDate },
            new RawMaterial { Id = 8, Name = "Flotador completo", Code = "RM-FLOT", Unit = "pz", Category = MaterialCategory.Accesorio, CurrentStock = 950, MinimumStock = 400, UnitCost = 45.00m, IsActive = true, CreatedAt = seedDate },
            new RawMaterial { Id = 9, Name = "Tapa con cierre", Code = "RM-TAPA", Unit = "pz", Category = MaterialCategory.Accesorio, CurrentStock = 1100, MinimumStock = 500, UnitCost = 22.00m, IsActive = true, CreatedAt = seedDate },
            new RawMaterial { Id = 10, Name = "Base reforzada", Code = "RM-BASE", Unit = "pz", Category = MaterialCategory.Accesorio, CurrentStock = 600, MinimumStock = 300, UnitCost = 35.00m, IsActive = true, CreatedAt = seedDate },
            // Empaque
            new RawMaterial { Id = 11, Name = "Etiqueta/Calcomanía", Code = "RM-ETIQ", Unit = "pz", Category = MaterialCategory.Empaque, CurrentStock = 3500, MinimumStock = 1500, UnitCost = 3.50m, IsActive = true, CreatedAt = seedDate },
            new RawMaterial { Id = 12, Name = "Película stretch", Code = "RM-FILM", Unit = "rollo", Category = MaterialCategory.Empaque, CurrentStock = 45, MinimumStock = 20, UnitCost = 280.00m, IsActive = true, CreatedAt = seedDate }
        );

        // Seed Bill of Materials (BOM) for each product
        modelBuilder.Entity<ProductMaterial>().HasData(
            // T-450
            new ProductMaterial { Id = 1, ProductId = 1, RawMaterialId = 1, QuantityRequired = 4.5m, CreatedAt = seedDate },
            new ProductMaterial { Id = 2, ProductId = 1, RawMaterialId = 2, QuantityRequired = 2.0m, CreatedAt = seedDate },
            new ProductMaterial { Id = 3, ProductId = 1, RawMaterialId = 3, QuantityRequired = 0.3m, CreatedAt = seedDate },
            new ProductMaterial { Id = 4, ProductId = 1, RawMaterialId = 5, QuantityRequired = 0.15m, CreatedAt = seedDate },
            new ProductMaterial { Id = 5, ProductId = 1, RawMaterialId = 6, QuantityRequired = 1, CreatedAt = seedDate },
            new ProductMaterial { Id = 6, ProductId = 1, RawMaterialId = 8, QuantityRequired = 1, CreatedAt = seedDate },
            new ProductMaterial { Id = 7, ProductId = 1, RawMaterialId = 9, QuantityRequired = 1, CreatedAt = seedDate },
            new ProductMaterial { Id = 8, ProductId = 1, RawMaterialId = 11, QuantityRequired = 1, CreatedAt = seedDate },
            // T-750
            new ProductMaterial { Id = 9, ProductId = 2, RawMaterialId = 1, QuantityRequired = 6.5m, CreatedAt = seedDate },
            new ProductMaterial { Id = 10, ProductId = 2, RawMaterialId = 2, QuantityRequired = 3.5m, CreatedAt = seedDate },
            new ProductMaterial { Id = 11, ProductId = 2, RawMaterialId = 3, QuantityRequired = 0.45m, CreatedAt = seedDate },
            new ProductMaterial { Id = 12, ProductId = 2, RawMaterialId = 5, QuantityRequired = 0.2m, CreatedAt = seedDate },
            new ProductMaterial { Id = 13, ProductId = 2, RawMaterialId = 6, QuantityRequired = 1, CreatedAt = seedDate },
            new ProductMaterial { Id = 14, ProductId = 2, RawMaterialId = 7, QuantityRequired = 1, CreatedAt = seedDate },
            new ProductMaterial { Id = 15, ProductId = 2, RawMaterialId = 8, QuantityRequired = 1, CreatedAt = seedDate },
            new ProductMaterial { Id = 16, ProductId = 2, RawMaterialId = 9, QuantityRequired = 1, CreatedAt = seedDate },
            new ProductMaterial { Id = 17, ProductId = 2, RawMaterialId = 10, QuantityRequired = 1, CreatedAt = seedDate },
            new ProductMaterial { Id = 18, ProductId = 2, RawMaterialId = 11, QuantityRequired = 1, CreatedAt = seedDate },
            // T-1100
            new ProductMaterial { Id = 19, ProductId = 3, RawMaterialId = 1, QuantityRequired = 9.0m, CreatedAt = seedDate },
            new ProductMaterial { Id = 20, ProductId = 3, RawMaterialId = 2, QuantityRequired = 5.0m, CreatedAt = seedDate },
            new ProductMaterial { Id = 21, ProductId = 3, RawMaterialId = 3, QuantityRequired = 0.6m, CreatedAt = seedDate },
            new ProductMaterial { Id = 22, ProductId = 3, RawMaterialId = 5, QuantityRequired = 0.3m, CreatedAt = seedDate },
            new ProductMaterial { Id = 23, ProductId = 3, RawMaterialId = 6, QuantityRequired = 1, CreatedAt = seedDate },
            new ProductMaterial { Id = 24, ProductId = 3, RawMaterialId = 7, QuantityRequired = 1, CreatedAt = seedDate },
            new ProductMaterial { Id = 25, ProductId = 3, RawMaterialId = 8, QuantityRequired = 1, CreatedAt = seedDate },
            new ProductMaterial { Id = 26, ProductId = 3, RawMaterialId = 9, QuantityRequired = 1, CreatedAt = seedDate },
            new ProductMaterial { Id = 27, ProductId = 3, RawMaterialId = 10, QuantityRequired = 1, CreatedAt = seedDate },
            new ProductMaterial { Id = 28, ProductId = 3, RawMaterialId = 11, QuantityRequired = 1, CreatedAt = seedDate },
            // T-2500
            new ProductMaterial { Id = 29, ProductId = 4, RawMaterialId = 1, QuantityRequired = 16.0m, CreatedAt = seedDate },
            new ProductMaterial { Id = 30, ProductId = 4, RawMaterialId = 2, QuantityRequired = 8.0m, CreatedAt = seedDate },
            new ProductMaterial { Id = 31, ProductId = 4, RawMaterialId = 3, QuantityRequired = 1.0m, CreatedAt = seedDate },
            new ProductMaterial { Id = 32, ProductId = 4, RawMaterialId = 4, QuantityRequired = 0.5m, CreatedAt = seedDate },
            new ProductMaterial { Id = 33, ProductId = 4, RawMaterialId = 5, QuantityRequired = 0.5m, CreatedAt = seedDate },
            new ProductMaterial { Id = 34, ProductId = 4, RawMaterialId = 7, QuantityRequired = 2, CreatedAt = seedDate },
            new ProductMaterial { Id = 35, ProductId = 4, RawMaterialId = 8, QuantityRequired = 1, CreatedAt = seedDate },
            new ProductMaterial { Id = 36, ProductId = 4, RawMaterialId = 9, QuantityRequired = 1, CreatedAt = seedDate },
            new ProductMaterial { Id = 37, ProductId = 4, RawMaterialId = 10, QuantityRequired = 1, CreatedAt = seedDate },
            new ProductMaterial { Id = 38, ProductId = 4, RawMaterialId = 11, QuantityRequired = 2, CreatedAt = seedDate },
            // T-5000
            new ProductMaterial { Id = 39, ProductId = 5, RawMaterialId = 1, QuantityRequired = 28.0m, CreatedAt = seedDate },
            new ProductMaterial { Id = 40, ProductId = 5, RawMaterialId = 2, QuantityRequired = 12.0m, CreatedAt = seedDate },
            new ProductMaterial { Id = 41, ProductId = 5, RawMaterialId = 3, QuantityRequired = 1.8m, CreatedAt = seedDate },
            new ProductMaterial { Id = 42, ProductId = 5, RawMaterialId = 4, QuantityRequired = 0.8m, CreatedAt = seedDate },
            new ProductMaterial { Id = 43, ProductId = 5, RawMaterialId = 5, QuantityRequired = 0.8m, CreatedAt = seedDate },
            new ProductMaterial { Id = 44, ProductId = 5, RawMaterialId = 7, QuantityRequired = 2, CreatedAt = seedDate },
            new ProductMaterial { Id = 45, ProductId = 5, RawMaterialId = 8, QuantityRequired = 1, CreatedAt = seedDate },
            new ProductMaterial { Id = 46, ProductId = 5, RawMaterialId = 9, QuantityRequired = 1, CreatedAt = seedDate },
            new ProductMaterial { Id = 47, ProductId = 5, RawMaterialId = 10, QuantityRequired = 1, CreatedAt = seedDate },
            new ProductMaterial { Id = 48, ProductId = 5, RawMaterialId = 11, QuantityRequired = 2, CreatedAt = seedDate },
            new ProductMaterial { Id = 49, ProductId = 5, RawMaterialId = 12, QuantityRequired = 0.5m, CreatedAt = seedDate }
        );

        // Seed machines
        modelBuilder.Entity<Machine>().HasData(
            new Machine { Id = 1, Code = "ROT-01", Name = "Rotomoldeadora #1", Type = MachineType.Rotomoldeo, Status = MachineStatus.Running, CurrentModel = "T-1100", CycleTime = 22, Temperature = 285, RPM = 8.5m, IsActive = true, CreatedAt = seedDate },
            new Machine { Id = 2, Code = "ROT-02", Name = "Rotomoldeadora #2", Type = MachineType.Rotomoldeo, Status = MachineStatus.Running, CurrentModel = "T-750", CycleTime = 18, Temperature = 280, RPM = 9.0m, IsActive = true, CreatedAt = seedDate },
            new Machine { Id = 3, Code = "ROT-03", Name = "Rotomoldeadora #3", Type = MachineType.Rotomoldeo, Status = MachineStatus.Idle, CycleTime = 0, Temperature = 25, RPM = 0, IsActive = true, CreatedAt = seedDate },
            new Machine { Id = 4, Code = "ROT-04", Name = "Rotomoldeadora #4", Type = MachineType.Rotomoldeo, Status = MachineStatus.Maintenance, CycleTime = 0, Temperature = 25, RPM = 0, IsActive = true, CreatedAt = seedDate },
            new Machine { Id = 5, Code = "ENF-01", Name = "Estación Enfriamiento #1", Type = MachineType.Enfriamiento, Status = MachineStatus.Running, IsActive = true, CreatedAt = seedDate },
            new Machine { Id = 6, Code = "REB-01", Name = "Desbarbadora", Type = MachineType.Rebabeo, Status = MachineStatus.Running, IsActive = true, CreatedAt = seedDate },
            new Machine { Id = 7, Code = "ENS-01", Name = "Estación Ensamble #1", Type = MachineType.Ensamble, Status = MachineStatus.Running, IsActive = true, CreatedAt = seedDate },
            new Machine { Id = 8, Code = "ENS-02", Name = "Estación Ensamble #2", Type = MachineType.Ensamble, Status = MachineStatus.Idle, IsActive = true, CreatedAt = seedDate },
            new Machine { Id = 9, Code = "PRU-01", Name = "Estación Prueba Hermeticidad", Type = MachineType.Prueba, Status = MachineStatus.Running, IsActive = true, CreatedAt = seedDate }
        );

        // Seed sample suppliers
        modelBuilder.Entity<Supplier>().HasData(
            new Supplier { Id = 1, Name = "Proveedora de Plásticos SA", ContactName = "Juan Pérez", Phone = "555-1234", Email = "ventas@plasticos.com", Address = "Calle Principal 123", IsActive = true, CreatedAt = seedDate },
            new Supplier { Id = 2, Name = "Químicos Industriales", ContactName = "María García", Phone = "555-5678", Email = "contacto@quimicos.com", Address = "Av. Industrial 456", IsActive = true, CreatedAt = seedDate }
        );
    }
}
