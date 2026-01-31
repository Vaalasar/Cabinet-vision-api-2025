namespace CabinetVision.API.Models
{
    // Производство и CNC-обработка (S2M модуль)
    public class ManufacturingJob
    {
        public int Id { get; set; }
        public string JobNumber { get; set; } = string.Empty;
        public string ProjectId { get; set; } = string.Empty;
        public string JobName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, InProgress, Completed, OnHold, Cancelled
        public string Priority { get; set; } = "Normal"; // Low, Normal, High, Urgent
        public string MachineId { get; set; } = string.Empty;
        public string OperatorId { get; set; } = string.Empty;
        public decimal EstimatedTime { get; set; } // hours
        public decimal ActualTime { get; set; } // hours
        public decimal MaterialCost { get; set; }
        public decimal LaborCost { get; set; }
        public decimal MachineCost { get; set; }
        public List<ManufacturingOperation> Operations { get; set; } = new();
        public List<CNCProgram> CNCPrograms { get; set; } = new();
        public List<ManufacturingNote> Notes { get; set; } = new();
    }

    public class ManufacturingOperation
    {
        public int Id { get; set; }
        public string OperationType { get; set; } = string.Empty; // Cutting, Drilling, Routing, Assembly, etc.
        public string Description { get; set; } = string.Empty;
        public int SequenceNumber { get; set; }
        public string MachineType { get; set; } = string.Empty; // CNC, PanelSaw, Edgebander, etc.
        public decimal SetupTime { get; set; } // hours
        public decimal RunTime { get; set; } // hours
        public decimal ToolingCost { get; set; }
        public List<string> RequiredTools { get; set; } = new();
        public List<ManufacturingParameter> Parameters { get; set; } = new();
        public bool IsCompleted { get; set; }
        public DateTime? CompletionTime { get; set; }
        public string QualityNotes { get; set; } = string.Empty;
    }

    public class ManufacturingParameter
    {
        public string ParameterName { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
    }

    public class CNCProgram
    {
        public int Id { get; set; }
        public string ProgramName { get; set; } = string.Empty;
        public string ProgramNumber { get; set; } = string.Empty;
        public string MachineId { get; set; } = string.Empty;
        public string MaterialId { get; set; } = string.Empty;
        public string ProgramType { get; set; } = string.Empty; // Nesting, Drilling, Routing, etc.
        public DateTime CreatedDate { get; set; }
        public DateTime LastModified { get; set; }
        public string ProgramPath { get; set; } = string.Empty;
        public string ProgramCode { get; set; } = string.Empty; // G-code, etc.
        public decimal EstimatedRunTime { get; set; } // minutes
        public List<CNCTool> Tools { get; set; } = new();
        public List<CNCOperation> Operations { get; set; } = new();
        public bool IsOptimized { get; set; }
        public double MaterialUtilization { get; set; } // percentage
    }

    public class CNCTool
    {
        public int Id { get; set; }
        public string ToolNumber { get; set; } = string.Empty;
        public string ToolName { get; set; } = string.Empty;
        public string ToolType { get; set; } = string.Empty; // Drill, EndMill, RouterBit, etc.
        public decimal Diameter { get; set; }
        public decimal Length { get; set; }
        public int Flutes { get; set; }
        public string Coating { get; set; } = string.Empty;
        public decimal CuttingSpeed { get; set; } // SFM
        public decimal FeedRate { get; set; } // IPM
        public decimal DepthOfCut { get; set; }
        public int ToolLife { get; set; } // minutes
        public int CurrentUsage { get; set; } // minutes
        public bool NeedsReplacement { get; set; }
        public string Supplier { get; set; } = string.Empty;
        public decimal Cost { get; set; }
    }

    public class CNCOperation
    {
        public int Id { get; set; }
        public string OperationType { get; set; } = string.Empty; // Drill, Pocket, Profile, etc.
        public int ToolNumber { get; set; }
        public decimal X { get; set; }
        public decimal Y { get; set; }
        public decimal Z { get; set; }
        public decimal Depth { get; set; }
        public decimal FeedRate { get; set; }
        public decimal SpindleSpeed { get; set; }
        public string GCode { get; set; } = string.Empty;
        public int SequenceNumber { get; set; }
    }

    public class Machine
    {
        public int Id { get; set; }
        public string MachineId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // CNC, PanelSaw, Edgebander, etc.
        public string Manufacturer { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public DateTime InstallationDate { get; set; }
        public string Status { get; set; } = "Available"; // Available, Busy, Maintenance, Offline
        public decimal HourlyRate { get; set; }
        public List<string> Capabilities { get; set; } = new();
        public MachineSpecifications Specifications { get; set; } = new();
        public MaintenanceSchedule MaintenanceSchedule { get; set; } = new();
        public List<MaintenanceRecord> MaintenanceHistory { get; set; } = new();
    }

    public class MachineSpecifications
    {
        public decimal MaxWidth { get; set; }
        public decimal MaxLength { get; set; }
        public decimal MaxHeight { get; set; }
        public decimal MaxThickness { get; set; }
        public int SpindleSpeed { get; set; } // RPM
        public int ToolCapacity { get; set; }
        public string ControlSystem { get; set; } = string.Empty;
        public List<string> SupportedFormats { get; set; } = new();
    }

    public class MaintenanceSchedule
    {
        public int Id { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty; // Daily, Weekly, Monthly, Hours
        public int Interval { get; set; }
        public DateTime LastPerformed { get; set; }
        public DateTime NextDue { get; set; }
        public bool IsOverdue { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<string> RequiredParts { get; set; } = new();
        public decimal EstimatedCost { get; set; }
    }

    public class MaintenanceRecord
    {
        public int Id { get; set; }
        public DateTime MaintenanceDate { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public string Technician { get; set; } = string.Empty;
        public decimal Duration { get; set; } // hours
        public decimal Cost { get; set; }
        public string Notes { get; set; } = string.Empty;
        public List<string> PartsReplaced { get; set; } = new();
        public bool WasScheduled { get; set; }
    }

    public class OptimizationResult
    {
        public int Id { get; set; }
        public string ProjectId { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string OptimizationType { get; set; } = string.Empty; // Material, Time, Cost
        public decimal OriginalCost { get; set; }
        public decimal OptimizedCost { get; set; }
        public decimal Savings { get; set; }
        public double MaterialUtilization { get; set; }
        public decimal TotalTime { get; set; }
        public List<OptimizationSuggestion> Suggestions { get; set; } = new();
        public string ReportPath { get; set; } = string.Empty;
    }

    public class OptimizationSuggestion
    {
        public string SuggestionType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal PotentialSavings { get; set; }
        public string Impact { get; set; } = string.Empty; // Low, Medium, High
        public bool IsImplemented { get; set; }
    }

    public class ManufacturingNote
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // Quality, Safety, General
        public bool IsUrgent { get; set; }
    }

    public class QualityControl
    {
        public int Id { get; set; }
        public string JobId { get; set; } = string.Empty;
        public string PartId { get; set; } = string.Empty;
        public DateTime InspectionDate { get; set; }
        public string InspectorId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // Pass, Fail, Rework
        public List<QualityMeasurement> Measurements { get; set; } = new();
        public List<string> Defects { get; set; } = new();
        public string Notes { get; set; } = string.Empty;
        public bool RequiresRework { get; set; }
        public string ReworkInstructions { get; set; } = string.Empty;
    }

    public class QualityMeasurement
    {
        public string MeasurementName { get; set; } = string.Empty;
        public decimal NominalValue { get; set; }
        public decimal ActualValue { get; set; }
        public decimal Tolerance { get; set; }
        public bool IsWithinTolerance { get; set; }
        public string Unit { get; set; } = string.Empty;
    }
}
