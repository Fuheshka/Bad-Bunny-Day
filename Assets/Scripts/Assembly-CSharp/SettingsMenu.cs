using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI-контроллер для меню настроек.
/// Привязывается к Canvas с панелью настроек.
/// </summary>
public class SettingsMenu : MonoBehaviour
{
    [Header("Панель настроек")]
    [SerializeField] private GameObject _settingsPanel;

    [Header("Вкладки (Tab Buttons)")]
    [SerializeField] private Button _tabAudio;
    [SerializeField] private Button _tabControls;
    [SerializeField] private Button _tabGame;
    [SerializeField] private Button _tabGraphics;

    [Header("Панели вкладок")]
    [SerializeField] private GameObject _panelAudio;
    [SerializeField] private GameObject _panelControls;
    [SerializeField] private GameObject _panelGame;
    [SerializeField] private GameObject _panelGraphics;

    [Header("Звук - Слайдеры")]
    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;

    [Header("Звук - Метки значений")]
    [SerializeField] private TextMeshProUGUI _masterVolumeLabel;
    [SerializeField] private TextMeshProUGUI _musicVolumeLabel;
    [SerializeField] private TextMeshProUGUI _sfxVolumeLabel;

    [Header("Управление - Слайдеры")]
    [SerializeField] private Slider _sensitivitySlider;
    [SerializeField] private Toggle _invertYToggle;

    [Header("Управление - Метки")]
    [SerializeField] private TextMeshProUGUI _sensitivityLabel;

    [Header("Кнопки")]
    [SerializeField] private Button _resetButton;
    [SerializeField] private Button _closeButton;

    private void Start()
    {
        // Подписка на кнопки вкладок
        if (_tabAudio != null) _tabAudio.onClick.AddListener(() => SwitchTab(0));
        if (_tabControls != null) _tabControls.onClick.AddListener(() => SwitchTab(1));
        if (_tabGame != null) _tabGame.onClick.AddListener(() => SwitchTab(2));
        if (_tabGraphics != null) _tabGraphics.onClick.AddListener(() => SwitchTab(3));

        // Подписка на слайдеры (мгновенное применение)
        if (_masterVolumeSlider != null)
            _masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);

        if (_musicVolumeSlider != null)
            _musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

        if (_sfxVolumeSlider != null)
            _sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        if (_sensitivitySlider != null)
            _sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);

        if (_invertYToggle != null)
            _invertYToggle.onValueChanged.AddListener(OnInvertYChanged);

        // Кнопки
        if (_resetButton != null)
            _resetButton.onClick.AddListener(OnResetClicked);

        if (_closeButton != null)
            _closeButton.onClick.AddListener(CloseSettings);

        // Загружаем текущие значения в UI
        LoadCurrentValuesToUI();

        // Открываем первую вкладку по умолчанию
        SwitchTab(0);
    }

    private void OnEnable()
    {
        LoadCurrentValuesToUI();
    }

    /// <summary>
    /// Переключает видимую вкладку настроек
    /// </summary>
    public void SwitchTab(int tabIndex)
    {
        // Скрываем все панели
        if (_panelAudio != null) _panelAudio.SetActive(false);
        if (_panelControls != null) _panelControls.SetActive(false);
        if (_panelGame != null) _panelGame.SetActive(false);
        if (_panelGraphics != null) _panelGraphics.SetActive(false);

        // Показываем нужную
        switch (tabIndex)
        {
            case 0:
                if (_panelAudio != null) _panelAudio.SetActive(true);
                break;
            case 1:
                if (_panelControls != null) _panelControls.SetActive(true);
                break;
            case 2:
                if (_panelGame != null) _panelGame.SetActive(true);
                break;
            case 3:
                if (_panelGraphics != null) _panelGraphics.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// Загружает текущие настройки из SettingsManager в UI-элементы
    /// </summary>
    private void LoadCurrentValuesToUI()
    {
        if (SettingsManager.Instance == null) return;

        // Аудио
        if (_masterVolumeSlider != null)
        {
            _masterVolumeSlider.SetValueWithoutNotify(SettingsManager.Instance.MasterVolume);
            UpdateVolumeLabel(_masterVolumeLabel, SettingsManager.Instance.MasterVolume);
        }

        if (_musicVolumeSlider != null)
        {
            _musicVolumeSlider.SetValueWithoutNotify(SettingsManager.Instance.MusicVolume);
            UpdateVolumeLabel(_musicVolumeLabel, SettingsManager.Instance.MusicVolume);
        }

        if (_sfxVolumeSlider != null)
        {
            _sfxVolumeSlider.SetValueWithoutNotify(SettingsManager.Instance.SFXVolume);
            UpdateVolumeLabel(_sfxVolumeLabel, SettingsManager.Instance.SFXVolume);
        }

        // Управление
        if (_sensitivitySlider != null)
        {
            _sensitivitySlider.SetValueWithoutNotify(SettingsManager.Instance.Sensitivity);
            UpdateSensitivityLabel(SettingsManager.Instance.Sensitivity);
        }

        if (_invertYToggle != null)
        {
            _invertYToggle.SetIsOnWithoutNotify(SettingsManager.Instance.InvertY);
        }
    }

    // === Обработчики изменений слайдеров ===

    private void OnMasterVolumeChanged(float value)
    {
        if (SettingsManager.Instance != null)
            SettingsManager.Instance.MasterVolume = value;
        UpdateVolumeLabel(_masterVolumeLabel, value);
    }

    private void OnMusicVolumeChanged(float value)
    {
        if (SettingsManager.Instance != null)
            SettingsManager.Instance.MusicVolume = value;
        UpdateVolumeLabel(_musicVolumeLabel, value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        if (SettingsManager.Instance != null)
            SettingsManager.Instance.SFXVolume = value;
        UpdateVolumeLabel(_sfxVolumeLabel, value);
    }

    private void OnSensitivityChanged(float value)
    {
        if (SettingsManager.Instance != null)
            SettingsManager.Instance.Sensitivity = value;
        UpdateSensitivityLabel(value);
    }

    private void OnInvertYChanged(bool value)
    {
        if (SettingsManager.Instance != null)
            SettingsManager.Instance.InvertY = value;
    }

    // === Кнопки ===

    private void OnResetClicked()
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.ResetToDefaults();
            LoadCurrentValuesToUI();
        }
    }

    public void OpenSettings()
    {
        if (_settingsPanel != null)
        {
            _settingsPanel.SetActive(true);
            LoadCurrentValuesToUI();
        }
    }

    public void CloseSettings()
    {
        if (_settingsPanel != null)
            _settingsPanel.SetActive(false);
    }

    // === Вспомогательные методы ===

    private void UpdateVolumeLabel(TextMeshProUGUI label, float value)
    {
        if (label != null)
            label.text = Mathf.RoundToInt(value * 100f) + "%";
    }

    private void UpdateSensitivityLabel(float value)
    {
        if (_sensitivityLabel != null)
            _sensitivityLabel.text = value.ToString("F2");
    }
}
