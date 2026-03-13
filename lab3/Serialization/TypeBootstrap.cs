using MediaCatalog.Models;

namespace MediaCatalog.Serialization;

/// <summary>
/// Инициализирует реестр типов — вызывает статические конструкторы всех классов.
/// При добавлении нового класса — добавить одну строку сюда.
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
