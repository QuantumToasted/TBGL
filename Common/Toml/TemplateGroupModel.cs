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

    public IEnumerable<GeneralLedgerTransactionHistory> Filter(IEnumerable<GeneralLedgerTransactionHistory> histories)
    {
        if (Start is not null && End is not null)
        {
            var found = false;
            foreach (var history in histories)
            {
                if (history.Metadata.GetNumber().Equals(Start))
                    found = true;

                if (found)
                    yield return history;

                if (history.Metadata.GetNumber().Equals(End))
                    yield break;
            }

            yield break;
        }

        foreach (var history in histories)
        {
            if (Codes!.Contains(history.Metadata.GetNumber()))
                yield return history;
        }
    }
}