namespace CabinetVision.API.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ILogger<ProjectService> _logger;
        private readonly List<Project> _projects;

        public ProjectService(ILogger<ProjectService> logger)
        {
            _logger = logger;
            _projects = new List<Project>();
            InitializeSampleData();
        }

        public async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            return await Task.FromResult(_projects);
        }

        public async Task<Project?> GetProjectByIdAsync(int id)
        {
            return await Task.FromResult(_projects.FirstOrDefault(p => p.Id == id));
        }

        public async Task<Project> CreateProjectAsync(Project project)
        {
            project.Id = _projects.Any() ? _projects.Max(p => p.Id) + 1 : 1;
            project.CreatedDate = DateTime.UtcNow;
            project.ModifiedDate = DateTime.UtcNow;
            project.ProjectNumber = $"PRJ-{DateTime.Now:yyyyMMdd}-{project.Id:D4}";
            
            _projects.Add(project);
            _logger.LogInformation($"Created new project: {project.Name} (ID: {project.Id})");
            
            return await Task.FromResult(project);
        }

        public async Task<Project> UpdateProjectAsync(int id, Project project)
        {
            var existingProject = _projects.FirstOrDefault(p => p.Id == id);
            if (existingProject == null)
                throw new KeyNotFoundException($"Project with ID {id} not found");

            existingProject.Name = project.Name;
            existingProject.Description = project.Description;
            existingProject.CustomerId = project.CustomerId;
            existingProject.DesignerId = project.DesignerId;
            existingProject.DueDate = project.DueDate;
            existingProject.Status = project.Status;
            existingProject.Notes = project.Notes;
            existingProject.Rooms = project.Rooms;
            existingProject.Settings = project.Settings;
            existingProject.ModifiedDate = DateTime.UtcNow;

            _logger.LogInformation($"Updated project: {existingProject.Name} (ID: {existingProject.Id})");
            return await Task.FromResult(existingProject);
        }

        public async Task<bool> DeleteProjectAsync(int id)
        {
            var project = _projects.FirstOrDefault(p => p.Id == id);
            if (project == null)
                return false;

            _projects.Remove(project);
            _logger.LogInformation($"Deleted project: {project.Name} (ID: {project.Id})");
            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<Project>> GetProjectsByCustomerAsync(string customerId)
        {
            return await Task.FromResult(_projects.Where(p => p.CustomerId == customerId));
        }

        public async Task<IEnumerable<Project>> GetProjectsByDesignerAsync(string designerId)
        {
            return await Task.FromResult(_projects.Where(p => p.DesignerId == designerId));
        }

        public async Task<IEnumerable<Project>> GetProjectsByStatusAsync(string status)
        {
            return await Task.FromResult(_projects.Where(p => p.Status.Equals(status, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<decimal> CalculateProjectCostAsync(int projectId)
        {
            var project = await GetProjectByIdAsync(projectId);
            if (project == null)
                throw new KeyNotFoundException($"Project with ID {projectId} not found");

            decimal materialCost = 0;
            decimal hardwareCost = 0;
            decimal laborCost = 0;

            foreach (var room in project.Rooms)
            {
                foreach (var cabinet in room.Cabinets)
                {
                    materialCost += cabinet.Price;
                    hardwareCost += cabinet.Hardware.Sum(h => h.Price * h.Quantity);
                }

                foreach (var countertop in room.Countertops)
                {
                    materialCost += countertop.Price;
                }
            }

            laborCost = project.Settings.LaborRate * project.Rooms.Sum(r => r.Cabinets.Count * 2); // 2 hours per cabinet

            project.MaterialCost = materialCost;
            project.HardwareCost = hardwareCost;
            project.LaborCost = laborCost;
            project.TotalPrice = materialCost + hardwareCost + laborCost;

            return await Task.FromResult(project.TotalPrice);
        }

        public async Task<byte[]> GenerateProjectReportAsync(int projectId, string format)
        {
            var project = await GetProjectByIdAsync(projectId);
            if (project == null)
                throw new KeyNotFoundException($"Project with ID {projectId} not found");

            // Simulate report generation
            _logger.LogInformation($"Generating {format} report for project {projectId}");
            
            // In real implementation, this would generate actual PDF/Excel reports
            var reportData = $"Project Report: {project.Name}\n" +
                           $"Project Number: {project.ProjectNumber}\n" +
                           $"Customer: {project.CustomerId}\n" +
                           $"Total Cost: {project.TotalPrice:C}\n" +
                           $"Status: {project.Status}\n" +
                           $"Created: {project.CreatedDate:yyyy-MM-dd}";

            return await Task.FromResult(System.Text.Encoding.UTF8.GetBytes(reportData));
        }

        public async Task<string> ExportProjectAsync(int projectId, string format)
        {
            var project = await GetProjectByIdAsync(projectId);
            if (project == null)
                throw new KeyNotFoundException($"Project with ID {projectId} not found");

            var exportPath = $"exports/{project.ProjectNumber}.{format.ToLower()}";
            _logger.LogInformation($"Exporting project {projectId} to {format} format: {exportPath}");

            // In real implementation, this would export to actual file formats
            return await Task.FromResult(exportPath);
        }

        public async Task<bool> DuplicateProjectAsync(int projectId, string newName)
        {
            var originalProject = await GetProjectByIdAsync(projectId);
            if (originalProject == null)
                return false;

            var duplicatedProject = new Project
            {
                Name = newName,
                Description = originalProject.Description,
                CustomerId = originalProject.CustomerId,
                DesignerId = originalProject.DesignerId,
                Status = "New",
                Notes = $"Duplicated from project {originalProject.ProjectNumber}",
                Rooms = originalProject.Rooms,
                Settings = originalProject.Settings
            };

            await CreateProjectAsync(duplicatedProject);
            _logger.LogInformation($"Duplicated project {projectId} as {newName}");
            return true;
        }

        private void InitializeSampleData()
        {
            _projects.Add(new Project
            {
                Id = 1,
                ProjectNumber = "PRJ-20250131-0001",
                Name = "Modern Kitchen Renovation",
                Description = "Complete kitchen renovation with custom cabinets",
                CustomerId = "CUST-001",
                DesignerId = "DES-001",
                CreatedDate = DateTime.UtcNow.AddDays(-10),
                ModifiedDate = DateTime.UtcNow.AddDays(-5),
                DueDate = DateTime.UtcNow.AddDays(20),
                Status = "InProgress",
                TotalPrice = 25000,
                Rooms = new List<Room>
                {
                    new Room
                    {
                        Id = 1,
                        Name = "Main Kitchen",
                        RoomType = "Kitchen",
                        Width = 12,
                        Height = 8,
                        Depth = 10,
                        Cabinets = new List<Cabinet>
                        {
                            new Cabinet
                            {
                                Id = 1,
                                Name = "Base Cabinet 1",
                                CabinetType = "Base",
                                Style = "Shaker",
                                Width = 36,
                                Height = 34.5,
                                Depth = 24,
                                MaterialId = "MAT-001",
                                FinishId = "FIN-001",
                                Price = 450,
                                Hardware = new List<HardwareComponent>
                                {
                                    new HardwareComponent { Id = 1, Name = "Hinge", Type = "Hinge", PartNumber = "HNG-001", Quantity = 2, UnitPrice = 15, Price = 30 }
                                }
                            }
                        }
                    }
                }
            });
        }
    }
}
