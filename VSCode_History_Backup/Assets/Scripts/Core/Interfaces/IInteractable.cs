namespace CDB.Core
{
    /// <summary>
    /// Интерфейс для интерактивных объектов
    /// </summary>
    public interface IInteractable
    {
        void OnInteractStart();
        void OnInteractEnd();
        void OnInteractComplete();
        float InteractionTime { get; }
        bool CanInteract { get; }
    }
}
