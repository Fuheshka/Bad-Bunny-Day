namespace CDB.Core
{
    /// <summary>
    /// Интерфейс для объектов с подсветкой (обводкой)
    /// </summary>
    public interface IHighlightable
    {
        void Highlight();
        void Unhighlight();
        bool IsHighlighted { get; }
    }
}
