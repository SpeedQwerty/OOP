# Lab 6 — Design patterns (on top of Lab 5)

Media catalog with dynamic plugins, archive pipeline (variant 2), and **four GoF-style patterns** documented in code (English comments).

## Required: Adapter + classmate plugin exchange

| Assembly | Role |
|----------|------|
| `FriendPlugin.LegacyPack` | **Classmate plugin** — foreign API `ILegacyPackService` / `PackData` / `UnpackData` (no host references) |
| `MediaCatalog.Plugin.Adapter.LegacyPack` | **Adapter** — `LegacyPackPipelineAdapter` implements `IArchivePipelinePlugin` and delegates to the classmate service |

In settings, select **「Adapter: LegacyPack by Classmate」** (`adapter.legacy-pack`) to save/load through the adapted plugin.

To integrate a real classmate DLL: replace or reference their assembly instead of `FriendPlugin.LegacyPack`, keeping the adapter project as the only host-facing plugin.

## Other patterns (2+)

| Pattern | Location | Why it fits |
|---------|----------|-------------|
| **Facade** | `Services/CatalogPersistenceFacade.cs` | Hides serialization, file header, pipeline, and strategy choice from `MainViewModel` |
| **Strategy** | `Patterns/Strategy/*` | `PlainCatalogPersistenceStrategy` vs `ArchivedCatalogPersistenceStrategy`; `ClonePrototypeStrategy` for new items |
| **Observer** | `Patterns/Observer/*` | `CatalogSubject` notifies `MainViewModel` (status line) on add/remove/save/load |

## Build and run

```powershell
cd lab6
dotnet build MediaCatalog.slnx
dotnet run --project MediaCatalog
```

Startup project: **MediaCatalog**.

## Structure

Same as Lab 5, plus:

- `FriendPlugin.LegacyPack/`
- `MediaCatalog.Plugin.Adapter.LegacyPack/`
- `MediaCatalog/Patterns/Observer/`
- `MediaCatalog/Patterns/Strategy/`
- `MediaCatalog/Services/CatalogPersistenceFacade.cs`

## Git

Commit the `lab6` folder under your course repository.
