# Cabinet Vision SDK/API 2025

**Комплексный SDK и REST API для программного обеспечения Cabinet Vision 2025**, предоставляющий полный доступ ко всем функциям проектирования, управления материалами, производства и аналитики.

## 🎯 Обзор

На основе глубокого анализа архитектуры Cabinet Vision 2025 создан полнофункциональный SDK, который включает:

- **🏗️ Cabinet Vision Core SDK** - Ядро с нативной интеграцией DLL
- **📋 CV 2025 Module SDK** - Модуль проектирования и конструирования  
- **🏭 S2M 2025 CNC SDK** - Модуль CNC-обработки и оптимизации
- **🔧 Common 2025 SDK** - Общие компоненты и утилиты
- **🌐 REST API** - Веб API для внешней интеграции

## 📁 Структура проекта

```
Cabinet vision-api-2025/
├── CabinetVision.SDK/          # Основной SDK
│   ├── Core/                   # Ядро SDK с нативной интеграцией
│   ├── CV2025/                 # Модуль CV 2025 (проектирование)
│   ├── S2M2025/                # Модуль S2M 2025 (CNC производство)
│   ├── Common2025/             # Общие компоненты
│   ├── Interop/                # Interop с нативными DLL
│   └── Examples/               # Примеры использования
├── CabinetVision.API/          # REST API
│   ├── Controllers/            # API контроллеры
│   ├── Models/                 # Модели данных
│   ├── Services/               # Бизнес-логика
│   └── Configuration/          # Конфигурация
├── Documentation/              # Документация
└── Tests/                      # Тесты
```

## 🚀 Быстрый старт

### Предварительные требования

- **Cabinet Vision 2025** - Установленная программа
- **.NET 8.0 SDK** - Для разработки
- **Visual Studio 2022** или **VS Code** - IDE
- **SQL Server** - Для баз данных (опционально)
- **Windows 10/11** - Требуется для нативных DLL

### Установка и запуск

#### 1. Клонирование репозитория
```bash
git clone <repository-url>
cd "Cabinet vision-api-2025"
```

#### 2. Восстановление зависимостей
```bash
dotnet restore CabinetVision.SDK/CabinetVision.SDK.csproj
dotnet restore CabinetVision.API/CabinetVision.API.csproj
```

#### 3. Сборка SDK
```bash
dotnet build CabinetVision.SDK/CabinetVision.SDK.csproj --configuration Release
```

#### 4. Запуск REST API
```bash
dotnet run --project CabinetVision.API
```

#### 5. Использование SDK в коде
```csharp
using CabinetVision.SDK.Core;
using CabinetVision.SDK.CV2025;
using CabinetVision.SDK.S2M2025;

// Инициализация SDK
var result = CabinetVisionCore.Initialize();
if (result == CabinetVisionResult.Success)
{
    // Работа с проектами
    using var projectManager = new ProjectManager();
    var project = projectManager.CreateProject("Моя кухня", @"C:\Projects\Kitchen");
    
    // Работа с CNC
    using var cncManager = new CNCManager();
    cncManager.Initialize();
    var job = cncManager.CreateJob("Детали кухни", "Березовая фанера", 0.75);
}
```

## 📚 Документация API

### REST API Endpoints
- **Swagger UI**: `https://localhost:7000`
- **OpenAPI JSON**: `https://localhost:7000/swagger/v1/swagger.json`

### SDK Documentation
- **Основной SDK**: [CabinetVision.SDK/](./CabinetVision.SDK/)
- **Примеры**: [Examples/](./CabinetVision.SDK/Examples/)
- **Interop**: [Interop/](./CabinetVision.SDK/Interop/)

## 🏗️ Cabinet Vision Core SDK

### Основные возможности

#### 🔧 Нативная интеграция
- Прямой вызов Cabinet Vision DLL
- Работа с базами данных CVData и CxSystemData
- Поддержка всех форматов файлов (.cvc, .cwc, .sfs)
- COM/ActiveX интеграция

#### 📋 Управление проектами
```csharp
using var projectManager = new ProjectManager();

// Создание проекта
var project = projectManager.CreateProject("Кухня", @"C:\Projects\Kitchen");

// Добавление комнаты
var room = projectManager.AddRoom(project.Id, "Основная кухня", 12.0, 8.0, 10.0);

// Добавление шкафа
var cabinet = projectManager.AddCabinet(project.Id, room.Id, "Базовый шкаф", 
    "Base", 36.0, 34.5, 24.0, "Березовая фанера");

// Получение информации
var info = projectManager.GetProjectInfo(project.Id);
Console.WriteLine($"Стоимость: ${info.EstimatedCost:F2}");
```

