using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using TinacoPro.Application.Services;
using TinacoPro.Domain.Interfaces;
using TinacoPro.Infrastructure.Data;
using TinacoPro.Infrastructure.Repositories;
using TinacoPro.Web.Components;
using TinacoPro.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add MudBlazor services
builder.Services.AddMudServices();

// Add Localization service
builder.Services.AddScoped<LocalizationService>();

// Configure SQLite database
builder.Services.AddDbContext<TinacoProDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    // Use split query to avoid cartesian explosion warning when loading multiple collections
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
});

// Register repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IRawMaterialRepository, RawMaterialRepository>();
builder.Services.AddScoped<IProductionOrderRepository, ProductionOrderRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductTemplateRepository, ProductTemplateRepository>();
builder.Services.AddScoped<IFinishedGoodRepository, FinishedGoodRepository>();
builder.Services.AddScoped<IMaterialConsumptionLogRepository, MaterialConsumptionLogRepository>();

// Register application services
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<RawMaterialService>();
builder.Services.AddScoped<ProductionOrderService>();
builder.Services.AddScoped<SupplierService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<ExcelExportService>();
builder.Services.AddScoped<ProductTemplateService>();
builder.Services.AddScoped<FinishedGoodsService>();

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TinacoProDbContext>();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
