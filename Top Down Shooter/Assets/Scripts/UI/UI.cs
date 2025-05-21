using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    public static UI instance;

    public UI_Ingame inGameUI {  get; private set; }
	public UI_WeaponSelection weaponSelection { get; private set; }

	[SerializeField] private GameObject[] UIElements;

	private void Awake()
	{
		instance = this;
		inGameUI = GetComponentInChildren<UI_Ingame>(true);
		weaponSelection = GetComponentInChildren<UI_WeaponSelection>(true);
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
}
