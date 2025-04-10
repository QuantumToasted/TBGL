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
            var rangeStarted = false;
            foreach (var history in histories)
            {
                if (!rangeStarted)
                {
                    if (!history.Metadata.GetNumber().Equals(start))
                        continue;

                    rangeStarted = true;
                }
                
                // TODO: this feels like a hack.
                // If our "end" is 1200, and there ISN'T a 1200, we should stop if we find anything > 1200.
                var endCategory = int.Parse(end.Split('-')[0]);
                var currentCategory = int.Parse(history.Metadata.Category);
                if (rangeStarted && endCategory < currentCategory)
                    yield break;

                yield return history;

                if (rangeStarted && history.Metadata.GetNumber().Equals(end))
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