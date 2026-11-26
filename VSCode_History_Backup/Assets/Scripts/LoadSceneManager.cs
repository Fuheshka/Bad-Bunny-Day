using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadSceneManager : MonoBehaviour
{
    [SerializeField] private SettingsMenu _settingsMenu;
    
    public void StartGame()
    {
        SceneManager.LoadScene("Level1"); 
    }

    public void ExitGame()
    {
        Debug.Log("Игра закрыта");
        Application.Quit();
    }
    
    public void OpenSettings()
    {
        if (_settingsMenu != null)
        {
            _settingsMenu.OpenSettings();
        }
        else
        {
            Debug.LogWarning("SettingsMenu не назначен в LoadSceneManager!");
        }
    }
}
