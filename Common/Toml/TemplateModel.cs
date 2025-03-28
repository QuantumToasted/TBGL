using System.IO;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Tomlyn;

namespace TBGL.Common;

public sealed class TemplateModel
{
    public string Name { get; init; } = null!;

    public TemplateGroupModel[] Groups { get; init; } = [];

    public static async Task<TemplateModel> LoadAsync(IStorageFile file)
    {
        await using var stream = await file.OpenReadAsync();
        
        var output = new MemoryStream();
        await stream.CopyToAsync(output);

        using var reader = new StreamReader(output, Encoding.UTF8);
        var toml = await reader.ReadToEndAsync();

        return Toml.ToModel<TemplateModel>(toml);
    }
}