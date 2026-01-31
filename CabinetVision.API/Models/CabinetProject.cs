namespace CabinetVision.API.Models
{
    public class CabinetProject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Status { get; set; } = "Active";
        public List<CabinetItem> Cabinets { get; set; } = new();
        public ProjectSettings Settings { get; set; } = new();
    }

    public class CabinetItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public double Width { get; set; }
        public double Height { get; set; }
        public double Depth { get; set; }
        public string Material { get; set; } = string.Empty;
        public List<HardwareComponent> Hardware { get; set; } = new();
        public decimal Price { get; set; }
    }

    public class HardwareComponent
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string PartNumber { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class ProjectSettings
    {
        public string MeasurementUnit { get; set; } = "Inches";
        public decimal TaxRate { get; set; } = 0.08m;
        public decimal LaborRate { get; set; } = 75.00m;
        public string Currency { get; set; } = "USD";
        public bool IncludeHardware { get; set; } = true;
        public bool IncludeInstallation { get; set; } = false;
    }
}
