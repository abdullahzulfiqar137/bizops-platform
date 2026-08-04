using BusinessOperations.API.Entities.Sales;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BusinessOperations.API.Services.Pdf;

public sealed class PdfDocumentService : IPdfDocumentService
{
    static PdfDocumentService()
    {
        // QuestPDF Community License Configuration
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateSaleInvoicePdf(Sale sale)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial));

                // Header Component
                page.Header().Element(container => ComposeHeader(container, sale));

                // Main Document Body
                page.Content().Element(container => ComposeContent(container, sale));

                // Footer Component
                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    private static void ComposeHeader(IContainer container, Sale sale)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text("BIZOPS ERP ENTERPRISE")
                    .FontSize(20)
                    .Bold()
                    .FontColor(Colors.Blue.Darken2);

                column.Item().Text("Global Business Operations Workspace")
                    .FontSize(9)
                    .FontColor(Colors.Grey.Medium);

                column.Item().Text("Support & Inquiries: support@bizops-erp.com")
                    .FontSize(8)
                    .FontColor(Colors.Grey.Medium);
            });

            row.ConstantItem(200).Column(column =>
            {
                column.Item().AlignRight().Text("TAX INVOICE")
                    .FontSize(18)
                    .Bold()
                    .FontColor(Colors.Grey.Darken3);

                column.Item().AlignRight().Text($"Invoice #: {sale.InvoiceNumber}")
                    .FontSize(10)
                    .Bold();

                column.Item().AlignRight().Text($"Date: {sale.SaleDateUtc:yyyy-MM-dd}")
                    .FontSize(9)
                    .FontColor(Colors.Grey.Darken1);

                column.Item().AlignRight().Text($"Status: {sale.PaymentStatus}")
                    .FontSize(9)
                    .Bold()
                    .FontColor(sale.PaymentStatus == Enums.PaymentStatus.Paid ? Colors.Green.Medium : Colors.Orange.Medium);
            });
        });
    }

    private static void ComposeContent(IContainer container, Sale sale)
    {
        container.PaddingVertical(20).Column(column =>
        {
            // Customer Details Box
            column.Item().Background(Colors.Grey.Lighten4).Padding(12).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("BILLED TO:").FontSize(8).Bold().FontColor(Colors.Grey.Medium);
                    c.Item().Text(sale.Customer?.Name ?? "Walk-in Customer").FontSize(11).Bold();
                    if (!string.IsNullOrWhiteSpace(sale.Customer?.Email))
                        c.Item().Text($"Email: {sale.Customer.Email}").FontSize(9);
                    if (!string.IsNullOrWhiteSpace(sale.Customer?.PhoneNumber))
                        c.Item().Text($"Phone: {sale.Customer.PhoneNumber}").FontSize(9);
                });
            });

            column.Item().PaddingVertical(15);

            // Itemized Table
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(30);
                    columns.RelativeColumn(4);
                    columns.ConstantColumn(60);
                    columns.ConstantColumn(80);
                    columns.ConstantColumn(90);
                });

                // Header Row
                table.Header(header =>
                {
                    header.Cell().Background(Colors.Blue.Darken2).Padding(6).Text("#").FontColor(Colors.White).Bold();
                    header.Cell().Background(Colors.Blue.Darken2).Padding(6).Text("Item & Description").FontColor(Colors.White).Bold();
                    header.Cell().Background(Colors.Blue.Darken2).Padding(6).AlignRight().Text("Qty").FontColor(Colors.White).Bold();
                    header.Cell().Background(Colors.Blue.Darken2).Padding(6).AlignRight().Text("Unit Price").FontColor(Colors.White).Bold();
                    header.Cell().Background(Colors.Blue.Darken2).Padding(6).AlignRight().Text("Total Amount").FontColor(Colors.White).Bold();
                });

                // Rows
                int index = 1;
                foreach (var item in sale.SaleItems)
                {
                    var bg = index % 2 == 0 ? Colors.Grey.Lighten5 : Colors.White;

                    table.Cell().Background(bg).Padding(6).Text(index.ToString());
                    table.Cell().Background(bg).Padding(6).Text(item.Product?.Name ?? "Item");
                    table.Cell().Background(bg).Padding(6).AlignRight().Text(item.Quantity.ToString("N0"));
                    table.Cell().Background(bg).Padding(6).AlignRight().Text($"${item.UnitPrice:N2}");
                    table.Cell().Background(bg).Padding(6).AlignRight().Text($"${item.TotalPrice:N2}");

                    index++;
                }
            });

            column.Item().PaddingVertical(15);

            // Totals Summary Box
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    if (!string.IsNullOrWhiteSpace(sale.Notes))
                    {
                        c.Item().Text("NOTES / REMARKS:").FontSize(8).Bold().FontColor(Colors.Grey.Medium);
                        c.Item().Text(sale.Notes).FontSize(9).Italic();
                    }
                });

                row.ConstantItem(220).Column(c =>
                {
                    c.Item().Row(r =>
                    {
                        r.RelativeItem().Text("Subtotal:").FontSize(9);
                        r.ConstantItem(90).AlignRight().Text($"${sale.SubTotal:N2}").FontSize(9);
                    });

                    if (sale.DiscountAmount > 0)
                    {
                        c.Item().Row(r =>
                        {
                            r.RelativeItem().Text("Discount:").FontSize(9);
                            r.ConstantItem(90).AlignRight().Text($"-${sale.DiscountAmount:N2}").FontSize(9).FontColor(Colors.Red.Medium);
                        });
                    }

                    if (sale.TaxAmount > 0)
                    {
                        c.Item().Row(r =>
                        {
                            r.RelativeItem().Text($"Tax ({sale.TaxRate}%):").FontSize(9);
                            r.ConstantItem(90).AlignRight().Text($"${sale.TaxAmount:N2}").FontSize(9);
                        });
                    }

                    c.Item().PaddingVertical(4).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                    c.Item().Row(r =>
                    {
                        r.RelativeItem().Text("Total Amount:").FontSize(11).Bold();
                        r.ConstantItem(90).AlignRight().Text($"${sale.TotalAmount:N2}").FontSize(11).Bold();
                    });

                    c.Item().Row(r =>
                    {
                        r.RelativeItem().Text("Paid Amount:").FontSize(9);
                        r.ConstantItem(90).AlignRight().Text($"${sale.PaidAmount:N2}").FontSize(9);
                    });

                    c.Item().Row(r =>
                    {
                        r.RelativeItem().Text("Balance Due:").FontSize(10).Bold();
                        r.ConstantItem(90).AlignRight().Text($"${sale.DueAmount:N2}").FontSize(10).Bold().FontColor(sale.DueAmount > 0 ? Colors.Red.Medium : Colors.Green.Medium);
                    });
                });
            });
        });
    }

    private static void ComposeFooter(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
            column.Item().PaddingTop(6).Row(row =>
            {
                row.RelativeItem().Text("Thank you for your business! Official computer-generated document.")
                    .FontSize(8)
                    .FontColor(Colors.Grey.Medium);

                row.RelativeItem().AlignRight().Text(x =>
                {
                    x.Span("Page ").FontSize(8).FontColor(Colors.Grey.Medium);
                    x.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Medium);
                    x.Span(" of ").FontSize(8).FontColor(Colors.Grey.Medium);
                    x.TotalPages().FontSize(8).FontColor(Colors.Grey.Medium);
                });
            });
        });
    }
}
