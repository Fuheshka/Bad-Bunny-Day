using UnityEngine;

/// <summary>
/// Управляет музыкой в главном меню и применяет настройки громкости.
/// Повесь на объект с AudioSource музыки в MainMenu.
/// </summary>
public class MainMenuAudio : MonoBehaviour
{
    [Header("Музыка главного меню")]
    [SerializeField] private AudioSource _musicSource;
    [Range(0f, 1f)] public float baseMusicVolume = 0.5f;
    
    private float _volumeMultiplier = 1f;

    void Start()
    {
        // Если AudioSource не назначен, пробуем найти на этом объекте
        if (_musicSource == null)
            _musicSource = GetComponent<AudioSource>();
        
        // Загрузить начальные значения
        if (SettingsManager.Instance != null)
        {
            _volumeMultiplier = SettingsManager.Instance.EffectiveMusicVolume;
            ApplyVolume();
        }
        
        // Запустить музыку если не играет
        if (_musicSource != null && !_musicSource.isPlaying)
        {
            _musicSource.loop = true;
            _musicSource.Play();
        }
    }

    void OnEnable()
    {
        // Подписка на события настроек
        SettingsManager.OnMusicVolumeChanged += OnVolumeChanged;
        SettingsManager.OnMasterVolumeChanged += OnVolumeChanged;
        
        // Применить текущие настройки
        if (SettingsManager.Instance != null)
        {
            _volumeMultiplier = SettingsManager.Instance.EffectiveMusicVolume;
            ApplyVolume();
        }
    }

    void OnDisable()
    {
        // Отписка от событий
        SettingsManager.OnMusicVolumeChanged -= OnVolumeChanged;
        SettingsManager.OnMasterVolumeChanged -= OnVolumeChanged;
    }

    private void OnVolumeChanged(float value)
    {
        if (SettingsManager.Instance != null)
        {
            _volumeMultiplier = SettingsManager.Instance.EffectiveMusicVolume;
            ApplyVolume();
        }
    }

    private void ApplyVolume()
    {
        if (_musicSource != null)
        {
            _musicSource.volume = baseMusicVolume * _volumeMultiplier;
        }
    }
}
