namespace Atko.Godot.Tiled.Tmx {
    public interface ITmxObjectConverter<T> where T : class {
        TmxObject ToTmxObject(T entity);
        T FromTmxObject(TmxObject tmxObject);
    }
}