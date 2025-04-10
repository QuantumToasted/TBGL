using ClosedXML.Excel;

namespace TBGL.Common;

public class IfFormula(Formula evaluationFormula, XLCellValue trueValue, XLCellValue falseValue) : Formula
{
    public override string Name => "IF";
    
    public Formula EvaluationFormula { get; } = evaluationFormula;

    public XLCellValue TrueValue { get; } = trueValue;

    public XLCellValue FalseValue { get; } = falseValue;

    public override string[] FormatArguments()
    {
        return [EvaluationFormula.ToString(), TrueValue.ToString(), FalseValue.ToString()];
    }
}