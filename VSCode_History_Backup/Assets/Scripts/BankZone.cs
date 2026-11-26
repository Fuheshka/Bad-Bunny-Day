using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Зона банкинга "Нора" - при входе игрок сохраняет очки из живота.
/// Прикрепить к GameObject с Collider (isTrigger = true).
/// </summary>
[RequireComponent(typeof(Collider))]
public class BankZone : MonoBehaviour
{
    [Header("Настройки")]
    [Tooltip("Восстанавливать ли HP при банкинге")]
    public bool healOnBank = true;
    
    [Tooltip("Количество HP для восстановления (0 = полное)")]
    public int healAmount = 0;
    
    [Tooltip("Сбрасывать ли сытость (вес) при банкинге")]
    public bool resetSaturation = true;

    [Header("Звуки")]
    [Tooltip("Звук при успешном банкинге (кассовый аппарат / отрыжка)")]
    public AudioClip bankSound;
    
    [Tooltip("Громкость звука")]
    [Range(0f, 1f)] public float bankSoundVolume = 1f;
    
    private AudioSource audioSource;

    [Header("Визуальные эффекты")]
    [Tooltip("Партиклы при банкинге")]
    public ParticleSystem bankParticles;

    [Header("События")]
    [Tooltip("Вызывается при успешном банкинге")]
    public UnityEvent onBankSuccess;
    
    [Tooltip("Вызывается когда игрок входит в зону без очков")]
    public UnityEvent onBankEmpty;

    private bool playerInZone = false;

    void Start()
    {
        // Убедимся что коллайдер - триггер
        Collider col = GetComponent<Collider>();
        if (!col.isTrigger)
        {
            Debug.LogWarning($"BankZone: Collider на {gameObject.name} должен быть Trigger!");
            col.isTrigger = true;
        }

        // Создаём AudioSource если есть звук
        if (bankSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.clip = bankSound;
            audioSource.volume = bankSoundVolume;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Проверяем что это игрок
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;
        
        playerInZone = true;
        ProcessBanking(player);
    }

    void OnTriggerStay(Collider other)
    {
        // На случай если игрок уже был в зоне когда получил очки
        if (!playerInZone) return;
        
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;
        
        // Проверяем есть ли что банкить
        if (ScoreManager.Instance != null && ScoreManager.Instance.HasBellyScore())
        {
            ProcessBanking(player);
        }
    }

    void OnTriggerExit(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            playerInZone = false;
        }
    }

    /// <summary>
    /// Обработка банкинга - перевод очков, сброс веса, лечение
    /// </summary>
    void ProcessBanking(PlayerController player)
    {
        if (ScoreManager.Instance == null) return;
        
        // Проверяем есть ли что банкить
        if (!ScoreManager.Instance.HasBellyScore())
        {
            onBankEmpty?.Invoke();
            return;
        }

        // === БАНКИНГ ОЧКОВ ===
        ScoreManager.Instance.BankBellyScore();

        // === СБРОС СЫТОСТИ (ВЕСА) ===
        if (resetSaturation)
        {
            player.ResetSaturation();
        }

        // === ЛЕЧЕНИЕ ===
        if (healOnBank)
        {
            if (healAmount > 0)
            {
                player.Heal(healAmount);
            }
            else
            {
                // Полное исцеление
                player.Heal(player.maxHealth);
            }
        }

        // === ЭФФЕКТЫ ===
        
        // Звук
        if (audioSource != null && bankSound != null)
        {
            audioSource.PlayOneShot(bankSound, bankSoundVolume);
        }
        
        // Партиклы
        if (bankParticles != null)
        {
            bankParticles.transform.position = player.transform.position;
            bankParticles.Play();
        }

        // Событие
        onBankSuccess?.Invoke();
        
        Debug.Log("[BankZone] Банкинг успешен!");
    }

    void OnDrawGizmos()
    {
        // Отображаем зону в редакторе
        Collider col = GetComponent<Collider>();
        if (col == null) return;

        Gizmos.color = new Color(0f, 1f, 0.5f, 0.3f); // Зелёный полупрозрачный
        
        if (col is BoxCollider box)
        {
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.DrawCube(box.center, box.size);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(box.center, box.size);
        }
        else if (col is SphereCollider sphere)
        {
            Gizmos.DrawSphere(transform.position + sphere.center, sphere.radius * transform.lossyScale.x);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position + sphere.center, sphere.radius * transform.lossyScale.x);
        }
    }
}
