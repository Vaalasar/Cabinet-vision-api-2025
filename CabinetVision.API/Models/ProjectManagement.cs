namespace CabinetVision.API.Models
{
    // Управление проектами
    public class Project
    {
        public int Id { get; set; }
        public string ProjectNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public string DesignerId { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string Status { get; set; } = "New"; // New, InProgress, Completed, Cancelled
        public decimal TotalPrice { get; set; }
        public decimal LaborCost { get; set; }
        public decimal MaterialCost { get; set; }
        public decimal HardwareCost { get; set; }
        public string Notes { get; set; } = string.Empty;
        public List<Room> Rooms { get; set; } = new();
        public ProjectSettings Settings { get; set; } = new();
    }

    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string RoomType { get; set; } = string.Empty; // Kitchen, Bathroom, Office, etc.
        public double Width { get; set; }
        public double Height { get; set; }
        public double Depth { get; set; }
        public List<Cabinet> Cabinets { get; set; } = new();
        public List<Countertop> Countertops { get; set; } = new();
    }

    public class Cabinet
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CabinetType { get; set; } = string.Empty; // Base, Wall, Tall, Specialty
        public string Style { get; set; } = string.Empty;
        public double Width { get; set; }
        public double Height { get; set; }
        public double Depth { get; set; }
        public string MaterialId { get; set; } = string.Empty;
        public string FinishId { get; set; } = string.Empty;
        public List<Door> Doors { get; set; } = new();
        public List<Drawer> Drawers { get; set; } = new();
        public List<Shelf> Shelves { get; set; } = new();
        public List<Hardware> Hardware { get; set; } = new();
        public decimal Price { get; set; }
        public decimal Weight { get; set; }
        public AssemblyInstructions Assembly { get; set; } = new();
    }

    public class Door
    {
        public int Id { get; set; }
        public string DoorType { get; set; } = string.Empty; // Flat, Panel, Glass, etc.
        public string Style { get; set; } = string.Empty;
        public double Width { get; set; }
        public double Height { get; set; }
        public double Thickness { get; set; }
        public string MaterialId { get; set; } = string.Empty;
        public string HingeType { get; set; } = string.Empty;
        public int HingeCount { get; set; }
        public bool HasHandle { get; set; }
        public string HandleId { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class Drawer
    {
        public int Id { get; set; }
        public string DrawerType { get; set; } = string.Empty; // Standard, SoftClose, etc.
        public double Width { get; set; }
        public double Height { get; set; }
        public double Depth { get; set; }
        public string MaterialId { get; set; } = string.Empty;
        public string SlideType { get; set; } = string.Empty;
        public decimal WeightCapacity { get; set; }
        public bool HasHandle { get; set; }
        public string HandleId { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class Shelf
    {
        public int Id { get; set; }
        public double Width { get; set; }
        public double Depth { get; set; }
        public double Thickness { get; set; }
        public string MaterialId { get; set; } = string.Empty;
        public bool IsAdjustable { get; set; }
        public int Position { get; set; }
        public decimal Price { get; set; }
    }

    public class Countertop
    {
        public int Id { get; set; }
        public string MaterialId { get; set; } = string.Empty;
        public double Length { get; set; }
        public double Depth { get; set; }
        public double Thickness { get; set; }
        public string EdgeProfile { get; set; } = string.Empty;
        public bool HasBacksplash { get; set; }
        public double BacksplashHeight { get; set; }
        public List<Cutout> Cutouts { get; set; } = new();
        public decimal Price { get; set; }
    }

    public class Cutout
    {
        public int Id { get; set; }
        public string CutoutType { get; set; } = string.Empty; // Sink, Cooktop, Faucet
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Depth { get; set; }
        public string TemplateId { get; set; } = string.Empty;
    }

    public class AssemblyInstructions
    {
        public List<AssemblyStep> Steps { get; set; } = new();
        public List<string> RequiredTools { get; set; } = new();
        public int EstimatedAssemblyTime { get; set; } // minutes
        public string DifficultyLevel { get; set; } = "Easy"; // Easy, Medium, Hard
    }

    public class AssemblyStep
    {
        public int StepNumber { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public List<string> RequiredParts { get; set; } = new();
        public List<string> RequiredHardware { get; set; } = new();
    }
}
