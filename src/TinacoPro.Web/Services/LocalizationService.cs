using Microsoft.JSInterop;

namespace TinacoPro.Web.Services;

public class LocalizationService
{
    private readonly IJSRuntime _jsRuntime;
    private string _currentLanguage = "en";
    private Dictionary<string, Dictionary<string, string>> _translations = new();

    public LocalizationService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        InitializeTranslations();
    }

    public string CurrentLanguage => _currentLanguage;

    public event Action? OnLanguageChanged;

    private void InitializeTranslations()
    {
        _translations = new Dictionary<string, Dictionary<string, string>>
        {
            ["en"] = new Dictionary<string, string>
            {
                ["Dashboard"] = "Dashboard",
                ["Products"] = "Products",
                ["RawMaterials"] = "Raw Materials",
                ["ProductionOrders"] = "Production Orders",
                ["Suppliers"] = "Suppliers",
                ["Reports"] = "Reports",
                ["ProductCatalog"] = "Product Catalog",
                ["RawMaterialsInventory"] = "Raw Materials Inventory",
                ["NewProduct"] = "New Product",
                ["NewMaterial"] = "New Material",
                ["NewProductionOrder"] = "New Production Order",
                ["NewSupplier"] = "New Supplier",
                ["Name"] = "Name",
                ["Model"] = "Model",
                ["Size"] = "Size",
                ["Capacity"] = "Capacity (L)",
                ["Status"] = "Status",
                ["Actions"] = "Actions",
                ["Edit"] = "Edit",
                ["Delete"] = "Delete",
                ["Active"] = "Active",
                ["Inactive"] = "Inactive",
                ["Code"] = "Code",
                ["Unit"] = "Unit",
                ["CurrentStock"] = "Current Stock",
                ["MinimumStock"] = "Minimum Stock",
                ["UnitCost"] = "Unit Cost",
                ["OrderNumber"] = "Order Number",
                ["Product"] = "Product",
                ["Quantity"] = "Quantity",
                ["OrderDate"] = "Order Date",
                ["CompletedDate"] = "Completed Date",
                ["Pending"] = "Pending",
                ["InProgress"] = "In Progress",
                ["Completed"] = "Completed",
                ["Contact"] = "Contact",
                ["Phone"] = "Phone",
                ["Email"] = "Email",
                ["Address"] = "Address",
                ["ProductionSummary"] = "Production Summary",
                ["InventoryStatus"] = "Inventory Status",
                ["ProductCatalogSummary"] = "Product Catalog Summary",
                ["Today"] = "Today",
                ["ThisWeek"] = "This Week",
                ["ThisMonth"] = "This Month",
                ["OrdersCompleted"] = "Orders Completed",
                ["TotalUnits"] = "Total Units",
                ["LowStockAlert"] = "Low Stock Alert!",
                ["MaterialsRunningLow"] = "materials are running low",
                ["AllMaterialsStocked"] = "All materials are adequately stocked.",
                ["TotalProducts"] = "Total Products",
                ["ActiveProducts"] = "Active Products",
                ["InactiveProducts"] = "Inactive Products",
                ["LowStockItems"] = "Low Stock Items",
                ["Loading"] = "Loading...",
                ["DailyProduction"] = "Daily Production",
                ["StockIn"] = "Stock In",
                ["StockOut"] = "Stock Out",
                ["ProductionDate"] = "Production Date",
                ["SaveDailyProduction"] = "Save Daily Production",
                ["HistoricalProduction"] = "Historical Production",
                ["TotalRawMaterials"] = "Total Raw Materials",
                ["ProductionOverview"] = "Production Overview",
                ["PendingOrders"] = "Pending Orders",
                ["CompletedToday"] = "Completed Today",
                ["CompletedThisWeek"] = "Completed This Week",
                ["CompletedThisMonth"] = "Completed This Month",
                ["Orders"] = "Orders",
                ["AdequateStock"] = "Adequate Stock",
            },
            ["es"] = new Dictionary<string, string>
            {
                ["Dashboard"] = "Panel de Control",
                ["Products"] = "Productos",
                ["RawMaterials"] = "Materias Primas",
                ["ProductionOrders"] = "Órdenes de Producción",
                ["Suppliers"] = "Proveedores",
                ["Reports"] = "Reportes",
                ["ProductCatalog"] = "Catálogo de Productos",
                ["RawMaterialsInventory"] = "Inventario de Materias Primas",
                ["NewProduct"] = "Nuevo Producto",
                ["NewMaterial"] = "Nueva Materia Prima",
                ["NewProductionOrder"] = "Nueva Orden de Producción",
                ["NewSupplier"] = "Nuevo Proveedor",
                ["Name"] = "Nombre",
                ["Model"] = "Modelo",
                ["Size"] = "Tamaño",
                ["Capacity"] = "Capacidad (L)",
                ["Status"] = "Estado",
                ["Actions"] = "Acciones",
                ["Edit"] = "Editar",
                ["Delete"] = "Eliminar",
                ["Active"] = "Activo",
                ["Inactive"] = "Inactivo",
                ["Code"] = "Código",
                ["Unit"] = "Unidad",
                ["CurrentStock"] = "Stock Actual",
                ["MinimumStock"] = "Stock Mínimo",
                ["UnitCost"] = "Costo Unitario",
                ["OrderNumber"] = "Número de Orden",
                ["Product"] = "Producto",
                ["Quantity"] = "Cantidad",
                ["OrderDate"] = "Fecha de Orden",
                ["CompletedDate"] = "Fecha de Completado",
                ["Pending"] = "Pendiente",
                ["InProgress"] = "En Progreso",
                ["Completed"] = "Completado",
                ["Contact"] = "Contacto",
                ["Phone"] = "Teléfono",
                ["Email"] = "Correo",
                ["Address"] = "Dirección",
                ["ProductionSummary"] = "Resumen de Producción",
                ["InventoryStatus"] = "Estado de Inventario",
                ["ProductCatalogSummary"] = "Resumen de Catálogo",
                ["Today"] = "Hoy",
                ["ThisWeek"] = "Esta Semana",
                ["ThisMonth"] = "Este Mes",
                ["OrdersCompleted"] = "Órdenes Completadas",
                ["TotalUnits"] = "Unidades Totales",
                ["LowStockAlert"] = "¡Alerta de Stock Bajo!",
                ["MaterialsRunningLow"] = "materiales tienen stock bajo",
                ["AllMaterialsStocked"] = "Todas las materias primas tienen stock adecuado.",
                ["TotalProducts"] = "Total de Productos",
                ["ActiveProducts"] = "Productos Activos",
                ["InactiveProducts"] = "Productos Inactivos",
                ["LowStockItems"] = "Artículos con Stock Bajo",
                ["Loading"] = "Cargando...",
                ["DailyProduction"] = "Producción Diaria",
                ["StockIn"] = "Entrada de Stock",
                ["StockOut"] = "Salida de Stock",
                ["ProductionDate"] = "Fecha de Producción",
                ["SaveDailyProduction"] = "Guardar Producción Diaria",
                ["HistoricalProduction"] = "Producción Histórica",
                ["TotalRawMaterials"] = "Total de Materias Primas",
                ["ProductionOverview"] = "Resumen de Producción",
                ["PendingOrders"] = "Órdenes Pendientes",
                ["CompletedToday"] = "Completado Hoy",
                ["CompletedThisWeek"] = "Completado Esta Semana",
                ["CompletedThisMonth"] = "Completado Este Mes",
                ["Orders"] = "Órdenes",
                ["AdequateStock"] = "Stock Adecuado",
            }
        };
    }

    public async Task InitializeAsync()
    {
        try
        {
            var storedLanguage = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "language");
            if (!string.IsNullOrEmpty(storedLanguage) && _translations.ContainsKey(storedLanguage))
            {
                _currentLanguage = storedLanguage;
            }
        }
        catch
        {
            // If localStorage is not available, use default language
            _currentLanguage = "en";
        }
    }

    public string Translate(string key)
    {
        if (_translations.ContainsKey(_currentLanguage) && _translations[_currentLanguage].ContainsKey(key))
        {
            return _translations[_currentLanguage][key];
        }
        return key; // Return key if translation not found
    }

    public async Task SetLanguageAsync(string language)
    {
        if (_translations.ContainsKey(language))
        {
            _currentLanguage = language;
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "language", language);
            OnLanguageChanged?.Invoke();
        }
    }
}
