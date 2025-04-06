using System.Collections.Generic;
using Tomlyn;

namespace TBGL.Common;

public sealed class TemplateModel
{
    public string Name { get; init; } = null!;

    public List<TemplateGroupModel> Groups { get; init; } = [];

    public override string ToString()
        => Name;

    public static TemplateModel FromToml(string toml)
        => Toml.ToModel<TemplateModel>(toml);
}