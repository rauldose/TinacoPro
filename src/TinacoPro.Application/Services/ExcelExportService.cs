using ClosedXML.Excel;
using TinacoPro.Application.DTOs;

namespace TinacoPro.Application.Services;

public class ExcelExportService
{
    public byte[] ExportProductionReport(
        IEnumerable<ProductionOrderDto> orders,
        DateTime? startDate,
        DateTime? endDate)
    {
        var filteredOrders = orders
            .Where(o => o.Status == "Completed" 
                && o.CompletedDate >= startDate 
                && o.CompletedDate <= endDate)
            .OrderByDescending(o => o.CompletedDate)
            .ToList();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Production Summary");

        // Title
        worksheet.Cell(1, 1).Value = "Production Summary Report";
        worksheet.Cell(1, 1).Style.Font.Bold = true;
        worksheet.Cell(1, 1).Style.Font.FontSize = 16;
        worksheet.Range(1, 1, 1, 4).Merge();

        // Date range
        worksheet.Cell(2, 1).Value = $"Period: {startDate?.ToString("MMM dd, yyyy")} - {endDate?.ToString("MMM dd, yyyy")}";
        worksheet.Range(2, 1, 2, 4).Merge();

        // Summary metrics
        worksheet.Cell(4, 1).Value = "Summary Metrics";
        worksheet.Cell(4, 1).Style.Font.Bold = true;
        worksheet.Cell(4, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        worksheet.Range(4, 1, 4, 2).Merge();

        worksheet.Cell(5, 1).Value = "Orders Completed:";
        worksheet.Cell(5, 2).Value = filteredOrders.Count;

        worksheet.Cell(6, 1).Value = "Total Units Produced:";
        worksheet.Cell(6, 2).Value = filteredOrders.Sum(o => o.Quantity);

        worksheet.Cell(7, 1).Value = "Product Types:";
        worksheet.Cell(7, 2).Value = filteredOrders.GroupBy(o => o.ProductName).Count();

        worksheet.Cell(8, 1).Value = "Avg Units per Order:";
        worksheet.Cell(8, 2).Value = filteredOrders.Any() 
            ? filteredOrders.Sum(o => o.Quantity) / (double)filteredOrders.Count 
            : 0;
        worksheet.Cell(8, 2).Style.NumberFormat.Format = "0.0";

        // Detailed orders table
        var startRow = 10;
        worksheet.Cell(startRow, 1).Value = "Order Details";
        worksheet.Cell(startRow, 1).Style.Font.Bold = true;
        worksheet.Cell(startRow, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        worksheet.Range(startRow, 1, startRow, 5).Merge();

        // Headers
        startRow++;
        worksheet.Cell(startRow, 1).Value = "Order #";
        worksheet.Cell(startRow, 2).Value = "Product";
        worksheet.Cell(startRow, 3).Value = "Quantity";
        worksheet.Cell(startRow, 4).Value = "Status";
        worksheet.Cell(startRow, 5).Value = "Completed Date";
        
        var headerRange = worksheet.Range(startRow, 1, startRow, 5);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
        headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        // Data rows
        startRow++;
        foreach (var order in filteredOrders)
        {
            worksheet.Cell(startRow, 1).Value = order.OrderNumber;
            worksheet.Cell(startRow, 2).Value = order.ProductName;
            worksheet.Cell(startRow, 3).Value = order.Quantity;
            worksheet.Cell(startRow, 4).Value = order.Status;
            worksheet.Cell(startRow, 5).Value = order.CompletedDate?.ToString("MMM dd, yyyy") ?? "";
            startRow++;
        }

        // Auto-fit columns
        worksheet.Columns().AdjustToContents();

        // Save to memory stream
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public byte[] ExportInventoryReport(IEnumerable<RawMaterialDto> materials)
    {
        var lowStockItems = materials.Where(m => m.IsLowStock).OrderBy(m => m.Name).ToList();
        var allMaterials = materials.OrderBy(m => m.Name).ToList();

        using var workbook = new XLWorkbook();
        
        // Summary sheet
        var summarySheet = workbook.Worksheets.Add("Inventory Summary");

        // Title
        summarySheet.Cell(1, 1).Value = "Inventory Status Report";
        summarySheet.Cell(1, 1).Style.Font.Bold = true;
        summarySheet.Cell(1, 1).Style.Font.FontSize = 16;
        summarySheet.Range(1, 1, 1, 4).Merge();

        // Date
        summarySheet.Cell(2, 1).Value = $"Report Date: {DateTime.Now:MMM dd, yyyy}";
        summarySheet.Range(2, 1, 2, 4).Merge();

        // Summary metrics
        summarySheet.Cell(4, 1).Value = "Summary Metrics";
        summarySheet.Cell(4, 1).Style.Font.Bold = true;
        summarySheet.Cell(4, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        summarySheet.Range(4, 1, 4, 2).Merge();

        summarySheet.Cell(5, 1).Value = "Total Materials:";
        summarySheet.Cell(5, 2).Value = materials.Count();

        summarySheet.Cell(6, 1).Value = "Low Stock Alerts:";
        summarySheet.Cell(6, 2).Value = lowStockItems.Count;
        summarySheet.Cell(6, 2).Style.Font.FontColor = XLColor.Red;
        summarySheet.Cell(6, 2).Style.Font.Bold = true;

        summarySheet.Cell(7, 1).Value = "Adequate Stock:";
        summarySheet.Cell(7, 2).Value = allMaterials.Count - lowStockItems.Count;

        // All materials table
        var startRow = 9;
        summarySheet.Cell(startRow, 1).Value = "All Materials";
        summarySheet.Cell(startRow, 1).Style.Font.Bold = true;
        summarySheet.Cell(startRow, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
        summarySheet.Range(startRow, 1, startRow, 6).Merge();

        // Headers
        startRow++;
        summarySheet.Cell(startRow, 1).Value = "Code";
        summarySheet.Cell(startRow, 2).Value = "Name";
        summarySheet.Cell(startRow, 3).Value = "Category";
        summarySheet.Cell(startRow, 4).Value = "Current Stock";
        summarySheet.Cell(startRow, 5).Value = "Minimum Stock";
        summarySheet.Cell(startRow, 6).Value = "Status";
        
        var headerRange = summarySheet.Range(startRow, 1, startRow, 6);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
        headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

        // Data rows
        startRow++;
        foreach (var material in allMaterials)
        {
            summarySheet.Cell(startRow, 1).Value = material.Code;
            summarySheet.Cell(startRow, 2).Value = material.Name;
            summarySheet.Cell(startRow, 3).Value = material.Category;
            summarySheet.Cell(startRow, 4).Value = $"{material.CurrentStock} {material.Unit}";
            summarySheet.Cell(startRow, 5).Value = $"{material.MinimumStock} {material.Unit}";
            summarySheet.Cell(startRow, 6).Value = material.IsLowStock ? "LOW STOCK" : "OK";
            
            if (material.IsLowStock)
            {
                summarySheet.Cell(startRow, 6).Style.Font.FontColor = XLColor.Red;
                summarySheet.Cell(startRow, 6).Style.Font.Bold = true;
            }
            else
            {
                summarySheet.Cell(startRow, 6).Style.Font.FontColor = XLColor.Green;
            }
            
            startRow++;
        }

        // Auto-fit columns
        summarySheet.Columns().AdjustToContents();

        // Low stock alerts sheet
        if (lowStockItems.Any())
        {
            var alertSheet = workbook.Worksheets.Add("Low Stock Alerts");

            // Title
            alertSheet.Cell(1, 1).Value = "⚠️ Low Stock Alerts";
            alertSheet.Cell(1, 1).Style.Font.Bold = true;
            alertSheet.Cell(1, 1).Style.Font.FontSize = 16;
            alertSheet.Cell(1, 1).Style.Font.FontColor = XLColor.Red;
            alertSheet.Range(1, 1, 1, 5).Merge();

            // Warning message
            alertSheet.Cell(2, 1).Value = $"{lowStockItems.Count} materials require immediate attention!";
            alertSheet.Range(2, 1, 2, 5).Merge();

            // Headers
            var alertStartRow = 4;
            alertSheet.Cell(alertStartRow, 1).Value = "Material";
            alertSheet.Cell(alertStartRow, 2).Value = "Code";
            alertSheet.Cell(alertStartRow, 3).Value = "Current Stock";
            alertSheet.Cell(alertStartRow, 4).Value = "Minimum Stock";
            alertSheet.Cell(alertStartRow, 5).Value = "Shortage";
            
            var alertHeaderRange = alertSheet.Range(alertStartRow, 1, alertStartRow, 5);
            alertHeaderRange.Style.Font.Bold = true;
            alertHeaderRange.Style.Fill.BackgroundColor = XLColor.Red;
            alertHeaderRange.Style.Font.FontColor = XLColor.White;
            alertHeaderRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            // Data rows
            alertStartRow++;
            foreach (var material in lowStockItems)
            {
                alertSheet.Cell(alertStartRow, 1).Value = material.Name;
                alertSheet.Cell(alertStartRow, 2).Value = material.Code;
                alertSheet.Cell(alertStartRow, 3).Value = $"{material.CurrentStock} {material.Unit}";
                alertSheet.Cell(alertStartRow, 4).Value = $"{material.MinimumStock} {material.Unit}";
                alertSheet.Cell(alertStartRow, 5).Value = $"{(material.MinimumStock - material.CurrentStock):F1} {material.Unit}";
                alertSheet.Cell(alertStartRow, 5).Style.Font.FontColor = XLColor.Red;
                alertSheet.Cell(alertStartRow, 5).Style.Font.Bold = true;
                alertStartRow++;
            }

            // Auto-fit columns
            alertSheet.Columns().AdjustToContents();
        }

        // Save to memory stream
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
