using ClosedXML.Excel;

namespace TBGL.Common;

public class IfErrorFormula(Formula formula, XLCellValue errorValue) : Formula
{
    public Formula Formula { get; } = formula;

    public XLCellValue ErrorValue { get; } = errorValue;

    public override string Name => "IFERROR";
    
    public override string[] FormatArguments()
    {
        return [Formula.ToString(), ErrorValue.ToString()];
    }
}