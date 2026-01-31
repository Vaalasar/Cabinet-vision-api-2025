using System;
using System.Collections.Generic;
using System.IO;
using CabinetVision.SDK.Core;
using CabinetVision.SDK.Interop;

namespace CabinetVision.SDK.S2M2025
{
    /// <summary>
    /// Менеджер CNC производства Cabinet Vision S2M 2025
    /// </summary>
    public class CNCManager : IDisposable
    {
        private bool _disposed = false;
        private bool _initialized = false;
        private readonly List<CNCJob> _jobs = new List<CNCJob>();
        private int _nextJobId = 1;

        #region Public Events

        /// <summary>
        /// Событие создания CNC задания
        /// </summary>
        public event EventHandler<CNCJobEventArgs>? JobCreated;

        /// <summary>
        /// Событие генерации G-code
        /// </summary>
        public event EventHandler<CNCJobEventArgs>? GCodeGenerated;

        /// <summary>
        /// Событие оптимизации раскроя
        /// </summary>
        public event EventHandler<CNCOptimizationEventArgs>? OptimizationCompleted;

        #endregion

        #region Initialization

        /// <summary>
        /// Инициализация CNC менеджера
        /// </summary>
        /// <param name="configPath">Путь к конфигурации CNC</param>
        public void Initialize(string? configPath = null)
        {
            if (!CabinetVisionCore.IsInitialized)
                throw new CabinetVisionException("Cabinet Vision SDK not initialized");

            if (_initialized)
                return;

            try
            {
                var config = configPath ?? Path.Combine(CabinetVisionCore.InstallationPath, "S2M 2025");
                var result = NativeMethods.CNC_Initialize(config);
                
                if (!NativeMethods.CheckResult(result))
                {
                    throw new CabinetVisionException($"Failed to initialize CNC: {CabinetVisionCore.GetLastError()}");
                }

                _initialized = true;
            }
            catch (Exception ex)
            {
                throw new CabinetVisionException("Failed to initialize CNC Manager", ex);
            }
        }

        #endregion

        #region Job Management

        /// <summary>
        /// Создание нового CNC задания
        /// </summary>
        /// <param name="jobName">Имя задания</param>
        /// <param name="material">Материал</param>
        /// <param name="thickness">Толщина материала</param>
        /// <returns>Созданное задание</returns>
        public CNCJob CreateJob(string jobName, string material, double thickness)
        {
            if (!_initialized)
                throw new CabinetVisionException("CNC Manager not initialized");

            try
            {
                var result = NativeMethods.CNC_CreateJob(jobName);
                if (!NativeMethods.CheckResult(result))
                {
                    throw new CabinetVisionException($"Failed to create CNC job: {CabinetVisionCore.GetLastError()}");
                }

                var job = new CNCJob
                {
                    Id = _nextJobId++,
                    Name = jobName,
                    Material = material,
                    Thickness = thickness,
                    CreatedDate = DateTime.Now,
                    Status = CNCJobStatus.Created
                };

                _jobs.Add(job);
                JobCreated?.Invoke(this, new CNCJobEventArgs(job));

                return job;
            }
            catch (Exception ex)
            {
                throw new CabinetVisionException("Failed to create CNC job", ex);
            }
        }

        /// <summary>
        /// Добавление детали в CNC задание
        /// </summary>
        /// <param name="jobId">ID задания</param>
        /// <param name="partName">Имя детали</param>
        /// <param name="width">Ширина</param>
        /// <param name="height">Высота</param>
        /// <param name="thickness">Толщина</param>
        /// <param name="quantity">Количество</param>
        /// <returns>Созданная деталь</returns>
        public CNCPart AddPartToJob(int jobId, string partName, double width, double height, double thickness, int quantity = 1)
        {
            if (!_initialized)
                throw new CabinetVisionException("CNC Manager not initialized");

            var job = _jobs.Find(j => j.Id == jobId);
            if (job == null)
                throw new ArgumentException("Job not found", nameof(jobId));

            try
            {
                var result = NativeMethods.CNC_AddPartToJob(jobId, partName, width, height, thickness);
                if (!NativeMethods.CheckResult(result))
                {
                    throw new CabinetVisionException($"Failed to add part to job: {CabinetVisionCore.GetLastError()}");
                }

                var part = new CNCPart
                {
                    Id = job.Parts.Count + 1,
                    Name = partName,
                    Width = width,
                    Height = height,
                    Thickness = thickness,
                    Quantity = quantity,
                    JobId = jobId
                };

                job.Parts.Add(part);
                return part;
            }
            catch (Exception ex)
            {
                throw new CabinetVisionException("Failed to add part to job", ex);
            }
        }

        /// <summary>
        /// Получение списка заданий
        /// </summary>
        /// <returns>Список заданий</returns>
        public IReadOnlyList<CNCJob> GetJobs()
        {
            return _jobs.AsReadOnly();
        }

