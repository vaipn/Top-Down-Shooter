using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
		//LevelGenerator.instance.InitializeGeneration();

		// we start selected mission in LevelGenerator script, after levels have finished generating
	}

	public void RestartScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

	public void GameOver()
	{
		TimeManager.instance.SlowMotion(1.5f);
		UI.instance.ShowGameOverUI();
		CameraManager.instance.ChangeCameraDistance(5);
	}

	public void LevelCompleted()
	{
		UI.instance.ShowVictoryScreenUI();
		ControlsManager.instance.controls.Character.Disable();
		player.health.currentHealth += 999999;
	}

	private void SetDefaultWeaponsForPlayer()
	{
		List<WeaponData> newWeaponsList = UI.instance.weaponSelection.SelectedWeaponData();
		player.weaponController.SetDefaultWeapon(newWeaponsList);
	}
}
