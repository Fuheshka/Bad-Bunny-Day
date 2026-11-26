using UnityEngine;
using UnityEngine.InputSystem;
using CDB.Core;
using CDB.Core.GameStates;

namespace CDB.UI
{
    /// <summary>
    /// Панель паузы.
    /// Управляет паузой игры и показом меню паузы.
    /// </summary>
    public class PausePanel : UIPanel
    {
        [Header("Ссылки")]
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private SettingsMenu _settingsMenu;

        private InputAction _pauseAction;
        private bool _isPaused;

        private void Awake()
        {
            if (_playerInput != null)
            {
                _pauseAction = _playerInput.actions["Pause"];
                _pauseAction.performed += OnPause;
            }
        }

        private void OnEnable()
        {
            _pauseAction?.Enable();
            GameStateManager.OnGameStateChanged += OnGameStateChanged;
        }

        private void OnDisable()
        {
            if (_pauseAction != null)
            {
                _pauseAction.performed -= OnPause;
                _pauseAction.Disable();
            }
            GameStateManager.OnGameStateChanged -= OnGameStateChanged;
        }

        private void OnPause(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            if (_playerInput == null) return;
            
            if (_isPaused)
                Resume();
            else
                Pause();
        }

        private void OnGameStateChanged(GameState newState)
        {
            _isPaused = newState == GameState.Paused;
        }

        public void Pause()
        {
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.Pause();
            }
            else
            {
                Time.timeScale = 0f;
            }
            
            _isPaused = true;
            Show();
            
            if (_playerInput != null)
                _playerInput.SwitchCurrentActionMap("UI");
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void Resume()
        {
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.Resume();
            }
            else
            {
                Time.timeScale = 1f;
            }
            
            _isPaused = false;
            Hide();
            
            if (_playerInput != null)
            {
                var gameplayMap = _playerInput.actions.FindActionMap("Gameplay");
                if (gameplayMap != null)
                {
                    gameplayMap.Enable();
                    _playerInput.SwitchCurrentActionMap("Gameplay");
                }
            }
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void LoadMainMenu()
        {
            Time.timeScale = 1f;
            _isPaused = false;
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.LoadMainMenu();
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            }
        }

        public void OpenSettings()
        {
            if (_settingsMenu != null)
                _settingsMenu.OpenSettings();
        }
    }
}
