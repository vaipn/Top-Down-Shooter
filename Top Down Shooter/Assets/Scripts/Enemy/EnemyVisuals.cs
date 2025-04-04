using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyMelee_WeaponType { OneHand, Throw, Unarmed, TwoHand}
public enum Enemy_WeaponName { Axe1, Axe2, Pipe, Hammer, Wrench, Unarmed, Pistol, Revolver, Shotgun, AutoRifle, Sniper}
public enum EnemyRange_WeaponType { Pistol, Revolver, Shotgun, AutoRifle, Sniper}

public class EnemyVisuals : MonoBehaviour
{
	[Header("Weapon visuals")]
	[SerializeField] private EnemyHeldWeaponModel[] heldWeaponModels;
	[SerializeField] private EnemySheathedWeaponModel[] sheathedWeaponModels;

	[Header("Color")]
	[SerializeField] private Texture[] colorTextures;
	[SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

	[Header("Corruption crystal visuals")]
	[SerializeField] private GameObject[] corruptionCrystals;
	[SerializeField] private int corruptionAmount;
	
	
	public EnemyHeldWeaponModel currentHeldWeaponModel {  private set; get; }
	public EnemySheathedWeaponModel currentSheathedWeaponModel { private set; get; }

	private void Awake()
	{
		//InvokeRepeating(nameof(SetupLook), 1, 2);

		heldWeaponModels = GetComponentsInChildren<EnemyHeldWeaponModel>(true);
		sheathedWeaponModels = GetComponentsInChildren<EnemySheathedWeaponModel>(true);
	}

	private GameObject[] CollectCorruptionCrystals()
	{
		EnemyCorruptionCrystal[] enemyCorruptionCrystals = GetComponentsInChildren<EnemyCorruptionCrystal>(true);
		GameObject[] corruptionCrystals = new GameObject[enemyCorruptionCrystals.Length];

		for (int i = 0; i < enemyCorruptionCrystals.Length; i++)
		{
			corruptionCrystals[i] = enemyCorruptionCrystals[i].gameObject;
		}

		return corruptionCrystals;
	}

	public void EnableWeaponTrail(bool enable)
	{
		currentHeldWeaponModel.EnableTrailEffect(enable);
	}

	public void SetupLook()
	{
		SetupRandomColor();
		SetupRandomWeapon();
		SetupRandomCorruption();
	}
	
	private void SetupRandomWeapon()
	{
		foreach (var weaponModel in heldWeaponModels)
		{
			weaponModel.gameObject.SetActive(false);
		}
		foreach (var weaponModel in sheathedWeaponModels)
			weaponModel.gameObject.SetActive(false);


		bool thisEnemyIsMelee = GetComponent<EnemyMelee>() != null;
		bool thisEnemyIsRange = GetComponent<EnemyRange>() != null;

		if (thisEnemyIsMelee)
			currentHeldWeaponModel = FindMeleeWeaponModel();
		if (thisEnemyIsRange)
			currentHeldWeaponModel = FindRangeWeaponModel();

		foreach (var weaponModel in sheathedWeaponModels)
		{
			if (thisEnemyIsMelee)
			{
				if (weaponModel.weaponName == currentHeldWeaponModel.weaponName)
				{
					currentSheathedWeaponModel = weaponModel;
					currentSheathedWeaponModel.gameObject.SetActive(true);
				}
			}
			
			else if (thisEnemyIsRange)
			{
				if (weaponModel.weaponName == currentHeldWeaponModel.weaponName)
				{
					currentSheathedWeaponModel = weaponModel;
					currentSheathedWeaponModel.gameObject.SetActive(true);
				}
			}
		}

		OverrideAnimatorControllerIfCan();
		//currentHeldWeaponModel.gameObject.SetActive(true);

	}

	private EnemyHeldWeaponModel FindRangeWeaponModel()
	{
		EnemyRangeWeaponModel[] weaponModels = GetComponentsInChildren<EnemyRangeWeaponModel>(true);
		EnemyRange_WeaponType weaponType = GetComponent<EnemyRange>().weaponType;

		foreach (var weaponModel in weaponModels)
		{
			if (weaponModel.weaponType == weaponType)
				return weaponModel.gameObject.GetComponent<EnemyHeldWeaponModel>();
		}

		Debug.LogWarning("No range weapon model found");
		return null;
	}

	private EnemyHeldWeaponModel FindMeleeWeaponModel()
	{
		EnemyMelee_WeaponType weaponType = GetComponent<EnemyMelee>().weaponType;
		List<EnemyHeldWeaponModel> filteredHeldWeaponModels = new List<EnemyHeldWeaponModel>();

		foreach (var weaponModel in heldWeaponModels)
		{
			if (weaponModel.meleeWeaponType == weaponType)
				filteredHeldWeaponModels.Add(weaponModel);
		}

		int randomIndex = Random.Range(0, filteredHeldWeaponModels.Count);

		return filteredHeldWeaponModels[randomIndex];
	}

	private void SetupRandomColor()
	{
		int randomIndex = Random.Range(0, colorTextures.Length);

		Material newMaterial = new Material(skinnedMeshRenderer.material);

		newMaterial.mainTexture = colorTextures[randomIndex];

		skinnedMeshRenderer.material = newMaterial;
	}

	private void SetupRandomCorruption()
	{
		List<int> availableIndexes = new List<int>();
		corruptionCrystals = CollectCorruptionCrystals();

		for (int i = 0; i < corruptionCrystals.Length; i++)
		{
			availableIndexes.Add(i);
			corruptionCrystals[i].SetActive(false);
		}

		for (int i = 0; i < corruptionAmount; i++)
		{
			if (availableIndexes.Count == 0)
				break;

			int randomIndex = Random.Range(0, availableIndexes.Count);
			int objectIndex = availableIndexes[randomIndex];

			corruptionCrystals[objectIndex].SetActive(true);
			availableIndexes.RemoveAt(randomIndex);
		}
	}

	private void OverrideAnimatorControllerIfCan()
	{
		AnimatorOverrideController overrideController = currentHeldWeaponModel.overrideController;

		if (overrideController != null)
		{
			GetComponentInChildren<Animator>().runtimeAnimatorController = overrideController;
		}
	}
}