        /// <summary>
        /// Получение задания по ID
        /// </summary>
        /// <param name="jobId">ID задания</param>
        /// <returns>Задание</returns>
        public CNCJob? GetJob(int jobId)
        {
            return _jobs.Find(j => j.Id == jobId);
        }

        #endregion

        #region G-Code Generation

        /// <summary>
        /// Генерация G-code для задания
        /// </summary>
        /// <param name="jobId">ID задания</param>
        /// <returns>Результат генерации</returns>
        public CNCGenerationResult GenerateGCode(int jobId)
        {
            if (!_initialized)
                throw new CabinetVisionException("CNC Manager not initialized");

            var job = _jobs.Find(j => j.Id == jobId);
            if (job == null)
                throw new ArgumentException("Job not found", nameof(jobId));

            try
            {
                var result = NativeMethods.CNC_GenerateGCode(jobId);
                if (!NativeMethods.CheckResult(result))
                {
                    throw new CabinetVisionException($"Failed to generate G-code: {CabinetVisionCore.GetLastError()}");
                }

                job.Status = CNCJobStatus.GCodeGenerated;
                job.GCodeGeneratedDate = DateTime.Now;

                var generationResult = new CNCGenerationResult
                {
                    JobId = jobId,
                    Success = true,
                    GeneratedDate = DateTime.Now,
                    EstimatedTime = EstimateJobTime(job)
                };

                GCodeGenerated?.Invoke(this, new CNCJobEventArgs(job));
                return generationResult;
            }
            catch (Exception ex)
            {
                throw new CabinetVisionException("Failed to generate G-code", ex);
            }
        }

        /// <summary>
        /// Экспорт G-code в файл
        /// </summary>
        /// <param name="jobId">ID задания</param>
        /// <param name="filePath">Путь к файлу</param>
        public void ExportGCode(int jobId, string filePath)
        {
            if (!_initialized)
                throw new CabinetVisionException("CNC Manager not initialized");

            var job = _jobs.Find(j => j.Id == jobId);
            if (job == null)
                throw new ArgumentException("Job not found", nameof(jobId));

            try
            {
                var result = NativeMethods.CNC_ExportGCode(jobId, filePath);
                if (!NativeMethods.CheckResult(result))
                {
                    throw new CabinetVisionException($"Failed to export G-code: {CabinetVisionCore.GetLastError()}");
                }

                job.GCodeFilePath = filePath;
                job.Status = CNCJobStatus.Exported;
            }
            catch (Exception ex)
            {
                throw new CabinetVisionException("Failed to export G-code", ex);
            }
        }

        #endregion

        #region Nesting Optimization

        /// <summary>
        /// Оптимизация раскроя для задания
        /// </summary>
        /// <param name="jobId">ID задания</param>
        /// <returns>Результат оптимизации</returns>
        public CNCOptimizationResult OptimizeNesting(int jobId)
        {
            if (!_initialized)
                throw new CabinetVisionException("CNC Manager not initialized");

            var job = _jobs.Find(j => j.Id == jobId);
            if (job == null)
                throw new ArgumentException("Job not found", nameof(jobId));

            try
            {
                var result = NativeMethods.CNC_OptimizeNesting(jobId);
                if (!NativeMethods.CheckResult(result))
                {
                    throw new CabinetVisionException($"Failed to optimize nesting: {CabinetVisionCore.GetLastError()}");
                }

                var efficiency = NativeMethods.CNC_GetNestingEfficiency(jobId);
                
                var optimizationResult = new CNCOptimizationResult
                {
                    JobId = jobId,
                    Success = true,
                    MaterialUtilization = efficiency,
                    OptimizedDate = DateTime.Now,
                    WastePercentage = 100.0 - efficiency,
                    EstimatedSavings = CalculateSavings(job, efficiency)
                };

                job.OptimizationResult = optimizationResult;
                job.Status = CNCJobStatus.Optimized;

                OptimizationCompleted?.Invoke(this, new CNCOptimizationEventArgs(optimizationResult));
                return optimizationResult;
            }
            catch (Exception ex)
            {
                throw new CabinetVisionException("Failed to optimize nesting", ex);
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Оценка времени выполнения задания
        /// </summary>
        /// <param name="job">Задание</param>
        /// <returns>Время в минутах</returns>
        private double EstimateJobTime(CNCJob job)
        {
            double totalTime = 0;
            
            foreach (var part in job.Parts)
            {
                // Упрощенный расчет времени (в реальности зависит от сложности операций)
                var partTime = (part.Width * part.Height) / 1000; // Условные единицы
                totalTime += partTime * part.Quantity;
            }

            return totalTime;
        }

        /// <summary>
        /// Расчет экономии от оптимизации
        /// </summary>
        /// <param name="job">Задание</param>
        /// <param name="efficiency">Эффективность</param>
        /// <returns>Экономия</returns>
        private decimal CalculateSavings(CNCJob job, double efficiency)
        {
            // Упрощенный расчет экономии материала
            var materialCostPerSquareFoot = 5.0m; // Условная стоимость
            var totalArea = job.Parts.Sum(p => p.Width * p.Height * p.Quantity);
            var wasteArea = totalArea * (1.0 - efficiency / 100.0);
            
            return materialCostPerSquareFoot * (decimal)wasteArea;
        }

        #endregion

        #region Shutdown

        /// <summary>
        /// Завершение работы CNC менеджера
        /// </summary>
        public void Shutdown()
        {
            if (_initialized)
            {
                try
                {
                    NativeMethods.CNC_Shutdown();
                    _initialized = false;
                }
                catch (Exception ex)
                {
                    throw new CabinetVisionException("Failed to shutdown CNC Manager", ex);
                }
            }
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
                Shutdown();
                _disposed = true;
            }
        }

        ~CNCManager()
        {
            Dispose(false);
        }

        #endregion
    }

