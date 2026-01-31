using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;

namespace CabinetVision.SDK.Core
{
    /// <summary>
    /// Основной класс Cabinet Vision Core SDK для интеграции с нативными DLL
    /// </summary>
    public static class CabinetVisionCore
    {
        private static bool _isInitialized = false;
        private static string? _installationPath;
        private static readonly object _lockObject = new object();

        #region Native DLL Imports

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("CVGlue.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int CV_Initialize(string dataPath);

        [DllImport("CVGlue.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void CV_Shutdown();

        [DllImport("CVGlue.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int CV_GetVersion();

        [DllImport("CVGlue.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr CV_GetLastError();

        [DllImport("CVGlue.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int CV_OpenDatabase(string databasePath);

        [DllImport("CVGlue.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void CV_CloseDatabase();

        #endregion

        #region Public Properties

        /// <summary>
        /// Путь установки Cabinet Vision
        /// </summary>
        public static string InstallationPath
        {
            get
            {
                if (_installationPath == null)
                {
                    _installationPath = FindCabinetVisionInstallation();
                }
                return _installationPath ?? string.Empty;
            }
        }

        /// <summary>
        /// Версия Cabinet Vision
        /// </summary>
        public static string Version => _isInitialized ? $"2025.{CV_GetVersion()}" : "Not Initialized";

        /// <summary>
        /// Флаг инициализации SDK
        /// </summary>
        public static bool IsInitialized => _isInitialized;

        #endregion

        #region Initialization Methods

        /// <summary>
        /// Инициализация Cabinet Vision SDK
        /// </summary>
        /// <param name="dataPath">Путь к данным Cabinet Vision (опционально)</param>
        /// <returns>Результат инициализации</returns>
        public static CabinetVisionResult Initialize(string? dataPath = null)
        {
            lock (_lockObject)
            {
                if (_isInitialized)
                    return CabinetVisionResult.Success;

                try
                {
                    var installPath = dataPath ?? InstallationPath;
                    if (string.IsNullOrEmpty(installPath))
                        return CabinetVisionResult.InstallationNotFound;

                    if (!Directory.Exists(installPath))
                        return CabinetVisionResult.InvalidPath;

                    // Загрузка нативных DLL
                    if (!LoadNativeLibraries(installPath))
                        return CabinetVisionResult.DllLoadFailed;

                    // Инициализация Cabinet Vision
                    var result = CV_Initialize(installPath);
                    if (result != 0)
                    {
                        var error = Marshal.PtrToStringAnsi(CV_GetLastError());
                        throw new CabinetVisionException($"Failed to initialize Cabinet Vision: {error}");
                    }

                    _isInitialized = true;
                    return CabinetVisionResult.Success;
                }
                catch (Exception ex)
                {
                    throw new CabinetVisionException("Failed to initialize Cabinet Vision SDK", ex);
                }
            }
        }

        /// <summary>
        /// Завершение работы Cabinet Vision SDK
        /// </summary>
        public static void Shutdown()
        {
            lock (_lockObject)
            {
                if (_isInitialized)
                {
                    try
                    {
                        CV_Shutdown();
                        _isInitialized = false;
                    }
                    catch (Exception ex)
                    {
                        throw new CabinetVisionException("Failed to shutdown Cabinet Vision SDK", ex);
                    }
                }
            }
        }

        #endregion

        #region Database Methods

        /// <summary>
        /// Открытие базы данных Cabinet Vision
        /// </summary>
        /// <param name="databasePath">Путь к базе данных</param>
        /// <returns>Результат операции</returns>
        public static CabinetVisionResult OpenDatabase(string databasePath)
        {
            if (!_isInitialized)
                throw new CabinetVisionException("SDK not initialized");

            try
            {
                var result = CV_OpenDatabase(databasePath);
                return result == 0 ? CabinetVisionResult.Success : CabinetVisionResult.DatabaseError;
            }
            catch (Exception ex)
            {
                throw new CabinetVisionException("Failed to open database", ex);
            }
        }

        /// <summary>
        /// Закрытие базы данных Cabinet Vision
        /// </summary>
        public static void CloseDatabase()
        {
            if (_isInitialized)
            {
                CV_CloseDatabase();
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Поиск установки Cabinet Vision в системе
        /// </summary>
        /// <returns>Путь к установке</returns>
        private static string? FindCabinetVisionInstallation()
        {
            var possiblePaths = new[]
            {
                @"C:\Cabinet vision",
                @"C:\Program Files\Cabinet Vision",
                @"C:\Program Files (x86)\Cabinet Vision",
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\Cabinet Vision",
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\Cabinet Vision"
            };

            foreach (var path in possiblePaths)
            {
                if (Directory.Exists(path))
                {
                    // Проверяем наличие ключевых файлов Cabinet Vision
                    var cvFiles = new[] { "CV.exe", "CVGlue.dll", "CVAutomate.dll" };
                    if (cvFiles.All(file => File.Exists(Path.Combine(path, file))))
                    {
                        return path;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Загрузка нативных библиотек Cabinet Vision
        /// </summary>
        /// <param name="installPath">Путь установки</param>
        /// <returns>Результат загрузки</returns>
        private static bool LoadNativeLibraries(string installPath)
        {
            try
            {
                var requiredDlls = new[]
                {
                    "CVGlue.dll",
                    "CVAutomate.dll",
                    "CVManaged.dll",
                    "XCAD.dll",
                    "XCore.dll",
                    "XDAL.dll"
                };

                foreach (var dll in requiredDlls)
                {
                    var dllPath = Path.Combine(installPath, dll);
                    if (File.Exists(dllPath))
                    {
                        var handle = LoadLibrary(dllPath);
                        if (handle == IntPtr.Zero)
                        {
                            var error = Marshal.GetLastWin32Error();
                            throw new CabinetVisionException($"Failed to load {dll}: Error {error}");
                        }
                    }
                    else
                    {
                        throw new CabinetVisionException($"Required DLL not found: {dll}");
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Получение последней ошибки Cabinet Vision
        /// </summary>
        /// <returns>Текст ошибки</returns>
        public static string GetLastError()
        {
            if (!_isInitialized)
                return "SDK not initialized";

            try
            {
                var errorPtr = CV_GetLastError();
                return Marshal.PtrToStringAnsi(errorPtr) ?? "Unknown error";
            }
            catch
            {
                return "Failed to get error message";
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Событие изменения состояния Cabinet Vision
        /// </summary>
        public static event EventHandler<CabinetVisionEventArgs>? StateChanged;

        /// <summary>
        /// Событие ошибки Cabinet Vision
        /// </summary>
        public static event EventHandler<CabinetVisionErrorEventArgs>? Error;

        internal static void OnStateChanged(CabinetVisionState state, string message)
        {
            StateChanged?.Invoke(null, new CabinetVisionEventArgs(state, message));
        }

        internal static void OnError(string error, Exception? exception = null)
        {
            Error?.Invoke(null, new CabinetVisionErrorEventArgs(error, exception));
        }

        #endregion
    }

    #region Enums and Classes

    /// <summary>
    /// Результаты операций Cabinet Vision
    /// </summary>
    public enum CabinetVisionResult
    {
        Success = 0,
        NotInitialized = 1,
        InstallationNotFound = 2,
        InvalidPath = 3,
        DllLoadFailed = 4,
        DatabaseError = 5,
        AccessDenied = 6,
        InvalidParameter = 7,
        OperationFailed = 8,
        LicenseError = 9,
        VersionMismatch = 10
    }

    /// <summary>
    /// Состояния Cabinet Vision
    /// </summary>
    public enum CabinetVisionState
    {
        Uninitialized,
        Initializing,
        Ready,
        Busy,
        Error,
        ShuttingDown
    }

    /// <summary>
    /// Аргументы событий Cabinet Vision
    /// </summary>
    public class CabinetVisionEventArgs : EventArgs
    {
        public CabinetVisionState State { get; }
        public string Message { get; }

        public CabinetVisionEventArgs(CabinetVisionState state, string message)
        {
            State = state;
            Message = message;
        }
    }

    /// <summary>
    /// Аргументы событий ошибок Cabinet Vision
    /// </summary>
    public class CabinetVisionErrorEventArgs : EventArgs
    {
        public string Error { get; }
        public Exception? Exception { get; }

        public CabinetVisionErrorEventArgs(string error, Exception? exception = null)
        {
            Error = error;
            Exception = exception;
        }
    }

    /// <summary>
    /// Исключение Cabinet Vision SDK
    /// </summary>
    public class CabinetVisionException : Exception
    {
        public CabinetVisionResult Result { get; }

        public CabinetVisionException(string message) : base(message)
        {
            Result = CabinetVisionResult.OperationFailed;
        }

        public CabinetVisionException(string message, Exception innerException) : base(message, innerException)
        {
            Result = CabinetVisionResult.OperationFailed;
        }

        public CabinetVisionException(string message, CabinetVisionResult result) : base(message)
        {
            Result = result;
        }
    }

    #endregion
}
