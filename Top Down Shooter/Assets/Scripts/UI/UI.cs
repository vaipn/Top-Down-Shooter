using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    public static UI instance;

    public UI_Ingame inGameUI {  get; private set; }

	[SerializeField] private GameObject[] UIElements;

	private void Awake()
	{
		instance = this;
		inGameUI = GetComponentInChildren<UI_Ingame>(true);
	}

	public void SwitchTo(GameObject uiToSwitchOn)
	{
		foreach (GameObject go in UIElements)
			go.SetActive(false);

		uiToSwitchOn.SetActive(true);
	}

	public void SwitchToInGameUI() => SwitchTo(inGameUI.gameObject);
	public void QuitTheGame() => Application.Quit();
}
