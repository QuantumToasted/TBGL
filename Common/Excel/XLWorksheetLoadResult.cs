using System;
using ClosedXML.Excel;
using TBGL.Extensions;

namespace TBGL.Common;

public abstract class XLWorksheetLoadResult(string path, IXLWorksheet worksheet) : IDisposable
{
    public string Path { get; } = path;

    public IXLWorksheet Worksheet { get; } = worksheet;
    
    public PropertyMetadata Property { get; } = PropertyMetadata.Parse(worksheet.Cell(6, 2).GetString());
    
    public DateOnly StartDate { get; } = worksheet.Cell(4, 2).GetDateOnly();
    
    public DateOnly EndDate { get; } = worksheet.Cell(5, 2).GetDateOnly();

    public string ReportingBook { get; } = worksheet.Cell(3, 2).GetString();

    public void Dispose()
    {
        Worksheet.Workbook.Dispose();
    }
}