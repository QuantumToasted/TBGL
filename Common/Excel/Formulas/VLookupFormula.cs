using ClosedXML.Excel;

namespace TBGL.Common;

public sealed class VLookupFormula(XLCellValue lookupValue, IXLRange lookupRange, int columnOffset, bool approximateMatch) : Formula
{
    public XLCellValue LookupValue { get; } = lookupValue;

    public IXLRange LookupRange { get; } = lookupRange;

    public int ColumnOffset { get; } = columnOffset;

    public bool ApproximateMatch { get; } = approximateMatch;

    public override string Name => "VLOOKUP";
    
    public override string[] FormatArguments()
    {
        return
        [
            LookupValue.ToString(),
            LookupRange.ToString()!,
            ColumnOffset.ToString(),
            ApproximateMatch.ToString().ToUpper()
        ];
    }
}