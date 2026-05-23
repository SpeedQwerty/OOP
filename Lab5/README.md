# Lab 5 — Plugin pipeline (variant 2: archiving)

Extension of **Lab 4** (media catalog + dynamic plugins). **Variant 2** adds **archiving** of serialized catalog data **before save** and **decompression after load**, implemented by **three archive plugins** and configured from the **settings** menu.

## Requirements coverage

| Points | Feature |
|--------|---------|
| 8 | `IArchivePipelinePlugin`: process bytes before/after file I/O; settings menu depends on loaded plugins; auto load from `Plugins/` + **Load DLL** in UI |
| 10 | Per-plugin parameters in settings (ZIP level, GZip mode, Brotli quality) |

## Projects

| Project | Role |
|---------|------|
| `MediaCatalog.Abstractions` | `MediaItem`, `TypeRegistry`, signatures |
| `MediaCatalog.Plugin.Sdk` | `IMediaPlugin`, `IArchivePipelinePlugin`, settings descriptors |
| `MediaCatalog` | WPF host, settings dialog, pipeline orchestration |
| `MediaCatalog.Plugin.Podcast` | Sample media-type plugin (Lab 4) |
| `MediaCatalog.Plugin.Archive.Zip` | ZIP archive pipeline |
| `MediaCatalog.Plugin.Archive.GZip` | GZip pipeline |
| `MediaCatalog.Plugin.Archive.Brotli` | Brotli pipeline |
| `MediaCatalog.PluginSigner` | Signs plugin DLLs |

## Build and run

```powershell
cd Lab5
dotnet build MediaCatalog.slnx
dotnet run --project MediaCatalog
```

On build, signed plugin DLLs are copied to `MediaCatalog/bin/Debug/net10.0-windows/Plugins/`.

## Settings (Настройки → Плагины и архивация)

- Enable/disable archiving on save/load
- Choose active archive plugin (ZIP / GZip / Brotli)
- Configure plugin-specific options (10 pt)
- **Загрузить DLL…** — load a plugin from disk
- **Обновить из папки Plugins** — rescan the Plugins folder without restarting the app

## File format

When archiving is enabled, files start with header `MC5A` + pipeline id + compressed payload. Legacy plain `.bin` files (Lab 3/4) still load without a header.

## Dev: run without signature check

```powershell
dotnet run --project MediaCatalog -- --no-plugin-signature
```
