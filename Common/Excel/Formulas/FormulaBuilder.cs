using System.Text;
using ClosedXML.Excel;

namespace TBGL.Common;

public sealed class FormulaBuilder
{
    private readonly StringBuilder _builder = new("=");

    public override string ToString()
        => _builder.ToString();

    public static FormulaBuilder FromFormula(Formula formula)
    {
        var builder = new FormulaBuilder();
        builder._builder.Append(formula.ToString());
        return builder;
    }

    public static FormulaBuilder operator +(FormulaBuilder builder, Formula formula)
    {
        builder._builder.Append('+').Append(formula);
        return builder;
    }
    
    public static FormulaBuilder operator -(FormulaBuilder builder, Formula formula)
    {
        builder._builder.Append('-').Append(formula);
        return builder;
    }

    public static FormulaBuilder operator &(FormulaBuilder builder, Formula formula)
    {
        builder._builder.Append('&').Append(formula);
        return builder;
    }

    public static FormulaBuilder operator +(FormulaBuilder builder, XLCellValue value)
    {
        builder._builder.Append('+').Append(value);
        return builder;
    }
    
    public static FormulaBuilder operator -(FormulaBuilder builder, XLCellValue value)
    {
        builder._builder.Append('-').Append(value);
        return builder;
    }
    
    public static FormulaBuilder operator &(FormulaBuilder builder, XLCellValue value)
    {
        builder._builder.Append('&').Append(value);
        return builder;
    }

    public static implicit operator FormulaBuilder(Formula formula)
        => FromFormula(formula);

    public static implicit operator string(FormulaBuilder builder)
        => builder._builder.ToString();
}