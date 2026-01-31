using System;
using System.Runtime.InteropServices;
using System.Text;

namespace CabinetVision.SDK.Interop
{
    /// <summary>
    /// Нативные методы для взаимодействия с Cabinet Vision DLL
    /// </summary>
    public static class NativeMethods
    {
        #region Cabinet Vision Core DLL Imports

        [DllImport("CVGlue.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int CV_Initialize(string dataPath);

        [DllImport("CVGlue.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CV_Shutdown();

        [DllImport("CVGlue.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CV_GetVersion();

        [DllImport("CVGlue.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CV_GetLastError();

        [DllImport("CVGlue.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CV_OpenDatabase(string databasePath);

        [DllImport("CVGlue.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CV_CloseDatabase();

        #endregion

        #region Project Management DLL Imports

        [DllImport("CVCabGen.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int CV_CreateProject(string projectName, string projectPath);

        [DllImport("CVCabGen.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int CV_OpenProject(string projectPath);

        [DllImport("CVCabGen.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CV_CloseProject();

        [DllImport("CVCabGen.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CV_SaveProject();

        [DllImport("CVCabGen.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int CV_AddRoom(string roomName, double width, double height, double depth);

        [DllImport("CVCabGen.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int CV_AddCabinet(int roomId, string cabinetName, string cabinetType, 
            double width, double height, double depth, string material);

        #endregion

        #region Materials Management DLL Imports

        [DllImport("CVMaterial.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int CV_GetMaterialList();

        [DllImport("CVMaterial.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int CV_GetMaterialById(int materialId);

        [DllImport("CVMaterial.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern IntPtr CV_GetMaterialName(int materialId);

        [DllImport("CVMaterial.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double CV_GetMaterialPrice(int materialId);

        [DllImport("CVMaterial.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int CV_CreateMaterial(string name, string category, double price);

        [DllImport("CVMaterial.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CV_UpdateMaterialStock(int materialId, int quantity);

        #endregion

        #region Construction DLL Imports

        [DllImport("CVConstruction.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CV_GenerateConstruction();

        [DllImport("CVConstruction.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CV_GetCutList();

        [DllImport("CVConstruction.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CV_GetOptimizedLayout();

        [DllImport("CVConstruction.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double CV_GetMaterialWaste();

        [DllImport("CVConstruction.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CV_ExportCutList(string filePath, string format);

        #endregion

        #region CNC Manufacturing DLL Imports (S2M)

        [DllImport("PNcCore.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int CNC_Initialize(string configPath);

        [DllImport("PNcCore.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CNC_Shutdown();

        [DllImport("PNcCore.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CNC_CreateJob(string jobName);

        [DllImport("PNcCore.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CNC_AddPartToJob(int jobId, string partName, 
            double width, double height, double thickness);

        [DllImport("PNcCore.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CNC_GenerateGCode(int jobId);

        [DllImport("PNcCore.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int CNC_ExportGCode(int jobId, string filePath);

        [DllImport("PNcCore.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CNC_OptimizeNesting(int jobId);

        [DllImport("PNcCore.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double CNC_GetNestingEfficiency(int jobId);

        #endregion

        #region Database Access DLL Imports

        [DllImport("XDAL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int DB_Connect(string connectionString);

        [DllImport("XDAL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DB_Disconnect();

        [DllImport("XDAL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern IntPtr DB_ExecuteQuery(string query);

        [DllImport("XDAL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int DB_GetRowCount(IntPtr resultSet);

        [DllImport("XDAL.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern IntPtr DB_GetFieldValue(IntPtr resultSet, int row, int column);

        [DllImport("XDAL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DB_FreeResultSet(IntPtr resultSet);

        #endregion

        #region CAD Integration DLL Imports

        [DllImport("XCAD.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int CAD_CreateDrawing(string drawingName);

        [DllImport("XCAD.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CAD_CloseDrawing(int drawingId);

        [DllImport("XCAD.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CAD_AddLine(int drawingId, double x1, double y1, double x2, double y2);

        [DllImport("XCAD.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CAD_AddRectangle(int drawingId, double x, double y, double width, double height);

        [DllImport("XCAD.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CAD_AddCircle(int drawingId, double centerX, double centerY, double radius);

        [DllImport("XCAD.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int CAD_ExportDrawing(int drawingId, string filePath, string format);

        [DllImport("XCAD.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CAD_ImportDrawing(string filePath);

        #endregion

        #region Reporting DLL Imports

        [DllImport("CVReptVw.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int RPT_CreateReport(string reportName);

        [DllImport("CVReptVw.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RPT_CloseReport(int reportId);

        [DllImport("CVReptVw.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int RPT_AddDataSource(int reportId, string dataSourceName);

        [DllImport("CVReptVw.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int RPT_GenerateReport(int reportId, string templatePath);

        [DllImport("CVReptVw.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int RPT_ExportReport(int reportId, string filePath, string format);

        #endregion

        #region Utility Functions

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

        #endregion

        #region Helper Methods

        /// <summary>
        /// Преобразование IntPtr в строку
        /// </summary>
        public static string PtrToString(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                return string.Empty;
            return Marshal.PtrToStringUni(ptr) ?? string.Empty;
        }

        /// <summary>
        /// Преобразование IntPtr в строку ANSI
        /// </summary>
        public static string PtrToStringAnsi(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                return string.Empty;
            return Marshal.PtrToStringAnsi(ptr) ?? string.Empty;
        }

        /// <summary>
        /// Проверка результата нативной функции
        /// </summary>
        public static bool CheckResult(int result)
        {
            return result == 0; // 0 обычно означает успех в Cabinet Vision
        }

        #endregion
    }

    #region Constants

    /// <summary>
    /// Константы Cabinet Vision
    /// </summary>
    public static class CabinetVisionConstants
    {
        // Типы шкафов
        public const string CABINET_TYPE_BASE = "Base";
        public const string CABINET_TYPE_WALL = "Wall";
        public const string CABINET_TYPE_TALL = "Tall";
        public const string CABINET_TYPE_SPECIALTY = "Specialty";

        // Материалы
        public const string MATERIAL_PLYWOOD = "Plywood";
        public const string MATERIAL_SOLID_WOOD = "SolidWood";
        public const string MATERIAL_MDF = "MDF";
        public const string MATERIAL_PARTICLEBOARD = "ParticleBoard";

        // Форматы файлов
        public const string FORMAT_DWG = "DWG";
        public const string FORMAT_DXF = "DXF";
        public const string FORMAT_PDF = "PDF";
        public const string FORMAT_CSV = "CSV";
        public const string FORMAT_EXCEL = "XLSX";

        // Типы отчетов
        public const string REPORT_CUTLIST = "CutList";
        public const string REPORT_MATERIALS = "Materials";
        public const string REPORT_HARDWARE = "Hardware";
        public const string REPORT_COSTING = "Costing";

        // CNC операции
        public const int CNC_OP_DRILL = 1;
        public const int CNC_OP_CUT = 2;
        public const int CNC_OP_ROUTE = 3;
        public const int CNC_OP_POCKET = 4;
    }

    #endregion
}
