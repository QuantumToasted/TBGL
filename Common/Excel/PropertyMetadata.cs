namespace TBGL.Common;

public sealed record PropertyMetadata(string Code, string Name)
{
    public override string ToString()
        => $"{Code} - {Name}";

    public static PropertyMetadata Parse(string rawText)
    {
        var split = rawText.Split("--");
        return new PropertyMetadata(split[0].Trim(), split[1].Trim());
    }
}