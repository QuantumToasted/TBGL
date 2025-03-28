using System;
using ClosedXML.Excel;

namespace TBGL.Common;

public abstract class XLWorksheetLoadResult(string path, IXLWorksheet worksheet) : IDisposable
{
    private const string PROPERTY_METADATA_CELL = "B6";
    private const string START_DATE_CELL = "B4";
    private const string END_DATE_CELL = "B5";

    public string Path { get; } = path;

    public IXLWorksheet Worksheet { get; } = worksheet;
    
    public PropertyMetadata Property { get; } = PropertyMetadata.Parse(worksheet.Cell(PROPERTY_METADATA_CELL).GetString());
    
    public DateOnly StartDate { get; } = DateOnly.Parse(worksheet.Cell(START_DATE_CELL).GetString());
    
    public DateOnly EndDate { get; } = DateOnly.Parse(worksheet.Cell(END_DATE_CELL).GetString());

    public void Dispose()
    {
        Worksheet.Workbook.Dispose();
    }
}