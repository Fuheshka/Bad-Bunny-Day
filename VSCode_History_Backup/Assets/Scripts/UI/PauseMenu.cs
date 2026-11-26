using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenuUI;
    [SerializeField] private SettingsMenu _settingsMenu;
    private bool _isPaused = false;

 
    [SerializeField] private PlayerInput _playerInput;
    private InputAction _pauseAction;

    void Awake()
    {
        _pauseAction = _playerInput.actions["Pause"];
        _pauseAction.performed += OnPause;
    }


    private void OnEnable()
    {
        _pauseAction.Enable();
    }

    private void OnDisable()
    {
        _pauseAction.performed -= OnPause;
        _pauseAction.Disable();
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        // Проверяем, что это именно performed фаза и объект не уничтожен
        if (!context.performed) return;
        if (_playerInput == null) return;
        
        if (_isPaused) Resume();
        else Pause();
    }

    private void Pause()
    {
        if (_playerInput == null) return;
        
        _pauseMenuUI.SetActive(true);
        _playerInput.SwitchCurrentActionMap("UI");
        Time.timeScale = 0f;
        _isPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        if (_playerInput == null) return;
        
        _pauseMenuUI.SetActive(false);
        
        // Сначала включаем Gameplay action map, потом переключаемся
        var gameplayMap = _playerInput.actions.FindActionMap("Gameplay");
        if (gameplayMap != null)
        {
            gameplayMap.Enable();
            _playerInput.SwitchCurrentActionMap("Gameplay");
        }
        
        Time.timeScale = 1f;
        _isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void LoadMainMenu()
    {
        // Сбрасываем состояние перед сменой сцены
        Time.timeScale = 1f;
        _isPaused = false;
        
        // Возвращаем курсор к нормальному состоянию
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Уничтожаем Canvas (или родителя), чтобы он не остался после смены сцены
        // Если Canvas имеет DontDestroyOnLoad - это единственный способ от него избавиться
        if (_pauseMenuUI != null)
        {
            // Находим корневой Canvas и уничтожаем его
            Canvas rootCanvas = _pauseMenuUI.GetComponentInParent<Canvas>();
            if (rootCanvas != null && rootCanvas.transform.root != null)
            {
                Destroy(rootCanvas.transform.root.gameObject);
            }
        }

        SceneManager.LoadScene("MainMenu");
    }
    
    /// <summary>
    /// Открывает меню насOpenSettings( "Настройки")
    /// </summary>
    public void OpenSettings()
    {
        if (_settingsMenu != null)
        {
            _settingsMenu.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("SettingsMenu не назначен в PauseMenu!");
        }
    }
}
