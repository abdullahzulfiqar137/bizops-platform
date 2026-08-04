using BusinessOperations.API.Entities.Sales;

namespace BusinessOperations.API.Services.Pdf;

public interface IPdfDocumentService
{
    byte[] GenerateSaleInvoicePdf(Sale sale);
}
