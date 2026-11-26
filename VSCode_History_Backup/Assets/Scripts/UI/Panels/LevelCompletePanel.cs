using UnityEngine;
using TMPro;
using CDB.Core;
using CDB.Core.GameStates;

namespace CDB.UI
{
    /// <summary>
    /// Панель завершения уровня.
    /// Показывается когда время уровня истекло.
    /// </summary>
    public class LevelCompletePanel : UIPanel
    {
        [Header("UI Элементы")]
        [SerializeField] private TextMeshProUGUI _finalScoreText;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private string _scoreFormat = "Счёт: {0}";

        private void OnEnable()
        {
            GameStateManager.OnLevelComplete += OnLevelComplete;
        }

        private void OnDisable()
        {
            GameStateManager.OnLevelComplete -= OnLevelComplete;
        }

        private void OnLevelComplete()
        {
            UpdateScore();
            Show();
        }

        protected override void OnShow()
        {
            base.OnShow();
            UpdateScore();
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void UpdateScore()
        {
            if (_finalScoreText != null && ScoreManager.Instance != null)
            {
                int score = ScoreManager.Instance.GetTotalScore();
                _finalScoreText.text = string.Format(_scoreFormat, score);
            }
        }

        public void Restart()
        {
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.RestartLevel();
            }
            else
            {
                Time.timeScale = 1f;
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
            }
        }

        public void LoadMainMenu()
        {
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.LoadMainMenu();
            }
            else
            {
                Time.timeScale = 1f;
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            }
        }

        public void NextLevel()
        {
            // Реализация загрузки следующего уровня
            Debug.Log("Next level - not implemented");
        }
    }
}
