namespace CabinetVision.API.Services
{
    public interface IMaterialService
    {
        Task<IEnumerable<Material>> GetAllMaterialsAsync();
        Task<Material?> GetMaterialByIdAsync(int id);
        Task<Material> CreateMaterialAsync(Material material);
        Task<Material> UpdateMaterialAsync(int id, Material material);
        Task<bool> DeleteMaterialAsync(int id);
        Task<IEnumerable<Material>> GetMaterialsByCategoryAsync(string category);
        Task<IEnumerable<Material>> GetLowStockMaterialsAsync();
        Task<bool> UpdateMaterialStockAsync(int materialId, int quantity, string transactionType, string reason);
        Task<MaterialCutting> GenerateCuttingPlanAsync(string projectId, string materialId);
        Task<decimal> CalculateMaterialCostAsync(int materialId, double quantity);
        Task<IEnumerable<InventoryTransaction>> GetMaterialTransactionsAsync(int materialId);
    }
}
