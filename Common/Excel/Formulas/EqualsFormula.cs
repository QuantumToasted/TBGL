using ClosedXML.Excel;

namespace TBGL.Common;

public class EqualsFormula(XLCellValue lhs, XLCellValue rhs) : Formula
{
    public override string Name => string.Empty;
    public override bool RequiresParentheses => false;
    
    public XLCellValue LHS { get; } = lhs;

    public XLCellValue RHS { get; } = rhs;

    public override string[] FormatArguments()
    {
        return [$"{LHS}={RHS}"];
    }
}