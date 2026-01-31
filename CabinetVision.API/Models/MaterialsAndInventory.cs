namespace CabinetVision.API.Models
{
    // Управление материалами и инвентарем
    public class Material
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // Plywood, SolidWood, MDF, Laminate, etc.
        public string Type { get; set; } = string.Empty; // Oak, Maple, Cherry, etc.
        public string Grade { get; set; } = string.Empty;
        public double Thickness { get; set; }
        public double Width { get; set; }
        public double Length { get; set; }
        public string Unit { get; set; } = "Sheet"; // Sheet, Linear, SquareFoot
        public decimal PricePerUnit { get; set; }
        public decimal WeightPerUnit { get; set; }
        public string SupplierId { get; set; } = string.Empty;
        public string PartNumber { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public int MinStockLevel { get; set; }
        public int ReorderPoint { get; set; }
        public string Finish { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public bool IsStandard { get; set; }
        public List<MaterialProperty> Properties { get; set; } = new();
        public DateTime LastUpdated { get; set; }
    }

    public class MaterialProperty
    {
        public string PropertyName { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
    }

    public class Hardware
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // Hinges, Handles, Slides, Fasteners, etc.
        public string Type { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string PartNumber { get; set; } = string.Empty;
        public string Finish { get; set; } = string.Empty;
        public string Material { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int MinStockLevel { get; set; }
        public string SupplierId { get; set; } = string.Empty;
        public List<HardwareSpecification> Specifications { get; set; } = new();
        public List<string> CompatibleMaterials { get; set; } = new();
        public DateTime LastUpdated { get; set; }
    }

    public class HardwareSpecification
    {
        public string SpecName { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
    }

    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public List<string> ProductCategories { get; set; } = new();
        public decimal AverageLeadTime { get; set; } // days
        public string PaymentTerms { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime LastOrderDate { get; set; }
    }

    public class InventoryTransaction
    {
        public int Id { get; set; }
        public string ItemId { get; set; } = string.Empty;
        public string ItemType { get; set; } = string.Empty; // Material, Hardware
        public string TransactionType { get; set; } = string.Empty; // In, Out, Adjustment
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string ReferenceId { get; set; } = string.Empty; // Project ID, PO Number, etc.
        public DateTime TransactionDate { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int PreviousQuantity { get; set; }
        public int NewQuantity { get; set; }
    }

    public class PurchaseOrder
    {
        public int Id { get; set; }
        public string PONumber { get; set; } = string.Empty;
        public string SupplierId { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Ordered, Received, Cancelled
        public decimal TotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ShippingCost { get; set; }
        public string OrderedBy { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public List<PurchaseOrderItem> Items { get; set; } = new();
    }

    public class PurchaseOrderItem
    {
        public int Id { get; set; }
        public string ItemId { get; set; } = string.Empty;
        public string ItemType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string PartNumber { get; set; } = string.Empty;
        public bool IsReceived { get; set; }
        public int ReceivedQuantity { get; set; }
    }

    public class MaterialCutting
    {
        public int Id { get; set; }
        public string ProjectId { get; set; } = string.Empty;
        public string MaterialId { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public double SheetWidth { get; set; }
        public double SheetLength { get; set; }
        public double SheetThickness { get; set; }
        public decimal MaterialCost { get; set; }
        public double WastePercentage { get; set; }
        public List<CuttingPiece> Pieces { get; set; } = new();
        public List<CuttingLayout> Layouts { get; set; } = new();
    }

    public class CuttingPiece
    {
        public int Id { get; set; }
        public string PartId { get; set; } = string.Empty;
        public string PartName { get; set; } = string.Empty;
        public double Width { get; set; }
        public double Length { get; set; }
        public int Quantity { get; set; }
        public string GrainDirection { get; set; } = string.Empty;
        public bool HasGrain { get; set; }
    }

    public class CuttingLayout
    {
        public int LayoutNumber { get; set; }
        public string SheetId { get; set; } = string.Empty;
        public List<PlacedPiece> PlacedPieces { get; set; } = new();
        public double UtilizationPercentage { get; set; }
        public string ImagePath { get; set; } = string.Empty;
    }

    public class PlacedPiece
    {
        public int PieceId { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Length { get; set; }
        public double Rotation { get; set; } // degrees
    }
}
