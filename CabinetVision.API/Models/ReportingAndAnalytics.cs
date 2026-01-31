namespace CabinetVision.API.Models
{
    // Отчеты и аналитика
    public class Report
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Project, Inventory, Production, Financial, etc.
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? GeneratedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string Status { get; set; } = "Draft"; // Draft, Generated, Scheduled
        public ReportParameters Parameters { get; set; } = new();
        public ReportSchedule Schedule { get; set; } = new();
        public List<ReportFilter> Filters { get; set; } = new();
        public string OutputFormat { get; set; } = "PDF"; // PDF, Excel, CSV
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
    }

    public class ReportParameters
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ProjectId { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public string DesignerId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Dictionary<string, string> CustomParameters { get; set; } = new();
    }

    public class ReportSchedule
    {
        public bool IsScheduled { get; set; }
        public string Frequency { get; set; } = string.Empty; // Daily, Weekly, Monthly
        public string DayOfWeek { get; set; } = string.Empty;
        public int DayOfMonth { get; set; }
        public TimeOnly Time { get; set; }
        public List<string> Recipients { get; set; } = new();
        public DateTime NextRun { get; set; }
    }

    public class ReportFilter
    {
        public string FieldName { get; set; } = string.Empty;
        public string Operator { get; set; } = string.Empty; // Equals, Contains, GreaterThan, etc.
        public string Value { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty; // String, Number, Date
    }

    public class Dashboard
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime LastModified { get; set; }
        public bool IsDefault { get; set; }
        public bool IsPublic { get; set; }
        public List<DashboardWidget> Widgets { get; set; } = new();
        public DashboardLayout Layout { get; set; } = new();
    }

    public class DashboardWidget
    {
        public int Id { get; set; }
        public string WidgetType { get; set; } = string.Empty; // Chart, KPI, Table, etc.
        public string Title { get; set; } = string.Empty;
        public int Row { get; set; }
        public int Column { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public WidgetDataSource DataSource { get; set; } = new();
        public WidgetSettings Settings { get; set; } = new();
        public DateTime LastRefreshed { get; set; }
    }

    public class WidgetDataSource
    {
        public string Query { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public Dictionary<string, string> Parameters { get; set; } = new();
        public int RefreshInterval { get; set; } // minutes
    }

    public class WidgetSettings
    {
        public string ChartType { get; set; } = string.Empty; // Bar, Line, Pie, etc.
        public List<string> GroupBy { get; set; } = new();
        public List<string> Aggregates { get; set; } = new();
        public Dictionary<string, string> Colors { get; set; } = new();
        public bool ShowLegend { get; set; }
        public bool ShowLabels { get; set; }
    }

    public class DashboardLayout
    {
        public int Columns { get; set; }
        public int RowHeight { get; set; }
        public string Theme { get; set; } = string.Empty;
        public bool AutoRefresh { get; set; }
        public int RefreshInterval { get; set; } // minutes
    }

    public class AnalyticsData
    {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public SalesMetrics Sales { get; set; } = new();
        public ProductionMetrics Production { get; set; } = new();
        public InventoryMetrics Inventory { get; set; } = new();
        public FinancialMetrics Financial { get; set; } = new();
        public List<ProjectMetrics> Projects { get; set; } = new();
    }

    public class SalesMetrics
    {
        public int TotalProjects { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageProjectValue { get; set; }
        public decimal GrowthRate { get; set; }
        public int NewCustomers { get; set; }
        public int RepeatCustomers { get; set; }
        public List<SalesByPeriod> SalesByPeriod { get; set; } = new();
        public List<SalesByCategory> SalesByCategory { get; set; } = new();
    }

    public class SalesByPeriod
    {
        public DateTime Period { get; set; }
        public decimal Revenue { get; set; }
        public int ProjectCount { get; set; }
        public decimal AverageValue { get; set; }
    }

    public class SalesByCategory
    {
        public string Category { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int ProjectCount { get; set; }
        public decimal Percentage { get; set; }
    }

    public class ProductionMetrics
    {
        public int TotalJobs { get; set; }
        public int CompletedJobs { get; set; }
        public decimal CompletionRate { get; set; }
        public decimal AverageCompletionTime { get; set; } // days
        public decimal MaterialUtilization { get; set; } // percentage
        public decimal MachineUtilization { get; set; } // percentage
        public int QualityIssues { get; set; }
        public decimal ReworkRate { get; set; }
        public List<ProductionByMachine> ProductionByMachine { get; set; } = new();
    }

    public class ProductionByMachine
    {
        public string MachineId { get; set; } = string.Empty;
        public string MachineName { get; set; } = string.Empty;
        public int JobsCompleted { get; set; }
        public decimal Utilization { get; set; }
        public decimal Downtime { get; set; } // hours
    }

    public class InventoryMetrics
    {
        public decimal TotalInventoryValue { get; set; }
        public int MaterialItems { get; set; }
        public int HardwareItems { get; set; }
        public int LowStockItems { get; set; }
        public decimal TurnoverRate { get; set; }
        public List<InventoryByCategory> InventoryByCategory { get; set; } = new();
        public List<LowStockItem> LowStockItems { get; set; } = new();
    }

    public class InventoryByCategory
    {
        public string Category { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public int ItemCount { get; set; }
        public decimal Percentage { get; set; }
    }

    public class LowStockItem
    {
        public string ItemId { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
        public int MinStockLevel { get; set; }
        public int ReorderPoint { get; set; }
        public string Supplier { get; set; } = string.Empty;
        public int LeadTime { get; set; } // days
    }

    public class FinancialMetrics
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalCosts { get; set; }
        public decimal GrossProfit { get; set; }
        public decimal NetProfit { get; set; }
        public decimal ProfitMargin { get; set; }
        public decimal MaterialCosts { get; set; }
        public decimal LaborCosts { get; set; }
        public decimal OverheadCosts { get; set; }
        public List<ExpenseByCategory> ExpenseByCategory { get; set; } = new();
    }

    public class ExpenseByCategory
    {
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
    }

    public class ProjectMetrics
    {
        public string ProjectId { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public decimal ProjectValue { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public decimal MaterialCost { get; set; }
        public decimal LaborCost { get; set; }
        public decimal ProfitMargin { get; set; }
        public int DaysOverdue { get; set; }
    }

    public class KPI
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // Sales, Production, Financial, etc.
        public decimal CurrentValue { get; set; }
        public decimal TargetValue { get; set; }
        public decimal PreviousValue { get; set; }
        public string Unit { get; set; } = string.Empty;
        public string Trend { get; set; } = string.Empty; // Up, Down, Stable
        public decimal PercentageChange { get; set; }
        public DateTime LastUpdated { get; set; }
        public string Status { get; set; } = string.Empty; // Good, Warning, Critical
        public List<KPIDataPoint> HistoricalData { get; set; } = new();
    }

    public class KPIDataPoint
    {
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
        public string Target { get; set; } = string.Empty;
    }

    public class Alert
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Info, Warning, Error, Success
        public string Category { get; set; } = string.Empty; // Inventory, Production, Financial, etc.
        public DateTime CreatedDate { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadDate { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public string ActionUrl { get; set; } = string.Empty;
        public int Priority { get; set; } // 1-5
    }
}
