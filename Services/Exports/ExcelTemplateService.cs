using ClosedXML.Excel;
using CafeManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Services.Exports;

public class ExcelTemplateService
{
    public byte[] GenerateInvoiceExcel(
        string invoiceNumber,
        DateTime invoiceDate,
        string customerName,
        List<(string productName, int quantity, decimal unitPrice, decimal total)> items,
        decimal subtotal,
        decimal tax,
        decimal total,
        string notes = "")
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Hóa Đơn");

            // Header
            worksheet.Cell("A1").Value = "QUÁN CÀ PHÊ";
            worksheet.Cell("A1").Style.Font.Bold = true;
            worksheet.Cell("A1").Style.Font.FontSize = 14;

            worksheet.Cell("A2").Value = "HÓA ĐƠN BÁN HÀNG";
            worksheet.Cell("A2").Style.Font.Bold = true;
            worksheet.Cell("A2").Style.Font.FontSize = 12;

            // Invoice details
            worksheet.Cell("A4").Value = $"Số hóa đơn: {invoiceNumber}";
            worksheet.Cell("A5").Value = $"Ngày: {invoiceDate:dd/MM/yyyy}";
            worksheet.Cell("A6").Value = $"Khách hàng: {customerName}";

            // Table header
            int row = 8;
            worksheet.Cell($"A{row}").Value = "Sản phẩm";
            worksheet.Cell($"B{row}").Value = "SL";
            worksheet.Cell($"C{row}").Value = "Đơn giá";
            worksheet.Cell($"D{row}").Value = "Thành tiền";

            var headerRow = worksheet.Row(row);
            headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRow.Style.Font.Bold = true;

            // Items
            row++;
            foreach (var item in items)
            {
                worksheet.Cell($"A{row}").Value = item.productName;
                worksheet.Cell($"B{row}").Value = item.quantity;
                worksheet.Cell($"C{row}").Value = item.unitPrice;
                worksheet.Cell($"D{row}").Value = item.total;
                worksheet.Cell($"C{row}").Style.NumberFormat.Format = "#,##0\" ₫\"";
                worksheet.Cell($"D{row}").Style.NumberFormat.Format = "#,##0\" ₫\"";
                row++;
            }

            // Totals
            row += 1;
            worksheet.Cell($"A{row}").Value = "Tổng tiền:";
            worksheet.Cell($"B{row}").Value = subtotal;
            worksheet.Cell($"B{row}").Style.NumberFormat.Format = "#,##0\" ₫\"";
            worksheet.Cell($"B{row}").Style.Font.Bold = true;

            row++;
            worksheet.Cell($"A{row}").Value = "Thuế (10%):";
            worksheet.Cell($"B{row}").Value = tax;
            worksheet.Cell($"B{row}").Style.NumberFormat.Format = "#,##0\" ₫\"";
            worksheet.Cell($"B{row}").Style.Font.Bold = true;

            row++;
            worksheet.Cell($"A{row}").Value = "TỔNG CỘNG:";
            worksheet.Cell($"B{row}").Value = total;
            worksheet.Cell($"B{row}").Style.NumberFormat.Format = "#,##0\" ₫\"";
            worksheet.Cell($"B{row}").Style.Font.Bold = true;
            worksheet.Cell($"B{row}").Style.Font.FontSize = 12;

            if (!string.IsNullOrWhiteSpace(notes))
            {
                row += 2;
                worksheet.Cell($"A{row}").Value = "Ghi chú:";
                worksheet.Cell($"A{row}").Style.Font.Bold = true;
                row++;
                worksheet.Cell($"A{row}").Value = notes;
            }

            // Adjust column widths
            worksheet.Column("A").Width = 30;
            worksheet.Column("B").Width = 12;
            worksheet.Column("C").Width = 15;
            worksheet.Column("D").Width = 15;

