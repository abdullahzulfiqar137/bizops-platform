namespace BusinessOperations.API.DTOs.Imports;

public sealed class CsvImportResult
{
    public int TotalRows { get; set; }
    public int ImportedRows { get; set; }
    public int FailedRows => Errors.Count;
    public List<CsvImportError> Errors { get; set; } = [];
}
public sealed class CsvImportError { public int RowNumber { get; set; } public string Message { get; set; } = string.Empty; }
