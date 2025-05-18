using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    public static UI instance;

    public UI_Ingame inGameUI {  get; private set; }

	private void Awake()
	{
		instance = this;
		inGameUI = GetComponentInChildren<UI_Ingame>();
	}
}