#### 🏭 CNC производство
```csharp
using var cncManager = new CNCManager();
cncManager.Initialize();

// Создание задания
var job = cncManager.CreateJob("Детали", "Фанера", 0.75);

// Добавление деталей
cncManager.AddPartToJob(job.Id, "Панель", 24.0, 84.0, 0.75, 2);

// Оптимизация раскроя
var optimization = cncManager.OptimizeNesting(job.Id);
Console.WriteLine($"Эффективность: {optimization.MaterialUtilization:F1}%");

// Генерация G-code
cncManager.GenerateGCode(job.Id);
cncManager.ExportGCode(job.Id, @"C:\CNC\parts.nc");
```

## 🌐 REST API

### Основные эндпоинты

#### Проекты
- `GET /api/projects` - Получить все проекты
- `POST /api/projects` - Создать проект
- `GET /api/projects/{id}/cost` - Рассчитать стоимость
- `GET /api/projects/{id}/report` - Сгенерировать отчет

#### Материалы
- `GET /api/materials` - Получить материалы
- `POST /api/materials/cutting-plan` - Генерация плана раскроя
- `GET /api/materials/lowstock` - Материалы с низким запасом

#### Производство
- `POST /api/manufacturing/jobs` - Создать производственное задание
- `POST /api/manufacturing/optimize` - Оптимизировать производство
- `GET /api/manufacturing/status/{jobId}` - Статус задания

### Пример использования REST API

```bash
# Создание проекта
curl -X POST "https://localhost:7000/api/projects" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Современная кухня",
    "description": "Полная реконструкция",
    "customerId": "CUST-001",
    "rooms": [{
      "name": "Кухня",
      "roomType": "Kitchen",
      "width": 12,
      "height": 8,
      "depth": 10
    }]
  }'

# Расчет стоимости
curl -X GET "https://localhost:7000/api/projects/1/cost"

# Генерация плана раскроя
curl -X POST "https://localhost:7000/api/materials/cutting-plan" \
  -H "Content-Type: application/json" \
  -d '{
    "projectId": "1",
    "materialId": "MAT-001"
  }'
```

## Конфигурация

Основные настройки в `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CabinetVision;Trusted_Connection=true;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "CabinetVision": {
    "DataPath": "C:/Cabinet vision",
    "EnableCNCIntegration": true,
    "DefaultCurrency": "USD",
    "DefaultMeasurementUnit": "Inches"
  }
}
```

## Интеграция с Cabinet Vision

API интегрируется с существующей установкой Cabinet Vision через:

- Прямой доступ к базам данных (CVData, CxSystemData)
- Интеграцию с CNC-модулем S2M
- Использование существующих DLL библиотек
- Работа с файлами проектов и каталогов

## Тестирование

### Запуск тестов
```bash
dotnet test
```

### Интеграционные тесты
```bash
dotnet test --filter Category=Integration
```

## Развертывание

### Docker
```bash
docker build -t cabinet-vision-api .
docker run -p 8080:80 cabinet-vision-api
```

### Azure
```bash
az webapp up --sku F1 --name cabinet-vision-api
```

## Лицензирование

API совместим с лицензией Cabinet Vision 2025 и требует:
- Активной лицензии Cabinet Vision
- Прав доступа к базам данных
- Соответствующих прав на использование API

## Поддержка

- Документация: [Wiki](https://github.com/your-repo/wiki)
- Issues: [GitHub Issues](https://github.com/your-repo/issues)
- Email: support@cabinetvision-api.com

## Версии

- **v1.0.0** - Первоначальный выпуск с базовой функциональностью
- **v1.1.0** - Планируется: Расширенная CNC-интеграция
- **v1.2.0** - Планируется: Мобильное приложение

## Вклад в проект

Мы приветствуем вклад в проект! Пожалуйста, ознакомьтесь с [CONTRIBUTING.md](CONTRIBUTING.md) для получения информации о процессе внесения изменений.

---

**Cabinet Vision API 2025** - Мощный инструмент для интеграции и автоматизации процессов проектирования и производства мебели.
