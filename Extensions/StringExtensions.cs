namespace TBGL.Extensions;

public static class StringExtensions
{
    public static string TruncateFromEnd(this string str, int length, char prefix = '…')
    {
        if (str.Length <= length)
            return str;

        return prefix + str[^(length - 1)..];
    }
}