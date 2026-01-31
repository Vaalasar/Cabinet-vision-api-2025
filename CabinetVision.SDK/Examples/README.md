# Cabinet Vision SDK 2025 - Примеры использования

Этот каталог содержит примеры использования Cabinet Vision SDK 2025 для различных сценариев интеграции.

## 📁 Структура примеров

```
Examples/
├── BasicUsage.cs          # Базовые примеры использования
├── AdvancedScenarios.cs   # Продвинутые сценарии
├── IntegrationTests.cs    # Тесты интеграции
├── CustomPlugins.cs       # Примеры плагинов
├── PerformanceTests.cs    # Тесты производительности
└── README.md             # Этот файл
```

## 🚀 Быстрый старт

### 1. Инициализация SDK

```csharp
using CabinetVision.SDK.Core;

// Инициализация Cabinet Vision SDK
var result = CabinetVisionCore.Initialize();
if (result == CabinetVisionResult.Success)
{
    Console.WriteLine($"SDK инициализирован. Версия: {CabinetVisionCore.Version}");
}
```

### 2. Создание проекта

```csharp
using CabinetVision.SDK.CV2025;

using var projectManager = new ProjectManager();

// Создание нового проекта
var project = projectManager.CreateProject("Моя кухня", @"C:\Projects\Kitchen");

// Добавление комнаты
var room = projectManager.AddRoom(project.Id, "Кухня", 12.0, 8.0, 10.0);

// Добавление шкафа
var cabinet = projectManager.AddCabinet(project.Id, room.Id, "Базовый шкаф", 
    "Base", 36.0, 34.5, 24.0, "Березовая фанера");

// Сохранение проекта
projectManager.SaveProject(project.Id);
```

### 3. CNC производство

```csharp
using CabinetVision.SDK.S2M2025;

using var cncManager = new CNCManager();
cncManager.Initialize();

// Создание CNC задания
var job = cncManager.CreateJob("Детали кухни", "Березовая фанера", 0.75);

// Добавление деталей
var part = cncManager.AddPartToJob(job.Id, "Боковая панель", 24.0, 84.0, 0.75, 2);

// Оптимизация раскроя
var optimization = cncManager.OptimizeNesting(job.Id);
Console.WriteLine($"Эффективность: {optimization.MaterialUtilization:F1}%");

// Генерация G-code
var gcode = cncManager.GenerateGCode(job.Id);
cncManager.ExportGCode(job.Id, @"C:\CNC\kitchen_parts.nc");
```

## 📋 Подробные примеры

### 🏗️ Управление проектами

#### Создание сложного проекта

```csharp
public void CreateComplexProject()
{
    using var projectManager = new ProjectManager();
    
    var project = projectManager.CreateProject("Сложный проект", @"C:\Projects\Complex");
    
    // Добавление нескольких комнат
    var kitchen = projectManager.AddRoom(project.Id, "Кухня", 14.0, 10.0, 12.0);
    var bathroom = projectManager.AddRoom(project.Id, "Ванная", 8.0, 6.0, 8.0);
    
    // Добавление различных типов шкафов
    var kitchenCabinets = new[]
    {
        projectManager.AddCabinet(project.Id, kitchen.Id, "Базовый шкаф 1", "Base", 36.0, 34.5, 24.0, "Дуб"),
        projectManager.AddCabinet(project.Id, kitchen.Id, "Настенный шкаф 1", "Wall", 30.0, 30.0, 12.0, "Дуб"),
        projectManager.AddCabinet(project.Id, kitchen.Id, "Высокий шкаф", "Tall", 24.0, 84.0, 24.0, "Дуб")
    };
    
    var bathroomCabinets = new[]
    {
        projectManager.AddCabinet(project.Id, bathroom.Id, "Под раковину", "Base", 30.0, 34.5, 18.0, "Влагостойкая МДФ"),
        projectManager.AddCabinet(project.Id, bathroom.Id, "Зеркальный шкаф", "Wall", 24.0, 30.0, 6.0, "Влагостойкая МДФ")
    };
    
    // Получение полной информации
    var projectInfo = projectManager.GetProjectInfo(project.Id);
    Console.WriteLine($"Комнат: {projectInfo.TotalRooms}");
    Console.WriteLine($"Шкафов: {projectInfo.TotalCabinets}");
    Console.WriteLine($"Стоимость: ${projectInfo.EstimatedCost:F2}");
    Console.WriteLine($"Материалы: {string.Join(", ", projectInfo.MaterialList)}");
}
```

### 🏭 CNC производство

#### Оптимизация раскроя

