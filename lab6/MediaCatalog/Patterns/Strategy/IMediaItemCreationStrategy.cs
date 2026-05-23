using MediaCatalog.Abstractions.Models;

namespace MediaCatalog.Patterns.Strategy;

/// <summary>
/// Strategy pattern: encapsulates how a new catalog item is created from a prototype.
/// </summary>
public interface IMediaItemCreationStrategy
{
    MediaItem Create(MediaItem prototype);
}

/// <summary>Default strategy: deep copy via <see cref="MediaItem.Clone"/>.</summary>
public sealed class ClonePrototypeStrategy : IMediaItemCreationStrategy
{
    public MediaItem Create(MediaItem prototype) => prototype.Clone();
}

/// <summary>
/// Context object that uses a pluggable <see cref="IMediaItemCreationStrategy"/>.
/// </summary>
public sealed class MediaItemCreationContext
{
    private IMediaItemCreationStrategy _strategy;

    public MediaItemCreationContext(IMediaItemCreationStrategy strategy)
    {
        _strategy = strategy;
    }

    public void SetStrategy(IMediaItemCreationStrategy strategy) => _strategy = strategy;

    public MediaItem CreateItem(MediaItem prototype) => _strategy.Create(prototype);
}
