using ApiCallMonitor.Core.Export;
using ApiCallMonitor.Core.Models;

namespace ApiCallMonitor.Blazor.Services;

/// <summary>Keeps a runnable .ps1 file on disk for each collection, alongside the SQLite database,
/// so a saved configuration always has a matching script sitting next to it - not just a
/// download-on-demand. Collection/Details.razor calls <see cref="WriteAsync"/> after every load
/// (i.e. after every save, since pages reload their state right after saving), and
/// Collections/Index.razor calls it after creating a collection and <see cref="Delete"/> after
/// deleting one.</summary>
public class ScriptFileStore
{
    private readonly IPowerShellScriptGenerator _generator;
    private readonly string _scriptsDirectory;

    public ScriptFileStore(IPowerShellScriptGenerator generator)
    {
        _generator = generator;
        _scriptsDirectory = Path.Combine(AppContext.BaseDirectory, "App_Data", "Scripts");
        Directory.CreateDirectory(_scriptsDirectory);
    }

    public string GetPath(int collectionId) => Path.Combine(_scriptsDirectory, $"collection-{collectionId}.ps1");

    /// <summary>(Re)writes the on-disk script for this collection to match its current saved state.
    /// <paramref name="collection"/>.Calls must already be loaded.</summary>
    public async Task WriteAsync(ApiCallCollection collection, CancellationToken cancellationToken = default)
    {
        var script = _generator.Generate(collection);
        await File.WriteAllTextAsync(GetPath(collection.Id), script, cancellationToken);
    }

    public void Delete(int collectionId)
    {
        var path = GetPath(collectionId);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