```csharp
public void OptimizeCutting()
{
    using var cncManager = new CNCManager();
    cncManager.Initialize();
    
    var job = cncManager.CreateJob("Оптимизация", "Березовая фанера", 0.75);
    
    // Добавление множества деталей
    var parts = new[]
    {
        ("Боковая панель", 24.0, 84.0, 2),
        ("Верхняя панель", 36.0, 24.0, 1),
        ("Полка", 22.5, 11.5, 3),
        ("Дверца", 23.5, 29.5, 2),
        ("Фасад", 35.5, 29.5, 1)
    };
    
    foreach (var (name, width, height, qty) in parts)
    {
        cncManager.AddPartToJob(job.Id, name, width, height, 0.75, qty);
    }
    
    // Оптимизация с анализом результатов
    var optimization = cncManager.OptimizeNesting(job.Id);
    
    Console.WriteLine($"Результаты оптимизации:");
    Console.WriteLine($"  Эффективность: {optimization.MaterialUtilization:F1}%");
    Console.WriteLine($"  Отходы: {optimization.WastePercentage:F1}%");
    Console.WriteLine($"  Экономия: ${optimization.EstimatedSavings:F2}");
    Console.WriteLine($"  Количество листов: {optimization.Layouts.Count}");
    
    foreach (var layout in optimization.Layouts)
    {
        Console.WriteLine($"  Лист {layout.LayoutNumber}: {layout.Utilization:F1}% использовано");
    }
}
```

### 📊 Работа с событиями

```csharp
public void EventHandling()
{
    // Подписка на события ядра
    CabinetVisionCore.StateChanged += (sender, e) => 
    {
        Console.WriteLine($"[CORE] {e.State}: {e.Message}");
    };
    
    CabinetVisionCore.Error += (sender, e) => 
    {
        Console.WriteLine($"[CORE] ERROR: {e.Error}");
        if (e.Exception != null)
            Console.WriteLine($"  Exception: {e.Exception.Message}");
    };
    
    // Подписка на события проектов
    using var projectManager = new ProjectManager();
    
    projectManager.ProjectCreated += (sender, e) => 
        Console.WriteLine($"[PROJECT] Создан: {e.Project.Name}");
    
    projectManager.ProjectSaved += (sender, e) => 
        Console.WriteLine($"[PROJECT] Сохранен: {e.Project.Name}");
    
    // Подписка на события CNC
    using var cncManager = new CNCManager();
    cncManager.Initialize();
    
    cncManager.JobCreated += (sender, e) => 
        Console.WriteLine($"[CNC] Задание создано: {e.Job.Name}");
    
    cncManager.OptimizationCompleted += (sender, e) => 
        Console.WriteLine($"[CNC] Оптимизация завершена: {e.Result.MaterialUtilization:F1}%");
    
    // Выполнение операций для демонстрации событий
    var project = projectManager.CreateProject("Тест событий", @"C:\Temp\Events");
    var job = cncManager.CreateJob("Тест CNC", "Фанера", 0.75);
    var optimization = cncManager.OptimizeNesting(job.Id);
}
```

### 🔧 Обработка ошибок

```csharp
public void ErrorHandling()
{
    try
    {
        // Попытка инициализации с неверным путем
        var result = CabinetVisionCore.Initialize(@"C:\Invalid\Path");
        if (result != CabinetVisionResult.Success)
        {
            Console.WriteLine($"Ошибка инициализации: {result}");
            Console.WriteLine($"Детали: {CabinetVisionCore.GetLastError()}");
            
            // Обработка различных типов ошибок
            switch (result)
            {
                case CabinetVisionResult.InstallationNotFound:
                    Console.WriteLine("Установка Cabinet Vision не найдена");
                    break;
                case CabinetVisionResult.DllLoadFailed:
                    Console.WriteLine("Ошибка загрузки DLL");
                    break;
                case CabinetVisionResult.LicenseError:
                    Console.WriteLine("Проблема с лицензией");
                    break;
            }
        }
    }
    catch (CabinetVisionException ex)
    {
        Console.WriteLine($"Cabinet Vision Exception: {ex.Message}");
        Console.WriteLine($"Result: {ex.Result}");
        
        if (ex.InnerException != null)
        {
            Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"General Exception: {ex.Message}");
    }
}
```

### 📈 Производительность

