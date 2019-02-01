using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace MachineArea.Pn.Infrastructure.Extensions
{
    public static class ExcelWorksheetExtensions
    {
        public static void UpdateValue(
            this ExcelWorksheet excelWorksheet,
            int row,
            int col,
            object value,
            string defaultValue = null)
        {
            excelWorksheet.UpdateValue(row, col, value, false, false, Color.Empty);
        }

        public static void UpdateValue(
            this ExcelWorksheet excelWorksheet,
            int row,
            int col,
            object value,
            bool addBorders,
            string defaultValue = null)
        {
            excelWorksheet.UpdateValue(row, col, value, addBorders, false, Color.Empty);
        }

        public static void UpdateValue(
            this ExcelWorksheet excelWorksheet,
            int row,
            int col,
            object value,
            bool addBorders,
            bool autoFit,
            string defaultValue = null)
        {
            excelWorksheet.UpdateValue(row, col, value, addBorders, autoFit, Color.Empty);
        }

        public static void UpdateValue(
            this ExcelWorksheet excelWorksheet,
            int row,
            int col,
            object value,
            bool addBorders,
            bool autoFit,
            Color color,
            string defaultValue = null)
        {
            excelWorksheet.Cells[row, col].Value = value ?? defaultValue;

            if (value?.GetType() == typeof(decimal))
            {
                excelWorksheet.Cells[row, col].Style.Numberformat.Format = "0.00";
            }

            if (addBorders)
            {
                excelWorksheet.Cells[row, col].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                excelWorksheet.Cells[row, col].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                excelWorksheet.Cells[row, col].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                excelWorksheet.Cells[row, col].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            }

            if (autoFit)
            {
                excelWorksheet.Cells[row, col].AutoFitColumns();
            }

            if (!color.IsEmpty)
            {
                excelWorksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                excelWorksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(color);
            }
        }
    }
}