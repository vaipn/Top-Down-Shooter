using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
	public Player player;


	[Header("Settings")]
	public bool friendlyFire;
	private void Awake()
	{
		instance = this;

		player = FindObjectOfType<Player>();
	}

	public void GameStart()
	{
		SetDefaultWeaponsForPlayer();
		LevelGenerator.instance.InitializeGeneration();

		// we start selected mission in LevelGenerator script, after levels have finished generating
	}

	private void SetDefaultWeaponsForPlayer()
	{
		List<WeaponData> newWeaponsList = UI.instance.weaponSelection.SelectedWeaponData();
		player.weaponController.SetDefaultWeapon(newWeaponsList);
	}
}
