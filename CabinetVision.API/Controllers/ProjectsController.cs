using Microsoft.AspNetCore.Mvc;
using CabinetVision.API.Models;
using CabinetVision.API.Services;

namespace CabinetVision.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly ILogger<ProjectsController> _logger;

        public ProjectsController(IProjectService projectService, ILogger<ProjectsController> logger)
        {
            _projectService = projectService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetAllProjects()
        {
            try
            {
                var projects = await _projectService.GetAllProjectsAsync();
                return Ok(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all projects");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            try
            {
                var project = await _projectService.GetProjectByIdAsync(id);
                if (project == null)
                    return NotFound($"Project with ID {id} not found");

                return Ok(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving project {ProjectId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Project>> CreateProject(Project project)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var createdProject = await _projectService.CreateProjectAsync(project);
                return CreatedAtAction(nameof(GetProject), new { id = createdProject.Id }, createdProject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating project");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Project>> UpdateProject(int id, Project project)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var updatedProject = await _projectService.UpdateProjectAsync(id, project);
                return Ok(updatedProject);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating project {ProjectId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProject(int id)
        {
            try
            {
                var result = await _projectService.DeleteProjectAsync(id);
                if (!result)
                    return NotFound($"Project with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting project {ProjectId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjectsByCustomer(string customerId)
        {
            try
            {
                var projects = await _projectService.GetProjectsByCustomerAsync(customerId);
                return Ok(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving projects for customer {CustomerId}", customerId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("designer/{designerId}")]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjectsByDesigner(string designerId)
        {
            try
            {
                var projects = await _projectService.GetProjectsByDesignerAsync(designerId);
                return Ok(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving projects for designer {DesignerId}", designerId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjectsByStatus(string status)
        {
            try
            {
                var projects = await _projectService.GetProjectsByStatusAsync(status);
                return Ok(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving projects with status {Status}", status);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/cost")]
        public async Task<ActionResult<decimal>> CalculateProjectCost(int id)
        {
            try
            {
                var cost = await _projectService.CalculateProjectCostAsync(id);
                return Ok(cost);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating cost for project {ProjectId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/report")]
        public async Task<ActionResult> GenerateProjectReport(int id, [FromQuery] string format = "PDF")
        {
            try
            {
                var reportData = await _projectService.GenerateProjectReportAsync(id, format);
                var contentType = format.ToLower() switch
                {
                    "pdf" => "application/pdf",
                    "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "csv" => "text/csv",
                    _ => "application/octet-stream"
                };

                return File(reportData, contentType, $"project_{id}_report.{format.ToLower()}");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating report for project {ProjectId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/export")]
        public async Task<ActionResult<string>> ExportProject(int id, [FromQuery] string format = "JSON")
        {
            try
            {
                var exportPath = await _projectService.ExportProjectAsync(id, format);
                return Ok(new { ExportPath = exportPath });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting project {ProjectId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{id}/duplicate")]
        public async Task<ActionResult> DuplicateProject(int id, [FromBody] DuplicateProjectRequest request)
        {
            try
            {
                var result = await _projectService.DuplicateProjectAsync(id, request.NewName);
                if (!result)
                    return NotFound($"Project with ID {id} not found");

                return Ok(new { Message = "Project duplicated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error duplicating project {ProjectId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }

    public class DuplicateProjectRequest
    {
        public string NewName { get; set; } = string.Empty;
    }
}
