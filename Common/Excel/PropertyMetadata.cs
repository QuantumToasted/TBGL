namespace TBGL.Common;

public sealed record PropertyMetadata(int Code, string Name)
{
    public static PropertyMetadata Parse(string rawText)
    {
        var split = rawText.Split("--");
        return new PropertyMetadata(int.Parse(split[0].Trim()), split[1].Trim());
    }
}