    #region Data Classes

    /// <summary>
    /// CNC задание
    /// </summary>
    public class CNCJob
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Material { get; set; } = string.Empty;
        public double Thickness { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? GCodeGeneratedDate { get; set; }
        public string? GCodeFilePath { get; set; }
        public CNCJobStatus Status { get; set; }
        public List<CNCPart> Parts { get; set; } = new List<CNCPart>();
        public CNCOptimizationResult? OptimizationResult { get; set; }
    }

    /// <summary>
    /// CNC деталь
    /// </summary>
    public class CNCPart
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Width { get; set; }
        public double Height { get; set; }
        public double Thickness { get; set; }
        public int Quantity { get; set; }
        public int JobId { get; set; }
        public List<CNCOperation> Operations { get; set; } = new List<CNCOperation>();
    }

    /// <summary>
    /// CNC операция
    /// </summary>
    public class CNCOperation
    {
        public int Id { get; set; }
        public CNCOperationType Type { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Depth { get; set; }
        public double FeedRate { get; set; }
        public int SpindleSpeed { get; set; }
        public int ToolNumber { get; set; }
    }

    /// <summary>
    /// Результат генерации G-code
    /// </summary>
    public class CNCGenerationResult
    {
        public int JobId { get; set; }
        public bool Success { get; set; }
        public DateTime GeneratedDate { get; set; }
        public double EstimatedTime { get; set; } // минуты
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Результат оптимизации раскроя
    /// </summary>
    public class CNCOptimizationResult
    {
        public int JobId { get; set; }
        public bool Success { get; set; }
        public double MaterialUtilization { get; set; } // процент
        public double WastePercentage { get; set; } // процент
        public DateTime OptimizedDate { get; set; }
        public decimal EstimatedSavings { get; set; }
        public List<NestingLayout> Layouts { get; set; } = new List<NestingLayout>();
    }

    /// <summary>
    /// Раскройный лист
    /// </summary>
    public class NestingLayout
    {
        public int LayoutNumber { get; set; }
        public double SheetWidth { get; set; }
        public double SheetHeight { get; set; }
        public List<PlacedPart> PlacedParts { get; set; } = new List<PlacedPart>();
        public double Utilization { get; set; }
    }

    /// <summary>
    /// Размещенная деталь
    /// </summary>
    public class PlacedPart
    {
        public int PartId { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Rotation { get; set; } // градусы
        public bool IsFlipped { get; set; }
    }

    #endregion

    #region Enums

    /// <summary>
    /// Статус CNC задания
    /// </summary>
    public enum CNCJobStatus
    {
        Created,
        PartsAdded,
        Optimized,
        GCodeGenerated,
        Exported,
        Processing,
        Completed,
        Error
    }

    /// <summary>
    /// Тип CNC операции
    /// </summary>
    public enum CNCOperationType
    {
        Drill = CabinetVisionConstants.CNC_OP_DRILL,
        Cut = CabinetVisionConstants.CNC_OP_CUT,
        Route = CabinetVisionConstants.CNC_OP_ROUTE,
        Pocket = CabinetVisionConstants.CNC_OP_POCKET
    }

    #endregion

    #region Event Args

    /// <summary>
    /// Аргументы событий CNC задания
    /// </summary>
    public class CNCJobEventArgs : EventArgs
    {
        public CNCJob Job { get; }

        public CNCJobEventArgs(CNCJob job)
        {
            Job = job;
        }
    }

    /// <summary>
    /// Аргументы событий оптимизации
    /// </summary>
    public class CNCOptimizationEventArgs : EventArgs
    {
        public CNCOptimizationResult Result { get; }

        public CNCOptimizationEventArgs(CNCOptimizationResult result)
        {
            Result = result;
        }
    }

    #endregion
}
