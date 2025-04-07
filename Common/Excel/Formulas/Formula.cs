using ClosedXML.Excel;

namespace TBGL.Common;

public abstract class Formula
{
    public abstract string Name { get; }

    public abstract string[] FormatArguments();

    public override string ToString()
        => $"{Name}({string.Join(',', FormatArguments())})";

    public static Formula VLookup(XLCellValue value, IXLRange lookupRange, int columnOffset, bool approximateMatch = false) 
        => new VLookupFormula(value, lookupRange, columnOffset, approximateMatch);
    
    // single formula convenience conversion
    public static implicit operator string(Formula formula)
        => FormulaBuilder.FromFormula(formula).ToString();
}