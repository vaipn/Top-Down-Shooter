using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI instance;

    public UI_Ingame inGameUI {  get; private set; }
	public UI_WeaponSelection weaponSelection { get; private set; }
	public UI_GameOver gameOverUI { get; private set; }
	public UI_Settings settingsUI { get; private set; }
	public GameObject victoryScreenUI;
	public GameObject pauseUI;

	[SerializeField] private GameObject[] UIElements;


	[Header("Fade Image")]
	[SerializeField] private Image fadeImage;
	private void Awake()
	{
		instance = this;
		inGameUI = GetComponentInChildren<UI_Ingame>(true);
		weaponSelection = GetComponentInChildren<UI_WeaponSelection>(true);
		gameOverUI = GetComponentInChildren<UI_GameOver>(true);
		settingsUI = GetComponentInChildren<UI_Settings>(true);
	}
	private void Start()
	{
		AssignInputsUI();

		StartCoroutine(ChangeImageAlpha(0, 1.5f, null));

		settingsUI.LoadSettings();
	}
	public void SwitchTo(GameObject uiToSwitchOn)
	{
		foreach (GameObject go in UIElements)
			go.SetActive(false);

		uiToSwitchOn.SetActive(true);

		if (uiToSwitchOn == settingsUI.gameObject)
			settingsUI.LoadSettings();
	}

	public void StartTheGame() => StartCoroutine(StartGameSequence());
	public void QuitTheGame() => Application.Quit();

	public void RestartTheGame()
	{
		StartCoroutine(ChangeImageAlpha(1, 1, GameManager.instance.RestartScene));
	}

	public void StartLevelGeneration() => LevelGenerator.instance.InitializeGeneration();

	public void PauseSwitch()
	{
		bool gamePaused = pauseUI.activeSelf;

		if (gamePaused)
		{
			SwitchTo(inGameUI.gameObject);
			ControlsManager.instance.SwitchToCharacterControls();
			TimeManager.instance.ResumeTime();
			Cursor.visible = false;
		}
		else
		{
			SwitchTo(pauseUI);
			ControlsManager.instance.SwitchToUIControls();
			TimeManager.instance.PauseTime();
			Cursor.visible = true;
		}
	}

	public void ShowGameOverUI(string message = "Game Over")
	{
		SwitchTo(gameOverUI.gameObject);
		gameOverUI.ShowGameOverMessage(message);
	}
	public void ShowVictoryScreenUI()
	{
		StartCoroutine(ChangeImageAlpha(1, 1.5f, SwitchToVictoryScreenUI));
	}
	private void SwitchToVictoryScreenUI()
	{
		SwitchTo(victoryScreenUI);

		// because the screen would still be black when we switch to victoryScreenUI
		Color color = fadeImage.color;
		color.a = 0;

		fadeImage.color = color;
	}
	private void AssignInputsUI()
	{
		PlayerControls controls = GameManager.instance.player.controls;

		controls.UI.UIPause.performed += ctx => PauseSwitch();
	}

	private IEnumerator StartGameSequence()
	{
		StartCoroutine(ChangeImageAlpha(1, 1, null));

		yield return new WaitForSeconds(1); // duration of fade (ChangeImageAlpha)
		SwitchTo(inGameUI.gameObject);
		GameManager.instance.GameStart();

		StartCoroutine(ChangeImageAlpha(0, 1, null));
	}

	private IEnumerator ChangeImageAlpha(float targetAlpha, float duration, System.Action onComplete)
	{
		float time = 0;
		Color currentColor = fadeImage.color;
		float startAlpha = currentColor.a;

		while (time < duration)
		{
			time += Time.deltaTime;
			float alpha = Mathf.Lerp(startAlpha, targetAlpha, time/duration);

			fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
			yield return null; // so the loop runs once per frame
		}

		fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);

		// call the completion method if it exists
		onComplete?.Invoke();
	}

	[ContextMenu("Assign audio to buttons")]
	public void AssignAudioToButtons()
	{
		UI_Button[] buttons = FindObjectsOfType<UI_Button>(true);

		foreach (UI_Button button in buttons)
		{
			button.AssignAudioSource();
		}
	}
}
