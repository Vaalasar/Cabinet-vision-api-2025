namespace CabinetVision.API.Services
{
    public class MaterialService : IMaterialService
    {
        private readonly ILogger<MaterialService> _logger;
        private readonly List<Material> _materials;
        private readonly List<InventoryTransaction> _transactions;

        public MaterialService(ILogger<MaterialService> logger)
        {
            _logger = logger;
            _materials = new List<Material>();
            _transactions = new List<InventoryTransaction>();
            InitializeSampleData();
        }

        public async Task<IEnumerable<Material>> GetAllMaterialsAsync()
        {
            return await Task.FromResult(_materials);
        }

        public async Task<Material?> GetMaterialByIdAsync(int id)
        {
            return await Task.FromResult(_materials.FirstOrDefault(m => m.Id == id));
        }

        public async Task<Material> CreateMaterialAsync(Material material)
        {
            material.Id = _materials.Any() ? _materials.Max(m => m.Id) + 1 : 1;
            material.LastUpdated = DateTime.UtcNow;
            
            _materials.Add(material);
            _logger.LogInformation($"Created new material: {material.Name} (ID: {material.Id})");
            
            return await Task.FromResult(material);
        }

        public async Task<Material> UpdateMaterialAsync(int id, Material material)
        {
            var existingMaterial = _materials.FirstOrDefault(m => m.Id == id);
            if (existingMaterial == null)
                throw new KeyNotFoundException($"Material with ID {id} not found");

            existingMaterial.Name = material.Name;
            existingMaterial.Category = material.Category;
            existingMaterial.Type = material.Type;
            existingMaterial.Grade = material.Grade;
            existingMaterial.Thickness = material.Thickness;
            existingMaterial.Width = material.Width;
            existingMaterial.Length = material.Length;
            existingMaterial.Unit = material.Unit;
            existingMaterial.PricePerUnit = material.PricePerUnit;
            existingMaterial.WeightPerUnit = material.WeightPerUnit;
            existingMaterial.SupplierId = material.SupplierId;
            existingMaterial.PartNumber = material.PartNumber;
            existingMaterial.StockQuantity = material.StockQuantity;
            existingMaterial.MinStockLevel = material.MinStockLevel;
            existingMaterial.ReorderPoint = material.ReorderPoint;
            existingMaterial.Finish = material.Finish;
            existingMaterial.Color = material.Color;
            existingMaterial.IsStandard = material.IsStandard;
            existingMaterial.Properties = material.Properties;
            existingMaterial.LastUpdated = DateTime.UtcNow;

            _logger.LogInformation($"Updated material: {existingMaterial.Name} (ID: {existingMaterial.Id})");
            return await Task.FromResult(existingMaterial);
        }

        public async Task<bool> DeleteMaterialAsync(int id)
        {
            var material = _materials.FirstOrDefault(m => m.Id == id);
            if (material == null)
                return false;

            _materials.Remove(material);
            _logger.LogInformation($"Deleted material: {material.Name} (ID: {material.Id})");
            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<Material>> GetMaterialsByCategoryAsync(string category)
        {
            return await Task.FromResult(_materials.Where(m => 
                m.Category.Equals(category, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<IEnumerable<Material>> GetLowStockMaterialsAsync()
        {
            return await Task.FromResult(_materials.Where(m => 
                m.StockQuantity <= m.ReorderPoint));
        }

        public async Task<bool> UpdateMaterialStockAsync(int materialId, int quantity, string transactionType, string reason)
        {
            var material = _materials.FirstOrDefault(m => m.Id == materialId);
            if (material == null)
                return false;

            var previousQuantity = material.StockQuantity;
            
            if (transactionType.Equals("In", StringComparison.OrdinalIgnoreCase))
                material.StockQuantity += quantity;
            else if (transactionType.Equals("Out", StringComparison.OrdinalIgnoreCase))
                material.StockQuantity -= quantity;
            else if (transactionType.Equals("Adjustment", StringComparison.OrdinalIgnoreCase))
                material.StockQuantity = quantity;

            var transaction = new InventoryTransaction
            {
                Id = _transactions.Any() ? _transactions.Max(t => t.Id) + 1 : 1,
                ItemId = materialId.ToString(),
                ItemType = "Material",
                TransactionType = transactionType,
                Quantity = quantity,
                UnitCost = material.PricePerUnit,
                Reason = reason,
                TransactionDate = DateTime.UtcNow,
                UserId = "System",
                PreviousQuantity = previousQuantity,
                NewQuantity = material.StockQuantity
            };

            _transactions.Add(transaction);
            material.LastUpdated = DateTime.UtcNow;

            _logger.LogInformation($"Updated stock for material {material.Name}: {previousQuantity} -> {material.StockQuantity}");
            return true;
        }

        public async Task<MaterialCutting> GenerateCuttingPlanAsync(string projectId, string materialId)
        {
            var material = _materials.FirstOrDefault(m => m.Id.ToString() == materialId);
            if (material == null)
                throw new KeyNotFoundException($"Material with ID {materialId} not found");

            var cuttingPlan = new MaterialCutting
            {
                Id = 1,
                ProjectId = projectId,
                MaterialId = materialId,
                CreatedDate = DateTime.UtcNow,
                SheetWidth = material.Width,
                SheetLength = material.Length,
                SheetThickness = material.Thickness,
                MaterialCost = material.PricePerUnit,
                WastePercentage = 15.0,
                Pieces = new List<CuttingPiece>
                {
                    new CuttingPiece
                    {
                        Id = 1,
                        PartId = "CAB-001",
                        PartName = "Side Panel",
                        Width = 24,
                        Length = 84,
                        Quantity = 2,
                        GrainDirection = "Length",
                        HasGrain = true
                    },
                    new CuttingPiece
                    {
                        Id = 2,
                        PartId = "CAB-002",
                        PartName = "Top Panel",
                        Width = 36,
                        Length = 24,
                        Quantity = 1,
                        GrainDirection = "Width",
                        HasGrain = false
                    }
                },
                Layouts = new List<CuttingLayout>
                {
                    new CuttingLayout
                    {
                        LayoutNumber = 1,
                        SheetId = "SHEET-001",
                        UtilizationPercentage = 85.0,
                        ImagePath = "/layouts/layout1.png",
                        PlacedPieces = new List<PlacedPiece>
                        {
                            new PlacedPiece { PieceId = 1, X = 0, Y = 0, Width = 24, Length = 84, Rotation = 0 },
                            new PlacedPiece { PieceId = 2, X = 24, Y = 0, Width = 36, Length = 24, Rotation = 90 }
                        }
                    }
                }
            };

            _logger.LogInformation($"Generated cutting plan for project {projectId}, material {materialId}");
            return await Task.FromResult(cuttingPlan);
        }

        public async Task<decimal> CalculateMaterialCostAsync(int materialId, double quantity)
        {
            var material = _materials.FirstOrDefault(m => m.Id == materialId);
            if (material == null)
                throw new KeyNotFoundException($"Material with ID {materialId} not found");

            var cost = (decimal)(quantity * (double)material.PricePerUnit);
            return await Task.FromResult(cost);
        }

        public async Task<IEnumerable<InventoryTransaction>> GetMaterialTransactionsAsync(int materialId)
        {
            return await Task.FromResult(_transactions.Where(t => 
                t.ItemId == materialId.ToString() && t.ItemType == "Material"));
        }

        private void InitializeSampleData()
        {
            _materials.Add(new Material
            {
                Id = 1,
                Name = "Birch Plywood",
                Category = "Plywood",
                Type = "Birch",
                Grade = "A-1",
                Thickness = 0.75,
                Width = 48,
                Length = 96,
                Unit = "Sheet",
                PricePerUnit = 65.00m,
                WeightPerUnit = 75.0m,
                SupplierId = "SUP-001",
                PartNumber = "BIRCH-PLY-001",
                StockQuantity = 50,
                MinStockLevel = 10,
                ReorderPoint = 15,
                Finish = "Natural",
                Color = "Light Brown",
                IsStandard = true,
                LastUpdated = DateTime.UtcNow
            });

            _materials.Add(new Material
            {
                Id = 2,
                Name = "Red Oak Lumber",
                Category = "SolidWood",
                Type = "Oak",
                Grade = "FAS",
                Thickness = 0.875,
                Width = 6,
                Length = 96,
                Unit = "BoardFoot",
                PricePerUnit = 8.50m,
                WeightPerUnit = 3.5m,
                SupplierId = "SUP-002",
                PartNumber = "OAK-BF-001",
                StockQuantity = 200,
                MinStockLevel = 50,
                ReorderPoint = 75,
                Finish = "Unfinished",
                Color = "Red Brown",
                IsStandard = true,
                LastUpdated = DateTime.UtcNow
            });
        }
    }
}
