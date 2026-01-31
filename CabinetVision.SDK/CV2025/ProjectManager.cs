using System;
using System.Collections.Generic;
using System.IO;
using CabinetVision.SDK.Core;
using CabinetVision.SDK.Interop;

namespace CabinetVision.SDK.CV2025
{
    /// <summary>
    /// Менеджер проектов Cabinet Vision 2025
    /// </summary>
    public class ProjectManager : IDisposable
    {
        private bool _disposed = false;
        private int _currentProjectId = -1;
        private readonly List<CVProject> _openProjects = new List<CVProject>();

        #region Public Events

        /// <summary>
        /// Событие создания проекта
        /// </summary>
        public event EventHandler<ProjectEventArgs>? ProjectCreated;

        /// <summary>
        /// Событие открытия проекта
        /// </summary>
        public event EventHandler<ProjectEventArgs>? ProjectOpened;

        /// <summary>
        /// Событие сохранения проекта
        /// </summary>
        public event EventHandler<ProjectEventArgs>? ProjectSaved;

        /// <summary>
        /// Событие закрытия проекта
        /// </summary>
        public event EventHandler<ProjectEventArgs>? ProjectClosed;

        #endregion

        #region Project Management Methods

        /// <summary>
        /// Создание нового проекта Cabinet Vision
        /// </summary>
        /// <param name="projectName">Имя проекта</param>
        /// <param name="projectPath">Путь для сохранения проекта</param>
        /// <param name="template">Шаблон проекта (опционально)</param>
        /// <returns>Созданный проект</returns>
        public CVProject CreateProject(string projectName, string projectPath, string? template = null)
        {
            if (!CabinetVisionCore.IsInitialized)
                throw new CabinetVisionException("Cabinet Vision SDK not initialized");

            try
            {
                // Создаем директорию проекта если не существует
                if (!Directory.Exists(projectPath))
                {
                    Directory.CreateDirectory(projectPath);
                }

                // Вызываем нативную функцию создания проекта
                var result = NativeMethods.CV_CreateProject(projectName, projectPath);
                if (!NativeMethods.CheckResult(result))
                {
                    throw new CabinetVisionException($"Failed to create project: {CabinetVisionCore.GetLastError()}");
                }

                var project = new CVProject
                {
                    Id = ++_currentProjectId,
                    Name = projectName,
                    Path = projectPath,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    IsOpen = true
                };

                _openProjects.Add(project);
                ProjectCreated?.Invoke(this, new ProjectEventArgs(project));

                return project;
            }
            catch (Exception ex)
            {
                throw new CabinetVisionException("Failed to create project", ex);
            }
        }

        /// <summary>
        /// Открытие существующего проекта
        /// </summary>
        /// <param name="projectPath">Путь к файлу проекта</param>
        /// <returns>Открытый проект</returns>
        public CVProject OpenProject(string projectPath)
        {
            if (!CabinetVisionCore.IsInitialized)
                throw new CabinetVisionException("Cabinet Vision SDK not initialized");

            if (!File.Exists(projectPath))
                throw new FileNotFoundException("Project file not found", projectPath);

            try
            {
                var result = NativeMethods.CV_OpenProject(projectPath);
                if (!NativeMethods.CheckResult(result))
                {
                    throw new CabinetVisionException($"Failed to open project: {CabinetVisionCore.GetLastError()}");
                }

                var projectName = Path.GetFileNameWithoutExtension(projectPath);
                var project = new CVProject
                {
                    Id = ++_currentProjectId,
                    Name = projectName,
                    Path = projectPath,
                    CreatedDate = File.GetCreationTime(projectPath),
                    ModifiedDate = File.GetLastWriteTime(projectPath),
                    IsOpen = true
                };

                _openProjects.Add(project);
                ProjectOpened?.Invoke(this, new ProjectEventArgs(project));

                return project;
            }
            catch (Exception ex)
            {
                throw new CabinetVisionException("Failed to open project", ex);
            }
        }

