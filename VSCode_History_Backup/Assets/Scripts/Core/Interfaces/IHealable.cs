namespace CDB.Core
{
    /// <summary>
    /// Интерфейс для объектов, которые можно лечить
    /// </summary>
    public interface IHealable
    {
        void Heal(int amount);
        bool CanHeal { get; }
    }
}
