using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Простой скрипт для кнопки рестарта.
/// Работает независимо от Input System через IPointerClickHandler.
/// Прикрепить к кнопке.
/// </summary>
public class RestartButton : MonoBehaviour, IPointerClickHandler
{
    [Header("Настройки")]
    [Tooltip("Кнопка для рестарта (если не назначена - ищет на этом объекте)")]
    [SerializeField] private Button restartButton;
    
    [Tooltip("Клавиша для рестарта (дополнительно к кнопке)")]
    public KeyCode restartKey = KeyCode.R;
    
    [Tooltip("Работать только когда Game Over или Level Complete")]
    public bool onlyOnGameOver = true;

    void Start()
    {
        // Автоматически находим кнопку
        if (restartButton == null)
        {
            restartButton = GetComponent<Button>();
        }

        // Подписываемся на клик через Button компонент
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(OnRestartClicked);
            Debug.Log("[RestartButton] Кнопка подключена: " + restartButton.name);
        }
        else
        {
            Debug.LogWarning("[RestartButton] Button компонент не найден! Используем IPointerClickHandler.");
        }
    }

    void Update()
    {
        // Проверяем нажатие клавиши R (работает через старый Input)
        if (Input.GetKeyDown(restartKey))
        {
            bool canRestart = !onlyOnGameOver || 
                (GameManager.Instance != null && (GameManager.Instance.IsGameOver() || GameManager.Instance.IsLevelComplete()));
            
            if (canRestart)
            {
                Debug.Log("[RestartButton] Нажата клавиша " + restartKey);
                DoRestart();
            }
        }
    }

    // IPointerClickHandler - работает напрямую через EventSystem
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("[RestartButton] OnPointerClick вызван!");
        OnRestartClicked();
    }

    void OnRestartClicked()
    {
        Debug.Log("[RestartButton] Кнопка нажата!");
        
        bool canRestart = !onlyOnGameOver || 
            (GameManager.Instance != null && (GameManager.Instance.IsGameOver() || GameManager.Instance.IsLevelComplete()));
        
        if (canRestart)
        {
            DoRestart();
        }
        else
        {
            Debug.Log("[RestartButton] Рестарт заблокирован - игра ещё идёт");
        }
    }

    void DoRestart()
    {
        Debug.Log("[RestartButton] Перезапуск уровня...");
        
        // Восстанавливаем время
        Time.timeScale = 1f;
        
        // Перезагружаем сцену
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnDestroy()
    {
        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(OnRestartClicked);
        }
    }
}
