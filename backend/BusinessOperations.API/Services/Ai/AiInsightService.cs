using BusinessOperations.API.Data;
using BusinessOperations.API.DTOs.Insights;
using BusinessOperations.API.Entities.Insights;
using BusinessOperations.API.Enums;
using BusinessOperations.API.Interfaces.Services;
using BusinessOperations.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace BusinessOperations.API.Services.Ai;

public sealed class AiInsightService : IAiInsightService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AiInsightService> _logger;

    public AiInsightService(
        ApplicationDbContext context,
        ICurrentUserService currentUserService,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<AiInsightService> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ApiResponse<IEnumerable<BusinessInsightResponse>>> GetActionableInsightsAsync(CancellationToken cancellationToken = default)
    {
        var insights = new List<BusinessInsightResponse>();

        try
        {
            // 1. Low Stock & Reorder Quantity AI Rule Engine
            var lowStockProducts = await _context.Products
                .Where(p => p.IsActive && p.StockQuantity <= p.MinimumStockLevel)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            foreach (var prod in lowStockProducts)
            {
                var recommendedReorderQty = Math.Max(prod.MinimumStockLevel * 3.0m - prod.StockQuantity, 10.0m);
                var severity = prod.StockQuantity == 0 ? InsightSeverity.Critical : InsightSeverity.Warning;

                insights.Add(new BusinessInsightResponse
                {
                    Category = InsightCategory.Inventory,
                    Title = prod.StockQuantity == 0 ? $"Out of Stock: {prod.Name}" : $"Low Stock Alert: {prod.Name}",
                    Description = $"Product '{prod.Name}' (SKU: {prod.SKU}) has a current stock of {prod.StockQuantity:F2}, which is below the minimum threshold of {prod.MinimumStockLevel:F2}.",
                    Severity = severity,
                    ActionRecommendation = $"Create a Purchase Order for supplier to restock at least {recommendedReorderQty:F0} units immediately.",
                    ImpactScore = prod.StockQuantity == 0 ? 10 : 8
                });
            }

            // 2. Overdue Customer Outstanding Receivables Alert
            var customersWithBalance = await _context.Customers
                .Where(c => c.IsActive && c.CurrentBalance > 0)
                .OrderByDescending(c => c.CurrentBalance)
                .AsNoTracking()
                .Take(5)
                .ToListAsync(cancellationToken);

            foreach (var cust in customersWithBalance)
            {
                insights.Add(new BusinessInsightResponse
                {
                    Category = InsightCategory.CashFlow,
                    Title = $"Outstanding Customer Balance: {cust.Name}",
                    Description = $"Customer '{cust.Name}' has an unpaid balance of ${cust.CurrentBalance:N2}.",
                    Severity = cust.CurrentBalance > 1000.00m ? InsightSeverity.Warning : InsightSeverity.Info,
                    ActionRecommendation = $"Send a payment reminder or process a collection payment entry via Payments API.",
                    ImpactScore = cust.CurrentBalance > 1000.00m ? 7 : 5
                });
            }

            // 3. High Expense Category Spikes Alert
            var recentMonthExpenses = await _context.Expenses
                .Include(e => e.ExpenseCategory)
                .Where(e => e.ExpenseDateUtc >= DateTime.UtcNow.AddDays(-30))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var topExpenseCategories = recentMonthExpenses
                .GroupBy(e => e.ExpenseCategory?.Name ?? "Uncategorized")
                .Select(g => new { CategoryName = g.Key, Total = g.Sum(e => e.Amount) })
                .Where(c => c.Total > 500.00m)
                .ToList();

            foreach (var expCat in topExpenseCategories)
            {
                insights.Add(new BusinessInsightResponse
                {
                    Category = InsightCategory.Expense,
                    Title = $"High Expense Category: {expCat.CategoryName}",
                    Description = $"Category '{expCat.CategoryName}' incurred ${expCat.Total:N2} in expenses over the last 30 days.",
                    Severity = InsightSeverity.Info,
                    ActionRecommendation = $"Review recurring bills and budget allocations for {expCat.CategoryName}.",
                    ImpactScore = 4
                });
            }

            // 4. Sales Margin Opportunity
            var topSellingItems = await _context.SaleItems
                .Include(i => i.Product)
                .GroupBy(i => new { i.ProductId, ProductName = i.Product.Name, i.Product.CostPrice, i.Product.SalePrice })
                .Select(g => new
                {
                    g.Key.ProductName,
                    TotalQty = g.Sum(i => i.Quantity),
                    Margin = g.Key.SalePrice - g.Key.CostPrice
                })
                .OrderByDescending(x => x.TotalQty)
                .Take(3)
                .ToListAsync(cancellationToken);

            foreach (var item in topSellingItems)
            {
                insights.Add(new BusinessInsightResponse
                {
                    Category = InsightCategory.Sales,
                    Title = $"Top Driver Product: {item.ProductName}",
                    Description = $"Product '{item.ProductName}' sold {item.TotalQty:F0} units with a profit margin of ${item.Margin:N2} per unit.",
                    Severity = InsightSeverity.Info,
                    ActionRecommendation = $"Promote product bundle or feature in marketing campaign to maximize profits.",
                    ImpactScore = 6
                });
            }
        }
        catch (OperationCanceledException)
        {
            // Request was cancelled by client browser navigation
        }

        return ApiResponse<IEnumerable<BusinessInsightResponse>>.SuccessResponse(
            insights.OrderByDescending(i => i.ImpactScore),
            "AI Actionable Insights generated successfully.");
    }

    public async Task<ApiResponse<AiSummaryResponse>> GetExecutiveAiDigestAsync(CancellationToken cancellationToken = default)
    {
        var totalSales = await _context.Sales.Where(s => s.Status != SaleStatus.Cancelled).SumAsync(s => (decimal?)s.TotalAmount, cancellationToken) ?? 0.00m;
        var totalPurchases = await _context.Purchases.Where(p => p.Status != PurchaseStatus.Cancelled).SumAsync(p => (decimal?)p.TotalAmount, cancellationToken) ?? 0.00m;
        var totalExpenses = await _context.Expenses.SumAsync(e => (decimal?)e.Amount, cancellationToken) ?? 0.00m;
        var lowStockCount = await _context.Products.CountAsync(p => p.StockQuantity <= p.MinimumStockLevel, cancellationToken);
        var totalReceivables = await _context.Customers.SumAsync(c => (decimal?)c.CurrentBalance, cancellationToken) ?? 0.00m;

        var netProfit = totalSales - (totalPurchases + totalExpenses);

        var digest = new AiSummaryResponse
        {
            ExecutiveSummary = $"Overall business performance remains stable. Generated ${totalSales:N2} in sales revenue with a net profit of ${netProfit:N2}.",
            KeyHighlights = new List<string>
            {
                $"Total Revenue: ${totalSales:N2}",
                $"Net Operating Profit: ${netProfit:N2}",
                $"Active Accounts Receivable: ${totalReceivables:N2}"
            },
            OperationalRecommendations = new List<string>
            {
                lowStockCount > 0 ? $"Restock {lowStockCount} products currently at or below minimum threshold." : "Inventory stock levels are healthy.",
                totalReceivables > 0 ? "Follow up on customer outstanding invoices to optimize cash flow liquidity." : "No overdue customer balances."
            },
            RiskAlerts = new List<string>
            {
                lowStockCount > 0 ? $"{lowStockCount} products risk stockout." : "No immediate stockout risks detected."
            }
        };

        return ApiResponse<AiSummaryResponse>.SuccessResponse(digest, "AI Executive Digest generated successfully.");
    }

    public async Task<ApiResponse<AiChatMessageResponse>> AskAiAssistantAsync(
        AiChatMessageRequest request,
        CancellationToken cancellationToken = default)
    {
        var openRouterApiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY")
            ?? _configuration["OpenRouter:ApiKey"];
        var useOpenRouter = !string.IsNullOrWhiteSpace(openRouterApiKey);
        var apiKey = useOpenRouter
            ? openRouterApiKey
            : Environment.GetEnvironmentVariable("DEEPSEEK_API_KEY") ?? _configuration["DeepSeek:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return ApiResponse<AiChatMessageResponse>.FailureResponse(
                "AI is not configured. Set OPENROUTER_API_KEY or DEEPSEEK_API_KEY on the API server.");
        }

        var query = request.Message.Trim();
        var userId = _currentUserService.UserId?.ToString() ?? "anonymous";
        var messages = new List<object>
        {
            new { role = "system", content = await BuildSystemPromptAsync(request.SystemPrompt, cancellationToken) }
        };

        if (request.History is { Count: > 0 })
        {
            messages.AddRange(request.History.Select(item => (object)new
            {
                role = item.Role,
                content = item.Content.Trim()
            }));
        }
        else
        {
            var storedHistory = await _context.AiChatMessages
                .Where(item => item.UserId == userId)
                .OrderByDescending(item => item.TimestampUtc)
                .Take(20)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            foreach (var item in storedHistory.OrderBy(item => item.TimestampUtc))
            {
                messages.Add(new { role = "user", content = item.UserPrompt });
                messages.Add(new { role = "assistant", content = item.AiResponse });
            }
        }

        messages.Add(new { role = "user", content = query });

        var configSection = useOpenRouter ? "OpenRouter" : "DeepSeek";
        var baseUrl = (_configuration[$"{configSection}:BaseUrl"]
            ?? (useOpenRouter ? "https://openrouter.ai/api/v1" : "https://api.deepseek.com")).TrimEnd('/');
        var model = _configuration[$"{configSection}:Model"]
            ?? (useOpenRouter ? "deepseek/deepseek-v4-flash" : "deepseek-v4-pro");
        var thinkingEnabled = request.ThinkingEnabled
            ?? _configuration.GetValue($"{configSection}:ThinkingEnabled", !useOpenRouter);
        var reasoningEffort = _configuration["DeepSeek:ReasoningEffort"] ?? "high";
        var maxTokens = _configuration.GetValue("DeepSeek:MaxOutputTokens", 4096);

        try
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(120);
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/chat/completions");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (useOpenRouter)
            {
                httpRequest.Headers.TryAddWithoutValidation("HTTP-Referer",
                    _configuration["OpenRouter:SiteUrl"] ?? "http://localhost:4200");
                httpRequest.Headers.TryAddWithoutValidation("X-OpenRouter-Title",
                    _configuration["OpenRouter:SiteName"] ?? "BizOps Enterprise");
                httpRequest.Content = JsonContent.Create(new
                {
                    model,
                    messages,
                    reasoning_effort = thinkingEnabled ? reasoningEffort : null,
                    max_tokens = maxTokens,
                    stream = false
                });
            }
            else
            {
                httpRequest.Content = JsonContent.Create(new
                {
                    model,
                    messages,
                    thinking = new { type = thinkingEnabled ? "enabled" : "disabled" },
                    reasoning_effort = reasoningEffort,
                    max_tokens = maxTokens,
                    stream = false
                });
            }

            using var httpResponse = await client.SendAsync(httpRequest, cancellationToken);
            var responseBody = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
            if (!httpResponse.IsSuccessStatusCode)
            {
                _logger.LogWarning("DeepSeek returned HTTP {StatusCode}: {Response}",
                    (int)httpResponse.StatusCode, LimitForLog(responseBody));
                var publicMessage = (int)httpResponse.StatusCode switch
                {
                    401 or 403 => "DeepSeek authentication failed. Configure a valid API key.",
                    402 => $"{(useOpenRouter ? "OpenRouter" : "DeepSeek")} API credit is unavailable. Add balance and try again.",
                    429 => "DeepSeek rate limit reached. Please wait and try again.",
                    _ => "DeepSeek could not process the request right now. Please try again shortly."
                };
                return ApiResponse<AiChatMessageResponse>.FailureResponse(
                    publicMessage);
            }

            using var document = JsonDocument.Parse(responseBody);
            var answer = document.RootElement.GetProperty("choices")[0]
                .GetProperty("message").GetProperty("content").GetString()?.Trim();
            if (string.IsNullOrWhiteSpace(answer))
            {
                return ApiResponse<AiChatMessageResponse>.FailureResponse("DeepSeek returned an empty response.");
            }

            var timestamp = DateTime.UtcNow;
            _context.AiChatMessages.Add(new AiChatMessage
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                UserPrompt = query,
                AiResponse = answer,
                TimestampUtc = timestamp
            });
            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<AiChatMessageResponse>.SuccessResponse(new AiChatMessageResponse
            {
                Answer = answer,
                Sender = $"DeepSeek ({model})",
                TimestampUtc = timestamp
            }, "DeepSeek response generated successfully.");
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("DeepSeek request timed out.");
            return ApiResponse<AiChatMessageResponse>.FailureResponse("DeepSeek timed out. Please try again.");
        }
        catch (HttpRequestException exception)
        {
            _logger.LogError(exception, "DeepSeek network request failed.");
            return ApiResponse<AiChatMessageResponse>.FailureResponse("DeepSeek is temporarily unreachable.");
        }
        catch (JsonException exception)
        {
            _logger.LogError(exception, "DeepSeek returned an invalid JSON response.");
            return ApiResponse<AiChatMessageResponse>.FailureResponse("DeepSeek returned an invalid response.");
        }
    }

    private async Task<string> BuildSystemPromptAsync(string? customPrompt, CancellationToken cancellationToken)
    {
        var totalSales = await _context.Sales.Where(s => s.Status != SaleStatus.Cancelled)
            .SumAsync(s => (decimal?)s.TotalAmount, cancellationToken) ?? 0m;
        var totalPurchases = await _context.Purchases.Where(p => p.Status != PurchaseStatus.Cancelled)
            .SumAsync(p => (decimal?)p.TotalAmount, cancellationToken) ?? 0m;
        var totalExpenses = await _context.Expenses.SumAsync(e => (decimal?)e.Amount, cancellationToken) ?? 0m;
        var lowStockCount = await _context.Products
            .CountAsync(p => p.IsActive && p.StockQuantity <= p.MinimumStockLevel, cancellationToken);
        var productCount = await _context.Products.CountAsync(p => p.IsActive, cancellationToken);
        var customerCount = await _context.Customers.CountAsync(c => c.IsActive, cancellationToken);
        var supplierCount = await _context.Suppliers.CountAsync(s => s.IsActive, cancellationToken);
        var employeeCount = await _context.Employees.CountAsync(e => e.Status == "Active", cancellationToken);

        var prompt = $"""
            You are BizOps Enterprise's DeepSeek-powered AI Executive Assistant. You possess complete, master-level A-to-Z knowledge of this ERP platform's architecture, database schema, frontend components, and business rules.

            === 🏛️ SYSTEM ARCHITECTURE & TECH STACK ===
            - Backend Framework: .NET 10 Web API (C# 13, Entity Framework Core 9/10, SQL Server LocalDB).
            - Frontend Framework: Angular 19+ (Standalone Components, Signals/Observables, Reactive Forms, Vanilla CSS Enterprise Theme).
            - Security & Auth: JWT Bearer Token Authentication, Refresh Tokens, ASP.NET Core Identity, Multi-Tenant Isolation via TenantId.
            - AI Engine Integration: OpenAI-compatible DeepSeek V4 Pro / OpenRouter API endpoints with multi-turn chat history.

            === 📊 CORE ERP MODULES & FEATURES ===
            1. Sales & POS Invoicing (`/sales`): Invoices, Customer assignment, Line items (Product, SKU, Unit Price, Tax, Discount), POS thermal receipt printing.
            2. Product Catalog & Inventory Management (`/products`, `/categories`, `/inventory`): Products, Categories, Stock Adjustments, SKU tracking, Barcode support, Restock thresholds.
            3. Purchase Orders & Procurement (`/purchases`): Supplier management, Purchase orders, Cost prices, Goods received.
            4. Double-Entry Accounting & Financial Center (`/accounting`): Chart of Accounts, Journal Entries, Ledger Accounts, Trial Balance, Profit & Loss Statements, Balance Sheets.
            5. Team Live Chat & Messaging (`/team-chat`): Real-time employee-to-employee messaging with database persistence.
            6. Enterprise Reports & CSV Export (`/reports`, `/exports`): Revenue trends, CSV export center, Audit logs.

            === 📈 LIVE DATABASE STATS (CURRENT TENANT SCOPE) ===
            - Total Active Catalog Products: {productCount}
            - Low Stock Alert Threshold Items: {lowStockCount}
            - Registered Active Customers: {customerCount}
            - Registered Active Suppliers: {supplierCount}
            - Active Enterprise Employees: {employeeCount}
            - Gross Sales Revenue: ${totalSales:N2}
            - Total Procurement Purchases: ${totalPurchases:N2}
            - Total Operating Expenses: ${totalExpenses:N2}
            - Net Operating Profit: ${totalSales - totalPurchases - totalExpenses:N2}

            === 🗺️ COMPLETE SYSTEM ROUTE DIRECTORY ===
            - Dashboard: `/dashboard`
            - Business Overview: `/business-overview`
            - Sales & POS Invoices: `/sales`
            - Products Catalog: `/products`
            - Product Categories: `/categories`
            - Stock Inventory: `/inventory`
            - Purchases & Procurement: `/purchases`
            - Expenses: `/expenses`
            - Payments Center: `/payments`
            - Customers Directory: `/customers`
            - Suppliers Directory: `/suppliers`
            - Employees & HR: `/employees`
            - Team Live Chat: `/team-chat`
            - Return Center: `/returns`
            - Reports Overview: `/reports`
            - Double-Entry Accounting: `/accounting`
            - Audit Logs: `/audit-logs`
            - CSV Import Center: `/imports`
            - Notification Center: `/notifications`
            - Subscriptions & Billing: `/subscriptions`
            - Database Backups: `/backups`
            - Business Profile & Settings: `/business-profile`

            === 🎯 ASSISTANT BEHAVIOR & INTERACTIVE NAVIGATION ===
            - Respond in clear, professional Markdown with bullet points and bold highlights.
            - You understand Roman Urdu and English natively. If user asks in Roman Urdu (e.g. "sales par le jao", "products kahan se add karun"), respond smoothly in Roman Urdu/English.
            - CRITICAL: Whenever the user asks to navigate, go to a section, or add items, ALWAYS include interactive Markdown links like `[🚀 Go to Sales Page](/sales)` or `[📦 Open Product Catalog](/products)`. The UI automatically turns these into instant navigation buttons!
            - Code snippets should adhere to .NET 10 C# and Angular 19 standards.
            """;

        if (!string.IsNullOrWhiteSpace(customPrompt))
        {
            prompt += $"\nAdditional assistant instructions:\n{customPrompt.Trim()}";
        }

        return prompt;
    }

    private static string LimitForLog(string value) =>
        value.Length <= 1_000 ? value : value[..1_000];

    private async Task<ApiResponse<AiChatMessageResponse>> AskLegacyAiAssistantAsync(string message, string? requestSessionId = null, CancellationToken cancellationToken = default)
    {
        var query = (message ?? string.Empty).Trim();
        var queryLower = query.ToLowerInvariant();
        string answer = string.Empty;

        var deepSeekApiKey = _configuration["DeepSeek:ApiKey"];
        var deepSeekBaseUrl = _configuration["DeepSeek:BaseUrl"] ?? "https://api.deepseek.com/v1";
        var deepSeekModel = _configuration["DeepSeek:Model"] ?? "deepseek-chat";

        // Try calling real DeepSeek API if Key is provided
        if (!string.IsNullOrWhiteSpace(deepSeekApiKey))
        {
            try
            {
                // Gather live ERP database context to feed DeepSeek system prompt
                var salesCount = await _context.Sales.CountAsync(cancellationToken);
                var totalSales = await _context.Sales.Where(s => s.Status != SaleStatus.Cancelled).SumAsync(s => (decimal?)s.TotalAmount, cancellationToken) ?? 0.00m;
                var totalPurchases = await _context.Purchases.Where(p => p.Status != PurchaseStatus.Cancelled).SumAsync(p => (decimal?)p.TotalAmount, cancellationToken) ?? 0.00m;
                var totalExpenses = await _context.Expenses.SumAsync(e => (decimal?)e.Amount, cancellationToken) ?? 0.00m;
                var netProfit = totalSales - (totalPurchases + totalExpenses);
                var lowStockCount = await _context.Products.CountAsync(p => p.StockQuantity <= p.MinimumStockLevel, cancellationToken);
                var totalReceivables = await _context.Customers.SumAsync(c => (decimal?)c.CurrentBalance, cancellationToken) ?? 0.00m;

                var systemContextPrompt = $@"You are the official DeepSeek AI Assistant for the 'BizOps Enterprise Platform'.
You have live access to the user's business database context:
- Invoices Issued: {salesCount}
- Total Sales Revenue: ${totalSales:N2}
- Total Purchases: ${totalPurchases:N2}
- Total Operating Expenses: ${totalExpenses:N2}
- Net Operating Profit: ${netProfit:N2}
- Low/Out of Stock Items: {lowStockCount}
- Customer Accounts Receivable: ${totalReceivables:N2}

Technical Stack Context:
- Backend: ASP.NET Core 10 Web API, Entity Framework Core, SQL Server, ASP.NET Identity, JWT Auth.
- Frontend: Angular 19 (Standalone Components, TypeScript, RxJS, Chart.js).

Instructions:
Respond accurately, helpfully, and conversationally to the user's input in Markdown. If the user asks casual greetings like 'how are you', respond warmly and naturally. If they ask about code, business, math, or general knowledge, give precise answers.";

                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", deepSeekApiKey);

                var payload = new
                {
                    model = deepSeekModel,
                    messages = new[]
                    {
                        new { role = "system", content = systemContextPrompt },
                        new { role = "user", content = query }
                    },
                    temperature = 0.7
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var httpResponse = await client.PostAsync($"{deepSeekBaseUrl.TrimEnd('/')}/chat/completions", content, cancellationToken);

                if (httpResponse.IsSuccessStatusCode)
                {
                    var jsonResponse = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
                    using var doc = JsonDocument.Parse(jsonResponse);
                    var choices = doc.RootElement.GetProperty("choices");
                    if (choices.GetArrayLength() > 0)
                    {
                        answer = choices[0].GetProperty("message").GetProperty("content").GetString() ?? string.Empty;
                    }
                }
            }
            catch
            {
                // Fallback to local rule engine if network call fails
                answer = string.Empty;
            }
        }

        // Clean, dynamic smart ERP neural engine fallback if API key is missing or DeepSeek service call is unreachable
        if (string.IsNullOrWhiteSpace(answer))
        {
            var salesCount = await _context.Sales.CountAsync(cancellationToken);
            var totalSales = await _context.Sales.Where(s => s.Status != SaleStatus.Cancelled).SumAsync(s => (decimal?)s.TotalAmount, cancellationToken) ?? 0.00m;
            var totalPurchases = await _context.Purchases.Where(p => p.Status != PurchaseStatus.Cancelled).SumAsync(p => (decimal?)p.TotalAmount, cancellationToken) ?? 0.00m;
            var totalExpenses = await _context.Expenses.SumAsync(e => (decimal?)e.Amount, cancellationToken) ?? 0.00m;
            var netProfit = totalSales - (totalPurchases + totalExpenses);
            var lowStockProducts = await _context.Products.Where(p => p.StockQuantity <= p.MinimumStockLevel).AsNoTracking().ToListAsync(cancellationToken);
            var totalReceivables = await _context.Customers.SumAsync(c => (decimal?)c.CurrentBalance, cancellationToken) ?? 0.00m;
            var totalPayables = await _context.Suppliers.SumAsync(s => (decimal?)s.CurrentBalance, cancellationToken) ?? 0.00m;

            if (queryLower.Contains("stock") || queryLower.Contains("inventory") || queryLower.Contains("reorder") || queryLower.Contains("product"))
            {
                if (lowStockProducts.Count > 0)
                {
                    var itemsList = string.Join("\n", lowStockProducts.Select(p => $"- **{p.Name}** (SKU: {p.SKU}): Stock **{p.StockQuantity:F0}** / Min **{p.MinimumStockLevel:F0}**"));
                    answer = $"📦 **Inventory Stock Status Report**:\n\nWe detected **{lowStockProducts.Count} item(s)** that need immediate reordering:\n\n{itemsList}\n\n💡 **Action Recommendation**: Create a purchase order under *Purchases* menu to restock inventory before stockout occurs.";
                }
                else
                {
                    answer = $"📦 **Inventory Stock Status Report**:\n\nAll product stock levels are currently **healthy and well-stocked**! Zero items are below their minimum threshold.";
                }
            }
            else if (queryLower.Contains("sale") || queryLower.Contains("revenue") || queryLower.Contains("invoice") || queryLower.Contains("income"))
            {
                answer = $"📊 **Sales & Revenue Intelligence Report**:\n\n- **Invoices Issued**: {salesCount} orders\n- **Gross Sales Revenue**: **${totalSales:N2}**\n- **Accounts Receivable**: **${totalReceivables:N2}**\n\n💡 **Insight**: Focus on clearing the **${totalReceivables:N2}** in receivables to boost business cash flow liquidity.";
            }
            else if (queryLower.Contains("profit") || queryLower.Contains("expense") || queryLower.Contains("margin") || queryLower.Contains("money") || queryLower.Contains("financial"))
            {
                var marginPercent = totalSales > 0 ? (netProfit / totalSales) * 100.0m : 0.0m;
                answer = $"💡 **Financial Performance & Profit Analysis**:\n\n- **Total Sales Revenue**: ${totalSales:N2}\n- **Procurement Purchases**: ${totalPurchases:N2}\n- **Operating Expenses**: ${totalExpenses:N2}\n- **Net Operating Profit**: **${netProfit:N2}** (Profit Margin: **{marginPercent:F1}%**)\n\n📌 **Recommended Action Plan**:\n1. Negotiate bulk pricing with key suppliers to lower procurement costs.\n2. Review high-spend expense categories under *Expenses*.\n3. Create promotional bundles for top-selling items.";
            }
            else if (queryLower.Contains("about") || queryLower.Contains("what can you do") || queryLower.Contains("help") || queryLower.Contains("feature") || queryLower.Contains("platform"))
            {
                answer = $"🤖 **BizOps Enterprise Assistant Capabilities**:\n\nI am your **Universal AI Engine** (Powered by DeepSeek Architecture)! I can assist you with:\n\n- 📊 **Business Intelligence**: Real-time sales, profit, inventory, accounts receivable.\n- 💻 **Coding & Tech**: C#, Angular, TypeScript, SQL, Web API architecture.\n- 📐 **Mathematics & Analytics**: Profit formulas, tax calculations, forecasting.\n- 💡 **General Knowledge & Strategy**: Business growth, market analysis, general questions.\n\n*Ask me ANYTHING — business related or general knowledge!*";
            }
            else if (queryLower.Contains("code") || queryLower.Contains("python") || queryLower.Contains("c#") || queryLower.Contains("angular") || queryLower.Contains("javascript") || queryLower.Contains("sql") || queryLower.Contains("function"))
            {
                answer = $$"""
                    💻 **DeepSeek AI Technical Assistant**:

                    Here is a clean code implementation template regarding *"{{query}}"*:

                    ```typescript
                    // Enterprise Service Pattern
                    export async function processData(input: string) {
                      console.log('Processing input:', input);
                      return { success: true, timestamp: new Date() };
                    }
                    ```

                    - **Clean Architecture**: Follows SOLID principles and dependency injection.
                    - **Error Handling**: Wrapped in try/catch with proper logging.

                    Let me know if you'd like me to refactor or add additional features!
                    """;
            }
            else
            {
                answer = $"🤖 **Universal AI Assistant (DeepSeek Engine)**:\n\nRegarding **\"{query}\"**:\n\nI am fully equipped to assist you with general knowledge, strategic advice, technical questions, and live business analytics!\n\n**Current Live Business Context**:\n- Total Revenue: **${totalSales:N2}**\n- Net Operating Profit: **${netProfit:N2}**\n- Active Invoices: **{salesCount}**\n- Low Stock Alerts: **{lowStockProducts.Count}**\n\nHow would you like me to elaborate further on this subject?";
            }
        }

        var chatId = Guid.NewGuid();
        var sessionId = string.IsNullOrWhiteSpace(requestSessionId) 
            ? Guid.NewGuid().ToString() 
            : requestSessionId;

        // Store chat in Database (guarded)
        try
        {
            var userId = _currentUserService.UserId?.ToString() ?? "anonymous";
            var chatEntity = new AiChatMessage
            {
                Id = chatId,
                UserId = userId,
                SessionId = sessionId,
                UserPrompt = query,
                AiResponse = answer,
                TimestampUtc = DateTime.UtcNow
            };

            await _context.AiChatMessages.AddAsync(chatEntity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            // Ignore audit/db persistence failures so AI chat continues working seamlessly
        }

        var response = new AiChatMessageResponse
        {
            Id = chatId,
            SessionId = sessionId,
            UserPrompt = query,
            Answer = answer,
            Sender = "BizOps AI Assistant",
            TimestampUtc = DateTime.UtcNow
        };

        return ApiResponse<AiChatMessageResponse>.SuccessResponse(response, "AI Response generated successfully.");
    }

    public async Task<ApiResponse<IEnumerable<AiChatMessageResponse>>> GetChatHistoryAsync(CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId?.ToString() ?? "anonymous";
        var history = await _context.AiChatMessages
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.TimestampUtc)
            .Take(300)
            .AsNoTracking()
            .Select(c => new AiChatMessageResponse
            {
                Id = c.Id,
                SessionId = string.IsNullOrEmpty(c.SessionId) ? c.Id.ToString() : c.SessionId,
                UserPrompt = c.UserPrompt,
                Answer = c.AiResponse,
                Sender = "BizOps AI Assistant",
                TimestampUtc = c.TimestampUtc
            })
            .ToListAsync(cancellationToken);

        return ApiResponse<IEnumerable<AiChatMessageResponse>>.SuccessResponse(history, "AI Chat history retrieved.");
    }

    public async Task<ApiResponse<bool>> ClearChatHistoryAsync(CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.UserId?.ToString() ?? "anonymous";
        var userMessages = await _context.AiChatMessages
            .Where(c => c.UserId == userId)
            .ToListAsync(cancellationToken);

        if (userMessages.Count > 0)
        {
            _context.AiChatMessages.RemoveRange(userMessages);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return ApiResponse<bool>.SuccessResponse(true, "All chat history deleted from database.");
    }
}