            var memoryStream = new MemoryStream();
            workbook.SaveAs(memoryStream);
            return memoryStream.ToArray();
        }
    }

    public byte[] GenerateRevenueReportExcel(
        DateTime fromDate,
        DateTime toDate,
        string reportType,
        List<(string label, decimal revenue)> data,
        decimal totalRevenue)
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Doanh Thu");

            // Header
            worksheet.Cell("A1").Value = "QUÁN CÀ PHÊ";
            worksheet.Cell("A1").Style.Font.Bold = true;
            worksheet.Cell("A1").Style.Font.FontSize = 14;

            worksheet.Cell("A2").Value = "BÁO CÁO DOANH THU";
            worksheet.Cell("A2").Style.Font.Bold = true;
            worksheet.Cell("A2").Style.Font.FontSize = 12;

            worksheet.Cell("A3").Value = $"Từ {fromDate:dd/MM/yyyy} đến {toDate:dd/MM/yyyy}";
            worksheet.Cell("A4").Value = $"Loại báo cáo: {reportType}";

            // Table header
            int row = 6;
            worksheet.Cell($"A{row}").Value = "Thời kỳ";
            worksheet.Cell($"B{row}").Value = "Doanh thu";

            var headerRow = worksheet.Row(row);
            headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRow.Style.Font.Bold = true;

            // Data
            row++;
            foreach (var item in data)
            {
                worksheet.Cell($"A{row}").Value = item.label;
                worksheet.Cell($"B{row}").Value = item.revenue;
                worksheet.Cell($"B{row}").Style.NumberFormat.Format = "#,##0\" ₫\"";
                row++;
            }

            // Total
            worksheet.Cell($"A{row}").Value = "TỔNG CỘNG";
            worksheet.Cell($"A{row}").Style.Font.Bold = true;
            worksheet.Cell($"B{row}").Value = totalRevenue;
            worksheet.Cell($"B{row}").Style.NumberFormat.Format = "#,##0\" ₫\"";
            worksheet.Cell($"B{row}").Style.Font.Bold = true;
            var totalRow = worksheet.Row(row);
            totalRow.Style.Fill.BackgroundColor = XLColor.LightYellow;

            // Footer
            row += 2;
            worksheet.Cell($"A{row}").Value = $"Ngày in: {DateTime.Now:dd/MM/yyyy HH:mm}";

            // Adjust column widths
            worksheet.Column("A").Width = 25;
            worksheet.Column("B").Width = 20;

            var memoryStream = new MemoryStream();
            workbook.SaveAs(memoryStream);
            return memoryStream.ToArray();
        }
    }

    public byte[] GenerateInventoryReportExcel(
        List<(string productName, string unit, int currentStock, int minStock, decimal unitCost, decimal totalValue, string status)> inventory)
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Tồn Kho");

            // Header
            worksheet.Cell("A1").Value = "QUÁN CÀ PHÊ";
            worksheet.Cell("A1").Style.Font.Bold = true;
            worksheet.Cell("A1").Style.Font.FontSize = 14;

            worksheet.Cell("A2").Value = "BÁO CÁO TỒN KHO";
            worksheet.Cell("A2").Style.Font.Bold = true;
            worksheet.Cell("A2").Style.Font.FontSize = 12;

            worksheet.Cell("A3").Value = $"Ngày: {DateTime.Now:dd/MM/yyyy}";

            // Table header
            int row = 5;
            worksheet.Cell($"A{row}").Value = "Sản phẩm";
            worksheet.Cell($"B{row}").Value = "Đơn vị";
            worksheet.Cell($"C{row}").Value = "Tồn kho";
            worksheet.Cell($"D{row}").Value = "Tối thiểu";
            worksheet.Cell($"E{row}").Value = "Đơn giá";
            worksheet.Cell($"F{row}").Value = "Giá trị";
            worksheet.Cell($"G{row}").Value = "Trạng thái";

            var headerRow = worksheet.Row(row);
            headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRow.Style.Font.Bold = true;

            // Data
            row++;
            decimal totalValue = 0;
            foreach (var item in inventory)
            {
                worksheet.Cell($"A{row}").Value = item.productName;
                worksheet.Cell($"B{row}").Value = item.unit;
                worksheet.Cell($"C{row}").Value = item.currentStock;
                worksheet.Cell($"D{row}").Value = item.minStock;
                worksheet.Cell($"E{row}").Value = item.unitCost;
                worksheet.Cell($"F{row}").Value = item.totalValue;
                worksheet.Cell($"G{row}").Value = item.status;

                worksheet.Cell($"E{row}").Style.NumberFormat.Format = "#,##0\" ₫\"";
                worksheet.Cell($"F{row}").Style.NumberFormat.Format = "#,##0\" ₫\"";

                // Highlight low stock items
                if (item.status.Contains("Cảnh báo"))
                {
                    worksheet.Row(row).Style.Fill.BackgroundColor = XLColor.LightYellow;
                }

                totalValue += item.totalValue;
                row++;
            }

            // Total
            worksheet.Cell($"A{row}").Value = "TỔNG GIÁ TRỊ TỒN KHO";
            worksheet.Cell($"A{row}").Style.Font.Bold = true;
            worksheet.Cell($"F{row}").Value = totalValue;
            worksheet.Cell($"F{row}").Style.NumberFormat.Format = "#,##0\" ₫\"";
            worksheet.Cell($"F{row}").Style.Font.Bold = true;
            var totalRow = worksheet.Row(row);
            totalRow.Style.Fill.BackgroundColor = XLColor.LightYellow;

            // Adjust column widths
            worksheet.Column("A").Width = 25;
            worksheet.Column("B").Width = 12;
            worksheet.Column("C").Width = 12;
            worksheet.Column("D").Width = 12;
            worksheet.Column("E").Width = 15;
            worksheet.Column("F").Width = 15;
            worksheet.Column("G").Width = 15;

            var memoryStream = new MemoryStream();
            workbook.SaveAs(memoryStream);
            return memoryStream.ToArray();
        }
    }
}
