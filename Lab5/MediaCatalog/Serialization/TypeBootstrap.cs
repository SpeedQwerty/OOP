using MediaCatalog.Models;

namespace MediaCatalog.Serialization;

/// <summary>
/// Forces static constructors of built-in media types to register with TypeRegistry.
/// </summary>
internal static class TypeBootstrap
{
    public static void EnsureLoaded()
    {
        _ = new Book();
        _ = new Magazine();
        _ = new EBook();
        _ = new Movie();
        _ = new MusicAlbum();
        _ = new Audiobook();
    }
}
