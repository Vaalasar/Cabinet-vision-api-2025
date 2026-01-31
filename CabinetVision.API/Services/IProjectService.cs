namespace CabinetVision.API.Services
{
    public interface IProjectService
    {
        Task<IEnumerable<Project>> GetAllProjectsAsync();
        Task<Project?> GetProjectByIdAsync(int id);
        Task<Project> CreateProjectAsync(Project project);
        Task<Project> UpdateProjectAsync(int id, Project project);
        Task<bool> DeleteProjectAsync(int id);
        Task<IEnumerable<Project>> GetProjectsByCustomerAsync(string customerId);
        Task<IEnumerable<Project>> GetProjectsByDesignerAsync(string designerId);
        Task<IEnumerable<Project>> GetProjectsByStatusAsync(string status);
        Task<decimal> CalculateProjectCostAsync(int projectId);
        Task<byte[]> GenerateProjectReportAsync(int projectId, string format);
        Task<string> ExportProjectAsync(int projectId, string format);
        Task<bool> DuplicateProjectAsync(int projectId, string newName);
    }
}
