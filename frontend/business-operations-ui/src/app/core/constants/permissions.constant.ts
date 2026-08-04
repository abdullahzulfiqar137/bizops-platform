export const Permissions = {
  Users: {
    View: 'Permissions.Users.View',
    Create: 'Permissions.Users.Create',
    Edit: 'Permissions.Users.Edit',
    Delete: 'Permissions.Users.Delete',
    ManageRoles: 'Permissions.Users.ManageRoles'
  },
  Roles: {
    View: 'Permissions.Roles.View',
    Manage: 'Permissions.Roles.Manage'
  },
  Customers: {
    View: 'Permissions.Customers.View',
    Create: 'Permissions.Customers.Create',
    Edit: 'Permissions.Customers.Edit',
    Delete: 'Permissions.Customers.Delete'
  },
  Suppliers: {
    View: 'Permissions.Suppliers.View',
    Create: 'Permissions.Suppliers.Create',
    Edit: 'Permissions.Suppliers.Edit',
    Delete: 'Permissions.Suppliers.Delete'
  },
  Products: {
    View: 'Permissions.Products.View',
    Create: 'Permissions.Products.Create',
    Edit: 'Permissions.Products.Edit',
    Delete: 'Permissions.Products.Delete'
  },
  Inventory: {
    View: 'Permissions.Inventory.View',
    Adjust: 'Permissions.Inventory.Adjust'
  },
  Sales: {
    View: 'Permissions.Sales.View',
    Create: 'Permissions.Sales.Create',
    Cancel: 'Permissions.Sales.Cancel'
  },
  Purchases: {
    View: 'Permissions.Purchases.View',
    Create: 'Permissions.Purchases.Create',
    Cancel: 'Permissions.Purchases.Cancel'
  },
  Expenses: {
    View: 'Permissions.Expenses.View',
    Create: 'Permissions.Expenses.Create',
    Edit: 'Permissions.Expenses.Edit',
    Delete: 'Permissions.Expenses.Delete'
  },
  Reports: {
    View: 'Permissions.Reports.View',
    Export: 'Permissions.Reports.Export'
  },
  AI: {
    ViewInsights: 'Permissions.AI.ViewInsights'
  }
} as const;
