namespace TBGL.Common;

public record AccountMetadata(string Category, string SubCategory, string Name)
{
    public sealed override string ToString()
        => $"{Category}-{SubCategory}: {Name}";

    public string GetNumber()
        => $"{Category}-{SubCategory}";

    public static AccountMetadata FromRaw(string accountNumber, string name)
    {
        var split = accountNumber.Split('-');
        return new(split[0], split[1], name);
    }
}