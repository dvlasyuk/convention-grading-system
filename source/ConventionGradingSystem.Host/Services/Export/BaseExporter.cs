using System.Diagnostics.CodeAnalysis;

using ClosedXML.Excel;

namespace ConventionGradingSystem.Host.Services.Export;

public abstract class BaseExporter
{
    protected static void FillCell(
        [NotNull] IXLWorksheet worksheet,
        int rowNumber,
        ref int columnNumber,
        XLCellValue value)
    {
        worksheet.Cell(rowNumber, columnNumber).Value = value;
        columnNumber++;
    }

    protected static void FillCellsGroup(
        [NotNull] IXLWorksheet worksheet,
        int rowNumber,
        ref int columnNumber,
        int rowsCount,
        XLCellValue value)
    {
        worksheet.Cell(rowNumber, columnNumber).Value = value;
        worksheet.Range(rowNumber, columnNumber, rowNumber + rowsCount - 1, columnNumber).Merge();
        columnNumber++;
    }

    protected static void FillCellsGroup(
        [NotNull] IXLWorksheet worksheet,
        int rowNumber,
        ref int columnNumber,
        XLCellValue groupValue,
        [NotNull] XLCellValue[] values)
    {
        var startGradesColumn = columnNumber;

        worksheet.Cell(rowNumber, columnNumber).Value = groupValue;
        foreach (var value in values)
        {
            worksheet.Cell(rowNumber + 1, columnNumber).Value = value;
            columnNumber++;
        }

        worksheet.Range(rowNumber, startGradesColumn, rowNumber, columnNumber - 1).Merge();
    }

    protected static void FormatTable(
        [NotNull] IXLWorksheet worksheet,
        int startRowNumber,
        int startColumnNumber,
        int tableRowsCount,
        int tableColumnsCount,
        int headerRowsCount)
    {
        var tableRange = worksheet.Range(
            firstCellRow: startRowNumber,
            firstCellColumn: startColumnNumber,
            lastCellRow: startRowNumber + tableRowsCount - 1,
            lastCellColumn: startColumnNumber + tableColumnsCount - 1);

        tableRange.Style.Border.TopBorder = XLBorderStyleValues.Thin;
        tableRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        tableRange.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
        tableRange.Style.Border.RightBorder = XLBorderStyleValues.Thin;

        var headerRange = worksheet.Range(
            firstCellRow: startRowNumber,
            firstCellColumn: startColumnNumber,
            lastCellRow: startRowNumber + headerRowsCount - 1,
            lastCellColumn: startColumnNumber + tableColumnsCount - 1);

        headerRange.Style.Font.Bold = true;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        var tableColumns = worksheet.Columns(startColumnNumber, startColumnNumber + tableColumnsCount - 1);
        tableColumns.AdjustToContents(25d, 100d);
    }

    protected static byte[] ExportWorkbook([NotNull] IXLWorkbook workbook)
    {
        using var memoryStream = new MemoryStream();
        workbook.SaveAs(memoryStream);

        return memoryStream.ToArray();
    }
}
