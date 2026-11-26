using UnityEngine;

/// <summary>
/// Управляет видимостью Waypoint через настройки.
/// Повесь на объект со скриптом Waypoint.cs
/// </summary>
public class WaypointController : MonoBehaviour
{
    [SerializeField] private GameObject waypointVisuals; // Перетащи сюда объект со стрелкой/UI

    void OnEnable()
    {
        SettingsManager.OnShowWaypointChanged += OnWaypointSettingChanged;
        
        // Загрузить начальное значение
        if (SettingsManager.Instance != null)
            ApplyWaypointVisibility(SettingsManager.Instance.ShowWaypoint);
    }

    void OnDisable()
    {
        SettingsManager.OnShowWaypointChanged -= OnWaypointSettingChanged;
    }

    private void OnWaypointSettingChanged(bool show)
    {
        ApplyWaypointVisibility(show);
    }

    private void ApplyWaypointVisibility(bool show)
    {
        if (waypointVisuals != null)
            waypointVisuals.SetActive(show);
        
        // Или можно отключать весь объект:
        // gameObject.SetActive(show);
    }
}
