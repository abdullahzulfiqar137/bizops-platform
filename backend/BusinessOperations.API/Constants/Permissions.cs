namespace BusinessOperations.API.Constants;

public static class Permissions
{
    public static class Users
    {
        public const string View = "Permissions.Users.View";
        public const string Create = "Permissions.Users.Create";
        public const string Edit = "Permissions.Users.Edit";
        public const string Delete = "Permissions.Users.Delete";
        public const string ManageRoles = "Permissions.Users.ManageRoles";
    }

    public static class Roles
    {
        public const string View = "Permissions.Roles.View";
        public const string Manage = "Permissions.Roles.Manage";
    }

    public static class Customers
    {
        public const string View = "Permissions.Customers.View";
        public const string Create = "Permissions.Customers.Create";
        public const string Edit = "Permissions.Customers.Edit";
        public const string Delete = "Permissions.Customers.Delete";
    }

    public static class Suppliers
    {
        public const string View = "Permissions.Suppliers.View";
        public const string Create = "Permissions.Suppliers.Create";
        public const string Edit = "Permissions.Suppliers.Edit";
        public const string Delete = "Permissions.Suppliers.Delete";
    }

    public static class Products
    {
        public const string View = "Permissions.Products.View";
        public const string Create = "Permissions.Products.Create";
        public const string Edit = "Permissions.Products.Edit";
        public const string Delete = "Permissions.Products.Delete";
    }

    public static class Inventory
    {
        public const string View = "Permissions.Inventory.View";
        public const string Adjust = "Permissions.Inventory.Adjust";
    }

    public static class Sales
    {
        public const string View = "Permissions.Sales.View";
        public const string Create = "Permissions.Sales.Create";
        public const string Cancel = "Permissions.Sales.Cancel";
    }

    public static class Purchases
    {
        public const string View = "Permissions.Purchases.View";
        public const string Create = "Permissions.Purchases.Create";
        public const string Cancel = "Permissions.Purchases.Cancel";
    }

    public static class Expenses
    {
        public const string View = "Permissions.Expenses.View";
        public const string Create = "Permissions.Expenses.Create";
        public const string Edit = "Permissions.Expenses.Edit";
        public const string Delete = "Permissions.Expenses.Delete";
    }

    public static class Reports
    {
        public const string View = "Permissions.Reports.View";
        public const string Export = "Permissions.Reports.Export";
    }

    public static class AI
    {
        public const string ViewInsights = "Permissions.AI.ViewInsights";
    }
}