        /// <summary>
        /// Сохранение текущего проекта
        /// </summary>
        /// <param name="projectId">ID проекта</param>
        public void SaveProject(int projectId)
        {
            if (!CabinetVisionCore.IsInitialized)
                throw new CabinetVisionException("Cabinet Vision SDK not initialized");

            var project = _openProjects.Find(p => p.Id == projectId);
            if (project == null)
                throw new ArgumentException("Project not found or not open", nameof(projectId));

            try
            {
                var result = NativeMethods.CV_SaveProject();
                if (!NativeMethods.CheckResult(result))
                {
                    throw new CabinetVisionException($"Failed to save project: {CabinetVisionCore.GetLastError()}");
                }

                project.ModifiedDate = DateTime.Now;
                ProjectSaved?.Invoke(this, new ProjectEventArgs(project));
            }
            catch (Exception ex)
            {
                throw new CabinetVisionException("Failed to save project", ex);
            }
        }

        /// <summary>
        /// Закрытие проекта
        /// </summary>
        /// <param name="projectId">ID проекта</param>
        public void CloseProject(int projectId)
        {
            if (!CabinetVisionCore.IsInitialized)
                throw new CabinetVisionException("Cabinet Vision SDK not initialized");

            var project = _openProjects.Find(p => p.Id == projectId);
            if (project == null)
                return;

            try
            {
                NativeMethods.CV_CloseProject();
                project.IsOpen = false;
                _openProjects.Remove(project);
                ProjectClosed?.Invoke(this, new ProjectEventArgs(project));
            }
            catch (Exception ex)
            {
                throw new CabinetVisionException("Failed to close project", ex);
            }
        }

        /// <summary>
        /// Получение списка открытых проектов
        /// </summary>
        /// <returns>Список открытых проектов</returns>
        public IReadOnlyList<CVProject> GetOpenProjects()
        {
            return _openProjects.AsReadOnly();
        }

        #endregion

        #region Room Management

        /// <summary>
        /// Добавление комнаты в проект
        /// </summary>
        /// <param name="projectId">ID проекта</param>
        /// <param name="roomName">Имя комнаты</param>
        /// <param name="width">Ширина</param>
        /// <param name="height">Высота</param>
        /// <param name="depth">Глубина</param>
        /// <returns>Созданная комната</returns>
        public CVRoom AddRoom(int projectId, string roomName, double width, double height, double depth)
        {
            if (!CabinetVisionCore.IsInitialized)
                throw new CabinetVisionException("Cabinet Vision SDK not initialized");

            var project = _openProjects.Find(p => p.Id == projectId);
            if (project == null)
                throw new ArgumentException("Project not found or not open", nameof(projectId));

            try
            {
                var result = NativeMethods.CV_AddRoom(roomName, width, height, depth);
                if (!NativeMethods.CheckResult(result))
                {
                    throw new CabinetVisionException($"Failed to add room: {CabinetVisionCore.GetLastError()}");
                }

                var room = new CVRoom
                {
                    Id = project.Rooms.Count + 1,
                    Name = roomName,
                    Width = width,
                    Height = height,
                    Depth = depth
                };

                project.Rooms.Add(room);
                return room;
            }
            catch (Exception ex)
            {
                throw new CabinetVisionException("Failed to add room", ex);
            }
        }

        #endregion

        #region Cabinet Management

        /// <summary>
        /// Добавление шкафа в комнату
        /// </summary>
        /// <param name="projectId">ID проекта</param>
        /// <param name="roomId">ID комнаты</param>
        /// <param name="cabinetName">Имя шкафа</param>
        /// <param name="cabinetType">Тип шкафа</param>
        /// <param name="width">Ширина</param>
        /// <param name="height">Высота</param>
        /// <param name="depth">Глубина</param>
        /// <param name="material">Материал</param>
        /// <returns>Созданный шкаф</returns>
        public CVCabinet AddCabinet(int projectId, int roomId, string cabinetName, string cabinetType,
            double width, double height, double depth, string material)
        {
            if (!CabinetVisionCore.IsInitialized)
                throw new CabinetVisionException("Cabinet Vision SDK not initialized");

            var project = _openProjects.Find(p => p.Id == projectId);
            if (project == null)
                throw new ArgumentException("Project not found or not open", nameof(projectId));

            var room = project.Rooms.Find(r => r.Id == roomId);
            if (room == null)
                throw new ArgumentException("Room not found", nameof(roomId));

            try
            {
                var result = NativeMethods.CV_AddCabinet(roomId, cabinetName, cabinetType, width, height, depth, material);
                if (!NativeMethods.CheckResult(result))
                {
                    throw new CabinetVisionException($"Failed to add cabinet: {CabinetVisionCore.GetLastError()}");
                }

                var cabinet = new CVCabinet
                {
                    Id = room.Cabinets.Count + 1,
                    Name = cabinetName,
                    Type = cabinetType,
                    Width = width,
                    Height = height,
                    Depth = depth,
                    Material = material
                };

                room.Cabinets.Add(cabinet);
                return cabinet;
            }
            catch (Exception ex)
            {
                throw new CabinetVisionException("Failed to add cabinet", ex);
            }
        }

