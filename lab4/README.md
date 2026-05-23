# Lab 4 — Plugins and hierarchy (Media Catalog)

Extension of **lab 3** (media catalog) with **dynamic plugin loading**: new classes in the `MediaItem` hierarchy, serialization, and WPF UI are added without recompiling the host.

## Structure

| Project | Role |
|---------|------|
| `MediaCatalog.Abstractions` | Base `MediaItem`, `TypeRegistry`, signature format |
| `MediaCatalog.Plugin.Sdk` | `IMediaPlugin`, `IPluginContext` |
| `MediaCatalog` | WPF host application |
| `MediaCatalog.Plugin.Podcast` | Sample plugin (type **Подкаст**, id `100`) |
| `MediaCatalog.PluginSigner` | Signs plugin DLLs (10 pt task) |

## Build and run

```powershell
cd lab4
dotnet build MediaCatalog.sln
dotnet run --project MediaCatalog
```

After build, `MediaCatalog.Plugin.Podcast.dll` and `MediaCatalog.Plugin.Podcast.dll.plugin.sig` are copied to `MediaCatalog/bin/Debug/net10.0-windows/Plugins/`.

## Plugin signing (optional 10 points)

```powershell
dotnet run --project MediaCatalog.PluginSigner -- `
  "MediaCatalog\bin\Debug\net10.0-windows\Plugins\MediaCatalog.Plugin.Podcast.dll" 365
```

Keys: `Keys/plugin-trust.public.xml` (host), `Keys/plugin-trust.private.xml` (signer only).

Host verifies: SHA-256 hash, validity period (`NotBefore` / `NotAfter`), RSA-PSS signature.

Dev mode without signature check:

```powershell
dotnet run --project MediaCatalog -- --no-plugin-signature
```

Load a specific DLL:

```powershell
dotnet run --project MediaCatalog -- --plugin "C:\path\to\MyPlugin.dll"
```

## Adding a new plugin (no host code changes)

1. Create a class library referencing `Abstractions` + `Plugin.Sdk`.
2. Add a class inheriting `MediaItem` (use `TypeId >= 100`), register in static constructor via `TypeRegistry.Register`.
3. Implement `IMediaPlugin`: register prototype and merge WPF `ResourceDictionary` with `DataTemplate`.
4. Build DLL, sign with `PluginSigner`, copy to host `Plugins` folder.
