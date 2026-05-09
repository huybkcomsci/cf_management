using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;

namespace CafeManagement.Services.Exports;

public static class PdfFontHelper
{
    public static void RegisterVietnameseFonts()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public static string DefaultFont => "Arial";
}

public class PdfTemplateService
{
    public PdfTemplateService()
    {
        PdfFontHelper.RegisterVietnameseFonts();
    }

    public byte[] GenerateInvoicePdf(
        string invoiceNumber,
        DateTime invoiceDate,
        string customerName,
        List<(string productName, int quantity, decimal unitPrice, decimal total)> items,
        decimal subtotal,
        decimal tax,
        decimal total,
        string notes = "")
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);
                page.DefaultTextStyle(x => x.FontFamily(PdfFontHelper.DefaultFont).FontSize(11));

                page.Header().Element(header =>
                {
                    header.Row(row =>
                    {
                        row.RelativeItem().Element(col =>
                        {
                            col.Text("HÓA ĐƠN BÁN HÀNG").Bold().FontSize(16);
                        });

                        row.RelativeItem().AlignRight().Element(col =>
                        {
                            col.Column(c =>
                            {
                                c.Item().Text("QUÁN CÀ PHÊ").Bold().FontSize(12);
                                c.Item().Text("Địa chỉ: Tp. HCM").FontSize(9);
                                c.Item().Text("ĐT: 0123 456 789").FontSize(9);
                            });
                        });
                    });
                });

                page.Content().Element(content =>
                {
                    content.Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Element(c =>
                            {
                                c.Text($"Số hóa đơn: {invoiceNumber}").Bold();
                                c.Text($"Ngày: {invoiceDate:dd/MM/yyyy}");
                            });

                            row.RelativeItem().AlignRight().Element(c =>
                            {
                                c.Text($"Khách hàng: {customerName}");
                            });
                        });

                        col.Item().PaddingTop(10).PaddingBottom(5).Element(c =>
                        {
                            c.BorderBottom(1).BorderColor("#000");
                        });

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(1.5f);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background("#e8e8e8").Padding(5).Text("Sản phẩm").Bold();
                                header.Cell().Background("#e8e8e8").Padding(5).AlignCenter().Text("SL").Bold();
                                header.Cell().Background("#e8e8e8").Padding(5).AlignRight().Text("Đơn giá").Bold();
                                header.Cell().Background("#e8e8e8").Padding(5).AlignRight().Text("Thành tiền").Bold();
                            });

                            foreach (var item in items)
                            {
                                table.Cell().Padding(5).Text(item.productName);
                                table.Cell().Padding(5).AlignCenter().Text(item.quantity.ToString());
                                table.Cell().Padding(5).AlignRight().Text($"{item.unitPrice:N0}đ");
                                table.Cell().Padding(5).AlignRight().Text($"{item.total:N0}đ");
                            }
                        });

                        col.Item().PaddingTop(5).PaddingBottom(5).Element(c =>
                        {
                            c.BorderBottom(1).BorderColor("#000");
                        });

                        col.Item().Row(row =>
                        {
                            row.RelativeItem(3);
                            row.RelativeItem().Element(totals =>
                            {
                                totals.Column(c =>
                                {
                                    c.Item().Row(r =>
                                    {
                                        r.RelativeItem(2).Text("Tổng tiền:");
                                        r.RelativeItem().AlignRight().Text($"{subtotal:N0}đ");
                                    });
                                    c.Item().Row(r =>
                                    {
                                        r.RelativeItem(2).Text("Thuế (10%):");
                                        r.RelativeItem().AlignRight().Text($"{tax:N0}đ");
                                    });
                                    c.Item().Row(r =>
                                    {
                                        r.RelativeItem(2).Text("TỔNG CỘNG:").Bold().FontSize(12);
                                        r.RelativeItem().AlignRight().Text($"{total:N0}đ").Bold().FontSize(12);
                                    });
                                });
                            });
                        });

                        if (!string.IsNullOrWhiteSpace(notes))
                        {
                            col.Item().PaddingTop(10).Column(c =>
                            {
                                c.Item().Text("Ghi chú:").Bold();
                                c.Item().Text(notes);
                            });
                        }

                        col.Item().PaddingTop(20).AlignCenter().Text("Cảm ơn quý khách!").Italic().FontSize(9);
                    });
                });
            });
        });

        return document.GeneratePdf();
    }

    public byte[] GenerateRevenueReportPdf(
        DateTime fromDate,
        DateTime toDate,
        string reportType,
        List<(string label, decimal revenue)> data,
        decimal totalRevenue)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);
                page.DefaultTextStyle(x => x.FontFamily(PdfFontHelper.DefaultFont).FontSize(11));

                page.Header().Element(header =>
                {
                    header.AlignCenter().Column(col =>
                    {
                        col.Item().Text("QUÁN CÀ PHÊ").Bold().FontSize(14);
                        col.Item().Text("BÁO CÁO DOANH THU").Bold().FontSize(12);
                        col.Item().Text($"Từ {fromDate:dd/MM/yyyy} đến {toDate:dd/MM/yyyy}").FontSize(10);
                    });
                });

                page.Content().Element(content =>
                {
                    content.Column(col =>
                    {
                        col.Item().Text($"Loại báo cáo: {reportType}").Bold();
                        col.Item().PaddingTop(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background("#e8e8e8").Padding(5).Text("Thời kỳ").Bold();
                                header.Cell().Background("#e8e8e8").Padding(5).AlignRight().Text("Doanh thu").Bold();
                            });

                            foreach (var item in data)
                            {
                                table.Cell().Padding(5).Text(item.label);
                                table.Cell().Padding(5).AlignRight().Text($"{item.revenue:N0}đ");
                            }

                            table.Cell().Background("#f0f0f0").Padding(5).Text("TỔNG CỘNG").Bold();
                            table.Cell().Background("#f0f0f0").Padding(5).AlignRight().Text($"{totalRevenue:N0}đ").Bold();
                        });

                        col.Item().PaddingTop(20).AlignCenter().Text($"Ngày in: {DateTime.Now:dd/MM/yyyy HH:mm}")
                            .FontSize(9).Italic();
                    });
                });
            });
        });

        return document.GeneratePdf();
    }

    public byte[] GenerateInventoryReportPdf(
        List<(string productName, string unit, int currentStock, int minStock, decimal unitCost, decimal totalValue, string status)> inventory)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);
                page.DefaultTextStyle(x => x.FontFamily(PdfFontHelper.DefaultFont).FontSize(10));

                page.Header().Element(header =>
                {
                    header.AlignCenter().Column(col =>
                    {
                        col.Item().Text("QUÁN CÀ PHÊ").Bold().FontSize(14);
                        col.Item().Text("BÁO CÁO TỒN KHO").Bold().FontSize(12);
                        col.Item().Text($"Ngày: {DateTime.Now:dd/MM/yyyy}").FontSize(10);
                    });
                });

                page.Content().Element(content =>
                {
                    content.Column(col =>
                    {
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2.5f);
                                columns.RelativeColumn(1f);
                                columns.RelativeColumn(1.2f);
                                columns.RelativeColumn(1.2f);
                                columns.RelativeColumn(1.5f);
                                columns.RelativeColumn(1.5f);
                                columns.RelativeColumn(1f);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background("#e8e8e8").Padding(4).Text("Sản phẩm").Bold().FontSize(9);
                                header.Cell().Background("#e8e8e8").Padding(4).AlignCenter().Text("Đơn vị").Bold().FontSize(9);
                                header.Cell().Background("#e8e8e8").Padding(4).AlignCenter().Text("Tồn kho").Bold().FontSize(9);
                                header.Cell().Background("#e8e8e8").Padding(4).AlignCenter().Text("Tối thiểu").Bold().FontSize(9);
                                header.Cell().Background("#e8e8e8").Padding(4).AlignRight().Text("Đơn giá").Bold().FontSize(9);
                                header.Cell().Background("#e8e8e8").Padding(4).AlignRight().Text("Giá trị").Bold().FontSize(9);
                                header.Cell().Background("#e8e8e8").Padding(4).AlignCenter().Text("Trạng thái").Bold().FontSize(9);
                            });

                            foreach (var item in inventory)
                            {
                                var bgColor = item.status.Contains("Cảnh báo") ? "#fff3cd" : "white";
                                
                                table.Cell().Background(bgColor).Padding(4).Text(item.productName).FontSize(9);
                                table.Cell().Background(bgColor).Padding(4).AlignCenter().Text(item.unit).FontSize(9);
                                table.Cell().Background(bgColor).Padding(4).AlignCenter().Text(item.currentStock.ToString()).FontSize(9);
                                table.Cell().Background(bgColor).Padding(4).AlignCenter().Text(item.minStock.ToString()).FontSize(9);
                                table.Cell().Background(bgColor).Padding(4).AlignRight().Text($"{item.unitCost:N0}đ").FontSize(9);
                                table.Cell().Background(bgColor).Padding(4).AlignRight().Text($"{item.totalValue:N0}đ").FontSize(9);
                                table.Cell().Background(bgColor).Padding(4).AlignCenter().Text(item.status).FontSize(9);
                            }
                        });

                        col.Item().PaddingTop(15).AlignCenter().Text($"Ngày in: {DateTime.Now:dd/MM/yyyy HH:mm}")
                            .FontSize(9).Italic();
                    });
                });
            });
        });

        return document.GeneratePdf();
    }
}
