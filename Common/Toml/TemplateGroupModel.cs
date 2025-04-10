using System.Collections.Generic;
using System.Linq;

namespace TBGL.Common;

public class TemplateGroupModel
{
    public string Name { get; init; } = null!;
    
    public string? Color { get; init; }
    
    public string? Start { get; init; }
    
    public string? End { get; init; }
    
    public string[]? Codes { get; init; }

    public string GetName(PropertyMetadata property)
        => Name.Replace("{PROP}", property.Code);

    public IEnumerable<GeneralLedgerTransactionHistory> Filter(PropertyMetadata property, IEnumerable<GeneralLedgerTransactionHistory> histories)
    {
        var start = Start?.Replace("{PROP}", property.Code);
        var end = End?.Replace("{PROP}", property.Code);
        
        if (start is not null && end is not null)
        {
            var found = false;
            foreach (var history in histories)
            {
                if (history.Metadata.GetNumber().Equals(start))
                    found = true;

                if (found)
                    yield return history;

                if (history.Metadata.GetNumber().Equals(end))
                    yield break;
            }

            yield break;
        }

        var codes = Codes?.Select(x => x.Replace("{PROP}", property.Code)).ToList();

        foreach (var history in histories)
        {
            if (codes!.Contains(history.Metadata.GetNumber()))
                yield return history;
        }
    }
}