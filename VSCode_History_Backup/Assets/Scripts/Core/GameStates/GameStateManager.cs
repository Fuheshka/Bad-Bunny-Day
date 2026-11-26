using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using CDB.Core.GameStates;

namespace CDB.Core
{
    /// <summary>
    /// Менеджер состояний игры.
    /// Управляет переходами между состояниями и уведомляет подписчиков.
    /// </summary>
    public class GameStateManager : Singleton<GameStateManager>
    {
        [Header("Настройки")]
        [SerializeField] private float _gameOverDelay = 2.5f;
        [SerializeField] private float _levelCompleteDelay = 1f;

        public static event Action<GameState> OnGameStateChanged;
        public static event Action OnGameOver;
        public static event Action OnLevelComplete;
        public static event Action OnGamePaused;
        public static event Action OnGameResumed;

        private GameState _currentState = GameState.MainMenu;
        public GameState CurrentState => _currentState;

        public bool IsPlaying => _currentState == GameState.Playing;
        public bool IsPaused => _currentState == GameState.Paused;
        public bool IsGameOver => _currentState == GameState.GameOver;
        public bool IsLevelComplete => _currentState == GameState.LevelComplete;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            // Если мы на игровой сцене, переходим в Playing
            if (SceneManager.GetActiveScene().name != "MainMenu")
            {
                SetState(GameState.Playing);
            }
        }

        /// <summary>
        /// Установить новое состояние игры
        /// </summary>
        public void SetState(GameState newState)
        {
            if (_currentState == newState) return;
            
            GameState previousState = _currentState;
            _currentState = newState;
            
            HandleStateTransition(previousState, newState);
            OnGameStateChanged?.Invoke(newState);
        }

        private void HandleStateTransition(GameState from, GameState to)
        {
            switch (to)
            {
                case GameState.Playing:
                    Time.timeScale = 1f;
                    break;
                    
                case GameState.Paused:
                    Time.timeScale = 0f;
                    OnGamePaused?.Invoke();
                    break;
                    
                case GameState.GameOver:
                    OnGameOver?.Invoke();
                    break;
                    
                case GameState.LevelComplete:
                    OnLevelComplete?.Invoke();
                    break;
            }
        }

        /// <summary>
        /// Начать игру (с задержкой для Game Over)
        /// </summary>
        public void TriggerGameOver()
        {
            if (_currentState == GameState.GameOver) return;
            StartCoroutine(GameOverRoutine());
        }

        private System.Collections.IEnumerator GameOverRoutine()
        {
            yield return new WaitForSeconds(_gameOverDelay);
            SetState(GameState.GameOver);
        }

        /// <summary>
        /// Завершить уровень (с задержкой)
        /// </summary>
        public void TriggerLevelComplete()
        {
            if (_currentState == GameState.GameOver || _currentState == GameState.LevelComplete) return;
            StartCoroutine(LevelCompleteRoutine());
        }

        private System.Collections.IEnumerator LevelCompleteRoutine()
        {
            yield return new WaitForSeconds(_levelCompleteDelay);
            SetState(GameState.LevelComplete);
        }

        /// <summary>
        /// Поставить игру на паузу
        /// </summary>
        public void Pause()
        {
            if (_currentState != GameState.Playing) return;
            SetState(GameState.Paused);
        }

        /// <summary>
        /// Снять игру с паузы
        /// </summary>
        public void Resume()
        {
            if (_currentState != GameState.Paused) return;
            SetState(GameState.Playing);
            Time.timeScale = 1f;
            OnGameResumed?.Invoke();
        }

        /// <summary>
        /// Перезапустить текущий уровень
        /// </summary>
        public void RestartLevel()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        /// <summary>
        /// Загрузить главное меню
        /// </summary>
        public void LoadMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }

        /// <summary>
        /// Загрузить сцену по имени
        /// </summary>
        public void LoadScene(string sceneName)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// Выйти из игры
        /// </summary>
        public void QuitGame()
        {
            Debug.Log("Игра закрыта");
            Application.Quit();
            
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }
}