```csharp
public void PerformanceTest()
{
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
    
    // Инициализация
    CabinetVisionCore.Initialize();
    stopwatch.Stop();
    Console.WriteLine($"Инициализация: {stopwatch.ElapsedMilliseconds} мс");
    
    // Создание большого проекта
    stopwatch.Restart();
    using var projectManager = new ProjectManager();
    var project = projectManager.CreateProject("Производительность", @"C:\Temp\Perf");
    var room = projectManager.AddRoom(project.Id, "Большая комната", 30.0, 20.0, 15.0);
    
    // Создание 100 шкафов
    for (int i = 0; i < 100; i++)
    {
        projectManager.AddCabinet(project.Id, room.Id, $"Шкаф {i+1}", 
            "Base", 36.0, 34.5, 24.0, "Дуб");
    }
    
    stopwatch.Stop();
    Console.WriteLine($"Создание 100 шкафов: {stopwatch.ElapsedMilliseconds} мс");
    
    // Расчет стоимости
    stopwatch.Restart();
    var projectInfo = projectManager.GetProjectInfo(project.Id);
    stopwatch.Stop();
    Console.WriteLine($"Расчет стоимости: {stopwatch.ElapsedMilliseconds} мс");
    Console.WriteLine($"Общая стоимость: ${projectInfo.EstimatedCost:F2}");
}
```

## 🔌 Расширенные сценарии

### Интеграция с базой данных

```csharp
public void DatabaseIntegration()
{
    // Подключение к базе данных Cabinet Vision
    var connectionString = "Server=localhost;Database=CVData;Trusted_Connection=true;";
    
    using var connection = new System.Data.SqlClient.SqlConnection(connectionString);
    connection.Open();
    
    // Чтение проектов из базы
    var command = new System.Data.SqlClient.SqlCommand(
        "SELECT ProjectID, ProjectName, CreatedDate FROM Projects", connection);
    
    using var reader = command.ExecuteReader();
    while (reader.Read())
    {
        var projectId = reader.GetInt32(0);
        var projectName = reader.GetString(1);
        var createdDate = reader.GetDateTime(2);
        
        Console.WriteLine($"Проект: {projectName} (ID: {projectId}, Создан: {createdDate})");
    }
}
```

### Создание плагина

```csharp
public class CustomCabinetPlugin
{
    public void CreateCustomCabinet(ProjectManager projectManager, int projectId, int roomId)
    {
        // Создание кастомного шкафа с особыми параметрами
        var cabinet = projectManager.AddCabinet(projectId, roomId, "Кастомный шкаф", 
            "Custom", 42.0, 36.0, 26.0, "Красное дерево");
        
        // Добавление специальной фурнитуры
        cabinet.Hardware.AddRange(new[] { "Специальные петли", "Демпферы", "LED подсветка" });
        
        // Расчет специальной цены
        cabinet.Price = CalculateCustomPrice(cabinet);
    }
    
    private decimal CalculateCustomPrice(CVCabinet cabinet)
    {
        // Кастомная логика расчета цены
        var basePrice = (decimal)(cabinet.Width * cabinet.Height * cabinet.Depth * 0.015);
        var materialMultiplier = cabinet.Material == "Красное дерево" ? 2.5m : 1.0m;
        var hardwareCost = cabinet.Hardware.Count * 25.0m;
        
        return (basePrice * materialMultiplier) + hardwareCost;
    }
}
```

## 🧪 Тестирование

### Базовые тесты

```csharp
[Test]
public void TestProjectCreation()
{
    CabinetVisionCore.Initialize();
    
    using var projectManager = new ProjectManager();
    var project = projectManager.CreateProject("Test Project", @"C:\Temp\Test");
    
    Assert.IsNotNull(project);
    Assert.AreEqual("Test Project", project.Name);
    Assert.IsTrue(project.IsOpen);
    
    var projectInfo = projectManager.GetProjectInfo(project.Id);
    Assert.AreEqual(0, projectInfo.TotalRooms);
    Assert.AreEqual(0, projectInfo.TotalCabinets);
    
    projectManager.CloseProject(project.Id);
    CabinetVisionCore.Shutdown();
}
```

## 📚 Дополнительные ресурсы

- [Основная документация SDK](../Documentation/)
- [API Reference](../Documentation/API/)
- [Руководство по установке](../Documentation/Installation.md)
- [FAQ](../Documentation/FAQ.md)
- [Поддержка](mailto:support@cabinetvision-sdk.com)

## 🤝 Вклад в примеры

Если у вас есть интересные примеры использования Cabinet Vision SDK, пожалуйста, поделитесь ими!

1. Fork репозиторий
2. Создайте ветку с вашим примером
3. Добавьте код с комментариями
4. Создайте Pull Request

---

**Cabinet Vision SDK 2025** - Мощный инструмент для интеграции и автоматизации процессов проектирования и производства мебели.
