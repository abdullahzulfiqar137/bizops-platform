namespace BusinessOperations.API.Configurations;

public sealed class EmailSettings
{
    public const string SectionName = "Email";

    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromAddress { get; set; } = string.Empty;
    public string FromName { get; set; } = "BizOps";
    public bool EnableSsl { get; set; } = true;
    public string FrontendBaseUrl { get; set; } = "http://localhost:4200";
}
