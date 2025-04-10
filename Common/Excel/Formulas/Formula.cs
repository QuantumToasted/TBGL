using System.Text;
using ClosedXML.Excel;

namespace TBGL.Common;

public abstract class Formula
{
    public abstract string Name { get; }

    public virtual bool RequiresParentheses { get; } = true;

    public abstract string[] FormatArguments();

    public override string ToString()
    {
        var builder = new StringBuilder(Name);

        if (RequiresParentheses)
            builder.Append('(');

        builder.AppendJoin(',', FormatArguments());

        if (RequiresParentheses)
            builder.Append(')');

        return builder.ToString();
    }

    public static Formula VLookup(XLCellValue value, IXLRange lookupRange, int columnOffset, bool approximateMatch = false) 
        => new VLookupFormula(value, lookupRange, columnOffset, approximateMatch);

    public static Formula IfError(Formula formula, XLCellValue errorValue)
        => new IfErrorFormula(formula, errorValue);

    public static Formula If(Formula evaluationFormula, XLCellValue trueValue, XLCellValue falseValue)
        => new IfFormula(evaluationFormula, trueValue, falseValue);

    public static Formula Equals(XLCellValue lhs, XLCellValue rhs)
        => new EqualsFormula(lhs, rhs);
    
    // single formula convenience conversion
    public static implicit operator string(Formula formula)
        => FormulaBuilder.FromFormula(formula).ToString();
}