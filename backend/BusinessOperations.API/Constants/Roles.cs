namespace BusinessOperations.API.Constants;

public static class Roles
{
    public const string Admin = "Admin";
    public const string Manager = "Manager";
    public const string Staff = "Staff";

    public static readonly IReadOnlyCollection<string> All = new[] { Admin, Manager, Staff };
}