        #endregion

        #region Project Information

        /// <summary>
        /// Получение информации о проекте
        /// </summary>
        /// <param name="projectId">ID проекта</param>
        /// <returns>Информация о проекте</returns>
        public CVProjectInfo GetProjectInfo(int projectId)
        {
            var project = _openProjects.Find(p => p.Id == projectId);
            if (project == null)
                throw new ArgumentException("Project not found", nameof(projectId));

            return new CVProjectInfo
            {
                Project = project,
                TotalRooms = project.Rooms.Count,
                TotalCabinets = project.Rooms.Sum(r => r.Cabinets.Count),
                EstimatedCost = CalculateProjectCost(project),
                MaterialList = GetProjectMaterials(project)
            };
        }

        /// <summary>
        /// Расчет стоимости проекта
        /// </summary>
        /// <param name="project">Проект</param>
        /// <returns>Стоимость проекта</returns>
        private decimal CalculateProjectCost(CVProject project)
        {
            decimal totalCost = 0;

            foreach (var room in project.Rooms)
            {
                foreach (var cabinet in room.Cabinets)
                {
                    // Базовая стоимость шкафа (расчет по размерам и материалу)
                    var cabinetCost = (decimal)(cabinet.Width * cabinet.Height * cabinet.Depth * 0.01); // Упрощенный расчет
                    totalCost += cabinetCost;
                }
            }

            return totalCost;
        }

        /// <summary>
        /// Получение списка материалов проекта
        /// </summary>
        /// <param name="project">Проект</param>
        /// <returns>Список материалов</returns>
        private List<string> GetProjectMaterials(CVProject project)
        {
            var materials = new HashSet<string>();

            foreach (var room in project.Rooms)
            {
                foreach (var cabinet in room.Cabinets)
                {
                    materials.Add(cabinet.Material);
                }
            }

            return materials.ToList();
        }

        #endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                // Закрываем все открытые проекты
                foreach (var project in _openProjects.ToList())
                {
                    try
                    {
                        CloseProject(project.Id);
                    }
                    catch
                    {
                        // Игнорируем ошибки при закрытии
                    }
                }

                _disposed = true;
            }
        }

        ~ProjectManager()
        {
            Dispose(false);
        }

        #endregion
    }

    #region Data Classes

    /// <summary>
    /// Проект Cabinet Vision
    /// </summary>
    public class CVProject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsOpen { get; set; }
        public List<CVRoom> Rooms { get; set; } = new List<CVRoom>();
        public string? Description { get; set; }
        public string? Customer { get; set; }
        public string? Designer { get; set; }
    }

    /// <summary>
    /// Комната в проекте
    /// </summary>
    public class CVRoom
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Width { get; set; }
        public double Height { get; set; }
        public double Depth { get; set; }
        public string RoomType { get; set; } = "Standard";
        public List<CVCabinet> Cabinets { get; set; } = new List<CVCabinet>();
    }

    /// <summary>
    /// Шкаф
    /// </summary>
    public class CVCabinet
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public double Width { get; set; }
        public double Height { get; set; }
        public double Depth { get; set; }
        public string Material { get; set; } = string.Empty;
        public string Style { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public List<string> Hardware { get; set; } = new List<string>();
    }

    /// <summary>
    /// Информация о проекте
    /// </summary>
    public class CVProjectInfo
    {
        public CVProject Project { get; set; } = null!;
        public int TotalRooms { get; set; }
        public int TotalCabinets { get; set; }
        public decimal EstimatedCost { get; set; }
        public List<string> MaterialList { get; set; } = new List<string>();
    }

    /// <summary>
    /// Аргументы событий проекта
    /// </summary>
    public class ProjectEventArgs : EventArgs
    {
        public CVProject Project { get; }

        public ProjectEventArgs(CVProject project)
        {
            Project = project;
        }
    }

    #endregion
}
