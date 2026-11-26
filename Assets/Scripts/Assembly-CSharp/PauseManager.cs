using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
	[SerializeField]
	private GameObject _pausePanel;

	[SerializeField]
	private GameObject _tutorialPanel;

	[SerializeField]
	private GameObject _settingsPanel;

	[SerializeField]
	private PlayerInput _playerInput;

	private InputAction _pauseAction;

	private bool _isPaused;

	private void Start()
	{
		Time.timeScale = 0f;
		_pauseAction = _playerInput.actions["Pause"];
		_pauseAction.performed += (InputAction.CallbackContext ctx) =>
		{
			CloseTutorial();
		};
	}

	public void CloseTutorial()
	{
		Time.timeScale = 1f;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		Object.Destroy(_tutorialPanel);
		_pauseAction.performed += (InputAction.CallbackContext ctx) =>
		{
			PauseGame();
		};
	}

	public void PauseGame()
	{
		_pausePanel.SetActive(value: true);
		Time.timeScale = 0f;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		_isPaused = true;
	}

	public void ResumeGame()
	{
		_pausePanel.SetActive(value: false);
		_settingsPanel.SetActive(value: false);
		Time.timeScale = 1f;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		_isPaused = true;
	}
}
