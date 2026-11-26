using UnityEngine;
using System;

/// <summary>
/// Синглтон для управления настройками игры.
/// Сохраняет и загружает настройки через PlayerPrefs.
/// </summary>
public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    // === Ключи для PlayerPrefs ===
    private const string KEY_MASTER_VOLUME = "Settings_MasterVolume";
    private const string KEY_MUSIC_VOLUME = "Settings_MusicVolume";
    private const string KEY_SFX_VOLUME = "Settings_SFXVolume";
    private const string KEY_SENSITIVITY = "Settings_Sensitivity";
    private const string KEY_INVERT_Y = "Settings_InvertY";

    // === Значения по умолчанию ===
    private const float DEFAULT_MASTER_VOLUME = 1f;
    private const float DEFAULT_MUSIC_VOLUME = 0.5f;
    private const float DEFAULT_SFX_VOLUME = 0.7f;
    private const float DEFAULT_SENSITIVITY = 0.5f;
    private const bool DEFAULT_INVERT_Y = false;

    // === События для подписки ===
    public static event Action<float> OnMasterVolumeChanged;
    public static event Action<float> OnMusicVolumeChanged;
    public static event Action<float> OnSFXVolumeChanged;
    public static event Action<float> OnSensitivityChanged;
    public static event Action<bool> OnInvertYChanged;

    // === Текущие значения ===
    private float _masterVolume;
    private float _musicVolume;
    private float _sfxVolume;
    private float _sensitivity;
    private bool _invertY;

    // === Публичные свойства ===
    public float MasterVolume
    {
        get => _masterVolume;
        set
        {
            _masterVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(KEY_MASTER_VOLUME, _masterVolume);
            PlayerPrefs.Save();
            OnMasterVolumeChanged?.Invoke(_masterVolume);
        }
    }

    public float MusicVolume
    {
        get => _musicVolume;
        set
        {
            _musicVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(KEY_MUSIC_VOLUME, _musicVolume);
            PlayerPrefs.Save();
            OnMusicVolumeChanged?.Invoke(_musicVolume);
        }
    }

    public float SFXVolume
    {
        get => _sfxVolume;
        set
        {
            _sfxVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(KEY_SFX_VOLUME, _sfxVolume);
            PlayerPrefs.Save();
            OnSFXVolumeChanged?.Invoke(_sfxVolume);
        }
    }

    public float Sensitivity
    {
        get => _sensitivity;
        set
        {
            _sensitivity = Mathf.Clamp(value, 0.1f, 2f);
            PlayerPrefs.SetFloat(KEY_SENSITIVITY, _sensitivity);
            PlayerPrefs.Save();
            OnSensitivityChanged?.Invoke(_sensitivity);
        }
    }

    public bool InvertY
    {
        get => _invertY;
        set
        {
            _invertY = value;
            PlayerPrefs.SetInt(KEY_INVERT_Y, _invertY ? 1 : 0);
            PlayerPrefs.Save();
            OnInvertYChanged?.Invoke(_invertY);
        }
    }

    // === Вычисляемые громкости (Master * Channel) ===
    public float EffectiveMusicVolume => _masterVolume * _musicVolume;
    public float EffectiveSFXVolume => _masterVolume * _sfxVolume;

    void Awake()
    {
        // Синглтон
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadSettings();
    }

    /// <summary>
    /// Загружает все настройки из PlayerPrefs
    /// </summary>
    private void LoadSettings()
    {
        _masterVolume = PlayerPrefs.GetFloat(KEY_MASTER_VOLUME, DEFAULT_MASTER_VOLUME);
        _musicVolume = PlayerPrefs.GetFloat(KEY_MUSIC_VOLUME, DEFAULT_MUSIC_VOLUME);
        _sfxVolume = PlayerPrefs.GetFloat(KEY_SFX_VOLUME, DEFAULT_SFX_VOLUME);
        _sensitivity = PlayerPrefs.GetFloat(KEY_SENSITIVITY, DEFAULT_SENSITIVITY);
        _invertY = PlayerPrefs.GetInt(KEY_INVERT_Y, DEFAULT_INVERT_Y ? 1 : 0) == 1;

        // Уведомляем подписчиков о загруженных значениях
        OnMasterVolumeChanged?.Invoke(_masterVolume);
        OnMusicVolumeChanged?.Invoke(_musicVolume);
        OnSFXVolumeChanged?.Invoke(_sfxVolume);
        OnSensitivityChanged?.Invoke(_sensitivity);
        OnInvertYChanged?.Invoke(_invertY);
    }

    /// <summary>
    /// Сбрасывает все настройки к значениям по умолчанию
    /// </summary>
    public void ResetToDefaults()
    {
        MasterVolume = DEFAULT_MASTER_VOLUME;
        MusicVolume = DEFAULT_MUSIC_VOLUME;
        SFXVolume = DEFAULT_SFX_VOLUME;
        Sensitivity = DEFAULT_SENSITIVITY;
        InvertY = DEFAULT_INVERT_Y;
    }

    /// <summary>
    /// Принудительно вызывает все события (для обновления UI)
    /// </summary>
    public void BroadcastCurrentSettings()
    {
        OnMasterVolumeChanged?.Invoke(_masterVolume);
        OnMusicVolumeChanged?.Invoke(_musicVolume);
        OnSFXVolumeChanged?.Invoke(_sfxVolume);
        OnSensitivityChanged?.Invoke(_sensitivity);
        OnInvertYChanged?.Invoke(_invertY);
    }
}
