using Microsoft.AspNetCore.Mvc;
using CabinetVision.API.Models;
using CabinetVision.API.Services;

namespace CabinetVision.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaterialsController : ControllerBase
    {
        private readonly IMaterialService _materialService;
        private readonly ILogger<MaterialsController> _logger;

        public MaterialsController(IMaterialService materialService, ILogger<MaterialsController> logger)
        {
            _materialService = materialService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Material>>> GetAllMaterials()
        {
            try
            {
                var materials = await _materialService.GetAllMaterialsAsync();
                return Ok(materials);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all materials");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Material>> GetMaterial(int id)
        {
            try
            {
                var material = await _materialService.GetMaterialByIdAsync(id);
                if (material == null)
                    return NotFound($"Material with ID {id} not found");

                return Ok(material);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving material {MaterialId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Material>> CreateMaterial(Material material)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var createdMaterial = await _materialService.CreateMaterialAsync(material);
                return CreatedAtAction(nameof(GetMaterial), new { id = createdMaterial.Id }, createdMaterial);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating material");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Material>> UpdateMaterial(int id, Material material)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var updatedMaterial = await _materialService.UpdateMaterialAsync(id, material);
                return Ok(updatedMaterial);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating material {MaterialId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMaterial(int id)
        {
            try
            {
                var result = await _materialService.DeleteMaterialAsync(id);
                if (!result)
                    return NotFound($"Material with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting material {MaterialId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<Material>>> GetMaterialsByCategory(string category)
        {
            try
            {
                var materials = await _materialService.GetMaterialsByCategoryAsync(category);
                return Ok(materials);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving materials for category {Category}", category);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("lowstock")]
        public async Task<ActionResult<IEnumerable<Material>>> GetLowStockMaterials()
        {
            try
            {
                var materials = await _materialService.GetLowStockMaterialsAsync();
                return Ok(materials);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving low stock materials");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{id}/stock")]
        public async Task<ActionResult> UpdateMaterialStock(int id, [FromBody] UpdateStockRequest request)
        {
            try
            {
                var result = await _materialService.UpdateMaterialStockAsync(id, request.Quantity, request.TransactionType, request.Reason);
                if (!result)
                    return NotFound($"Material with ID {id} not found");

                return Ok(new { Message = "Stock updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock for material {MaterialId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/transactions")]
        public async Task<ActionResult<IEnumerable<InventoryTransaction>>> GetMaterialTransactions(int id)
        {
            try
            {
                var transactions = await _materialService.GetMaterialTransactionsAsync(id);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving transactions for material {MaterialId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("cutting-plan")]
        public async Task<ActionResult<MaterialCutting>> GenerateCuttingPlan([FromBody] CuttingPlanRequest request)
        {
            try
            {
                var cuttingPlan = await _materialService.GenerateCuttingPlanAsync(request.ProjectId, request.MaterialId);
                return Ok(cuttingPlan);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating cutting plan for project {ProjectId}, material {MaterialId}", 
                    request.ProjectId, request.MaterialId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/cost")]
        public async Task<ActionResult<decimal>> CalculateMaterialCost(int id, [FromQuery] double quantity)
        {
            try
            {
                var cost = await _materialService.CalculateMaterialCostAsync(id, quantity);
                return Ok(cost);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating cost for material {MaterialId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }

    public class UpdateStockRequest
    {
        public int Quantity { get; set; }
        public string TransactionType { get; set; } = string.Empty; // In, Out, Adjustment
        public string Reason { get; set; } = string.Empty;
    }

    public class CuttingPlanRequest
    {
        public string ProjectId { get; set; } = string.Empty;
        public string MaterialId { get; set; } = string.Empty;
    }
}
