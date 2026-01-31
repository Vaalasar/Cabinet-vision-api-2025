using System;
using System.IO;
using CabinetVision.SDK.Core;
using CabinetVision.SDK.CV2025;
using CabinetVision.SDK.S2M2025;

namespace CabinetVision.SDK.Examples
{
    /// <summary>
    /// Примеры базового использования Cabinet Vision SDK 2025
    /// </summary>
    public class BasicUsage
    {
        public static void Main()
        {
            try
            {
                // Пример 1: Инициализация SDK
                InitializeSDK();

                // Пример 2: Работа с проектами
                WorkWithProjects();

                // Пример 3: CNC производство
                WorkWithCNC();

                // Пример 4: Комплексный пример
                CompleteWorkflow();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                // Завершение работы SDK
                CabinetVisionCore.Shutdown();
            }
        }

        /// <summary>
        /// Пример 1: Инициализация Cabinet Vision SDK
        /// </summary>
        private static void InitializeSDK()
        {
            Console.WriteLine("=== Инициализация Cabinet Vision SDK ===");

            // Проверка установки Cabinet Vision
            Console.WriteLine($"Путь установки: {CabinetVisionCore.InstallationPath}");

            // Инициализация SDK
            var result = CabinetVisionCore.Initialize();
            if (result == CabinetVisionResult.Success)
            {
                Console.WriteLine("✓ SDK успешно инициализирован");
                Console.WriteLine($"Версия Cabinet Vision: {CabinetVisionCore.Version}");
            }
            else
            {
                Console.WriteLine($"✗ Ошибка инициализации: {result}");
                return;
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Пример 2: Работа с проектами
        /// </summary>
        private static void WorkWithProjects()
        {
            Console.WriteLine("=== Работа с проектами ===");

            using var projectManager = new ProjectManager();

            // Подписка на события
            projectManager.ProjectCreated += (sender, e) => 
                Console.WriteLine($"✓ Проект создан: {e.Project.Name}");
            projectManager.ProjectSaved += (sender, e) => 
                Console.WriteLine($"✓ Проект сохранен: {e.Project.Name}");

            try
            {
                // Создание нового проекта
                var projectPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TestProject");
                var project = projectManager.CreateProject("Тестовая кухня", projectPath);
                Console.WriteLine($"Создан проект: {project.Name}");

                // Добавление комнаты
                var room = projectManager.AddRoom(project.Id, "Основная кухня", 12.0, 8.0, 10.0);
                Console.WriteLine($"Добавлена комната: {room.Name}");

                // Добавление шкафов
                var cabinet1 = projectManager.AddCabinet(project.Id, room.Id, "Базовый шкаф 1", 
                    CabinetVisionConstants.CABINET_TYPE_BASE, 36.0, 34.5, 24.0, "Березовая фанера");
                var cabinet2 = projectManager.AddCabinet(project.Id, room.Id, "Настенный шкаф 1", 
                    CabinetVisionConstants.CABINET_TYPE_WALL, 30.0, 30.0, 12.0, "Березовая фанера");

                Console.WriteLine($"Добавлены шкафы: {cabinet1.Name}, {cabinet2.Name}");

                // Получение информации о проекте
                var projectInfo = projectManager.GetProjectInfo(project.Id);
                Console.WriteLine($"Всего комнат: {projectInfo.TotalRooms}");
                Console.WriteLine($"Всего шкафов: {projectInfo.TotalCabinets}");
                Console.WriteLine($"Примерная стоимость: ${projectInfo.EstimatedCost:F2}");

                // Сохранение проекта
                projectManager.SaveProject(project.Id);

                // Закрытие проекта
                projectManager.CloseProject(project.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Ошибка работы с проектами: {ex.Message}");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Пример 3: Работа с CNC
        /// </summary>
        private static void WorkWithCNC()
        {
            Console.WriteLine("=== Работа с CNC производством ===");

            using var cncManager = new CNCManager();

            // Инициализация CNC менеджера
            cncManager.Initialize();

            // Подписка на события
            cncManager.JobCreated += (sender, e) => 
                Console.WriteLine($"✓ CNC задание создано: {e.Job.Name}");
            cncManager.GCodeGenerated += (sender, e) => 
                Console.WriteLine($"✓ G-code сгенерирован для задания: {e.Job.Name}");
            cncManager.OptimizationCompleted += (sender, e) => 
                Console.WriteLine($"✓ Оптимизация завершена. Эффективность: {e.Result.MaterialUtilization:F1}%");

            try
            {
                // Создание CNC задания
                var job = cncManager.CreateJob("Кухонные детали", "Березовая фанера", 0.75);
                Console.WriteLine($"Создано CNC задание: {job.Name}");

                // Добавление деталей
                var part1 = cncManager.AddPartToJob(job.Id, "Боковая панель", 24.0, 84.0, 0.75, 2);
                var part2 = cncManager.AddPartToJob(job.Id, "Верхняя панель", 36.0, 24.0, 0.75, 1);
                var part3 = cncManager.AddPartToJob(job.Id, "Полка", 22.5, 11.5, 0.75, 3);

                Console.WriteLine($"Добавлены детали: {part1.Name}, {part2.Name}, {part3.Name}");

                // Оптимизация раскроя
                var optimizationResult = cncManager.OptimizeNesting(job.Id);
                Console.WriteLine($"Материал использован на {optimizationResult.MaterialUtilization:F1}%");
                Console.WriteLine($"Отходы: {optimizationResult.WastePercentage:F1}%");
                Console.WriteLine($"Экономия: ${optimizationResult.EstimatedSavings:F2}");

                // Генерация G-code
                var generationResult = cncManager.GenerateGCode(job.Id);
                Console.WriteLine($"G-code сгенерирован. Время выполнения: {generationResult.EstimatedTime:F1} мин");

                // Экспорт G-code
                var gcodePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "kitchen_parts.nc");
                cncManager.ExportGCode(job.Id, gcodePath);
                Console.WriteLine($"G-code экспортирован: {gcodePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Ошибка работы с CNC: {ex.Message}");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Пример 4: Комплексный рабочий процесс
        /// </summary>
        private static void CompleteWorkflow()
        {
            Console.WriteLine("=== Комплексный рабочий процесс ===");

            try
            {
                // 1. Создание проекта
                using var projectManager = new ProjectManager();
                var projectPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "CompleteProject");
                var project = projectManager.CreateProject("Полный рабочий процесс", projectPath);

                // 2. Добавление комнаты и шкафов
                var room = projectManager.AddRoom(project.Id, "Кухня", 14.0, 10.0, 12.0);
                
                // Добавление различных типов шкафов
                var cabinets = new[]
                {
                    projectManager.AddCabinet(project.Id, room.Id, "Базовый шкаф 1", 
                        CabinetVisionConstants.CABINET_TYPE_BASE, 36.0, 34.5, 24.0, "Дуб"),
                    projectManager.AddCabinet(project.Id, room.Id, "Базовый шкаф 2", 
                        CabinetVisionConstants.CABINET_TYPE_BASE, 30.0, 34.5, 24.0, "Дуб"),
                    projectManager.AddCabinet(project.Id, room.Id, "Настенный шкаф 1", 
                        CabinetVisionConstants.CABINET_TYPE_WALL, 36.0, 30.0, 12.0, "Дуб"),
                    projectManager.AddCabinet(project.Id, room.Id, "Высокий шкаф", 
                        CabinetVisionConstants.CABINET_TYPE_TALL, 24.0, 84.0, 24.0, "Дуб")
                };

                Console.WriteLine($"Создано {cabinets.Length} шкафов");

                // 3. Получение информации о проекте
                var projectInfo = projectManager.GetProjectInfo(project.Id);
                Console.WriteLine($"Общая стоимость проекта: ${projectInfo.EstimatedCost:F2}");

                // 4. Создание CNC задания на основе проекта
                using var cncManager = new CNCManager();
                cncManager.Initialize();

                var cncJob = cncManager.CreateJob($"{project.Name} - Детали", "Дуб", 0.75);

                // 5. Добавление всех деталей в CNC задание
                int partNumber = 1;
                foreach (var cabinet in cabinets)
                {
                    // Добавляем основные панели шкафа
                    cncManager.AddPartToJob(cncJob.Id, $"Деталь {partNumber++} - Боковая панель {cabinet.Name}", 
                        cabinet.Width - 1.5, cabinet.Height, 0.75, 2);
                    cncManager.AddPartToJob(cncJob.Id, $"Деталь {partNumber++} - Верхняя/нижняя панель {cabinet.Name}", 
                        cabinet.Width, cabinet.Depth, 0.75, 2);
                    
                    if (cabinet.Type != CabinetVisionConstants.CABINET_TYPE_WALL)
                    {
                        cncManager.AddPartToJob(cncJob.Id, $"Деталь {partNumber++} - Задняя панель {cabinet.Name}", 
                            cabinet.Width - 1.0, cabinet.Height - 1.0, 0.25, 1);
                    }
                }

                Console.WriteLine($"Добавлено {cncJob.Parts.Count} деталей в CNC задание");

                // 6. Оптимизация и генерация
                var optimization = cncManager.OptimizeNesting(cncJob.Id);
                var generation = cncManager.GenerateGCode(cncJob.Id);

                // 7. Сохранение результатов
                projectManager.SaveProject(project.Id);
                
                var reportsPath = Path.Combine(projectPath, "Reports");
                Directory.CreateDirectory(reportsPath);
                
                var gcodePath = Path.Combine(reportsPath, $"{project.Name}_gcode.nc");
                cncManager.ExportGCode(cncJob.Id, gcodePath);

                // 8. Вывод результатов
                Console.WriteLine("=== РЕЗУЛЬТАТЫ КОМПЛЕКСНОГО РАБОЧЕГО ПРОЦЕССА ===");
                Console.WriteLine($"Проект: {project.Name}");
                Console.WriteLine($"Комнат: {projectInfo.TotalRooms}");
                Console.WriteLine($"Шкафов: {projectInfo.TotalCabinets}");
                Console.WriteLine($"CNC деталей: {cncJob.Parts.Count}");
                Console.WriteLine($"Эффективность раскроя: {optimization.MaterialUtilization:F1}%");
                Console.WriteLine($"Экономия материала: ${optimization.EstimatedSavings:F2}");
                Console.WriteLine($"Время обработки: {generation.EstimatedTime:F1} минут");
                Console.WriteLine($"G-code сохранен: {gcodePath}");
                Console.WriteLine($"Проект сохранен: {projectPath}");

                // Закрытие проекта
                projectManager.CloseProject(project.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Ошибка в комплексном рабочем процессе: {ex.Message}");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Пример обработки ошибок
        /// </summary>
        private static void ErrorHandlingExample()
        {
            Console.WriteLine("=== Обработка ошибок ===");

            try
            {
                // Попытка инициализации с неверным путем
                var result = CabinetVision.Initialize("C:\\Несуществующий\\путь");
                if (result != CabinetVisionResult.Success)
                {
                    Console.WriteLine($"Ожидаемая ошибка: {result}");
                    Console.WriteLine($"Сообщение об ошибке: {CabinetVision.GetLastError()}");
                }

                // Попытка работы с неинициализированным SDK
                using var projectManager = new ProjectManager();
                try
                {
                    projectManager.CreateProject("Тест", "C:\\test");
                }
                catch (CabinetVisionException ex)
                {
                    Console.WriteLine($"Перехвачено исключение: {ex.Message}");
                    Console.WriteLine($"Результат: {ex.Result}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Неожиданная ошибка: {ex.Message}");
            }

            Console.WriteLine();
        }
    }

    /// <summary>
    /// Расширенные примеры использования SDK
    /// </summary>
    public static class AdvancedExamples
    {
        /// <summary>
        /// Пример работы с событиями
        /// </summary>
        public static void EventHandlingExample()
        {
            Console.WriteLine("=== Работа с событиями ===");

            // Подписка на события ядра
            CabinetVisionCore.StateChanged += (sender, e) => 
                Console.WriteLine($"[CORE] Состояние изменено: {e.State} - {e.Message}");
            
            CabinetVisionCore.Error += (sender, e) => 
                Console.WriteLine($"[CORE] Ошибка: {e.Error}");

            // Инициализация
            var result = CabinetVisionCore.Initialize();
            Console.WriteLine($"Результат инициализации: {result}");

            // Работа с проектами
            using var projectManager = new ProjectManager();
            projectManager.ProjectCreated += (sender, e) => 
                Console.WriteLine($"[PROJECT] Создан: {e.Project.Name}");
            
            projectManager.ProjectClosed += (sender, e) => 
                Console.WriteLine($"[PROJECT] Закрыт: {e.Project.Name}");

            var project = projectManager.CreateProject("Тест событий", "C:\\temp");
            projectManager.CloseProject(project.Id);

            CabinetVisionCore.Shutdown();
        }

        /// <summary>
        /// Пример массовых операций
        /// </summary>
        public static void BulkOperationsExample()
        {
            Console.WriteLine("=== Массовые операции ===");

            CabinetVisionCore.Initialize();

            using var projectManager = new ProjectManager();
            var project = projectManager.CreateProject("Массовый проект", "C:\\temp");
            var room = projectManager.AddRoom(project.Id, "Большая кухня", 20.0, 15.0, 12.0);

            // Создание множества шкафов
            for (int i = 1; i <= 10; i++)
            {
                var cabinet = projectManager.AddCabinet(project.Id, room.Id, $"Шкаф {i}", 
                    CabinetVisionConstants.CABINET_TYPE_BASE, 30.0 + i, 34.5, 24.0, "Береза");
                Console.WriteLine($"Создан шкаф {i}: {cabinet.Name}");
            }

            var projectInfo = projectManager.GetProjectInfo(project.Id);
            Console.WriteLine($"Всего создано шкафов: {projectInfo.TotalCabinets}");

            projectManager.CloseProject(project.Id);
            CabinetVisionCore.Shutdown();
        }
    }
}
