namespace CDB.Core
{
    /// <summary>
    /// Интерфейс для объектов, которые могут получать урон
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(int damage, UnityEngine.Vector3? knockbackSource = null);
        bool IsAlive();
        int CurrentHealth { get; }
        int MaxHealth { get; }
    }
}
