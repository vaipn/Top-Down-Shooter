using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    public static UI instance;

    public UI_Ingame inGameUI {  get; private set; }
	public UI_WeaponSelection weaponSelection { get; private set; }

	public GameObject pauseUI;

	[SerializeField] private GameObject[] UIElements;

	private void Awake()
	{
		instance = this;
		inGameUI = GetComponentInChildren<UI_Ingame>(true);
		weaponSelection = GetComponentInChildren<UI_WeaponSelection>(true);
	}
	private void Start()
	{
		AssignInputsUI();
	}
	public void SwitchTo(GameObject uiToSwitchOn)
	{
		foreach (GameObject go in UIElements)
			go.SetActive(false);

		uiToSwitchOn.SetActive(true);
	}

	public void StartTheGame()
	{
		SwitchTo(inGameUI.gameObject);
		GameManager.instance.GameStart();
	}
	public void QuitTheGame() => Application.Quit();

	public void RestartTheGame()
	{
		GameManager.instance.RestartScene();
	}

	public void PauseSwitch()
	{
		bool gamePaused = pauseUI.activeSelf;

		if (gamePaused)
		{
			SwitchTo(inGameUI.gameObject);
			ControlsManager.instance.SwitchToCharacterControls();
			TimeManager.instance.ResumeTime();
		}
		else
		{
			SwitchTo(pauseUI);
			ControlsManager.instance.SwitchToUIControls();
			TimeManager.instance.PauseTime();
		}
	}

	private void AssignInputsUI()
	{
		PlayerControls controls = GameManager.instance.player.controls;

		controls.UI.UIPause.performed += ctx => PauseSwitch();
	}
}
