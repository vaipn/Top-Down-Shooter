using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyMelee_WeaponType { OneHand, Throw}
public enum EnemyMelee_WeaponName { Axe1, Axe2, Pipe, Hammer, Wrench}

public class EnemyVisuals : MonoBehaviour
{
	[Header("Color")]
	[SerializeField] private Texture[] colorTextures;
	[SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

	[Header("Corruption crystal visuals")]
	[SerializeField] private GameObject[] corruptionCrystals;
	[SerializeField] private int corruptionAmount;

	[Header("Weapon visuals")]
	[SerializeField] private EnemyHeldWeaponModel[] heldWeaponModels;
	[SerializeField] private EnemySheathedWeaponModel[] sheathedWeaponModels;
	[SerializeField] private EnemyMelee_WeaponType weaponType;
	public EnemyHeldWeaponModel currentHeldWeaponModel {  private set; get; }
	public EnemySheathedWeaponModel currentSheathedWeaponModel { private set; get; }

	private void Awake()
	{
		//InvokeRepeating(nameof(SetupLook), 1, 2);

		heldWeaponModels = GetComponentsInChildren<EnemyHeldWeaponModel>(true);
		sheathedWeaponModels = GetComponentsInChildren<EnemySheathedWeaponModel>(true);

		CollectCorruptionCrystals();
	}

	public void SetupWeaponType(EnemyMelee_WeaponType type) => weaponType = type;
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

		List<EnemyHeldWeaponModel> filteredHeldWeaponModels = new List<EnemyHeldWeaponModel>();

		foreach (var weaponModel in heldWeaponModels)
		{
			if (weaponModel.weaponType == weaponType)
				filteredHeldWeaponModels.Add(weaponModel);
		}

		int randomIndex = Random.Range(0, filteredHeldWeaponModels.Count);

		currentHeldWeaponModel = filteredHeldWeaponModels[randomIndex];

		foreach (var weaponModel in sheathedWeaponModels)
		{
			if (weaponModel.weaponName == currentHeldWeaponModel.weaponName)
			{
				currentSheathedWeaponModel = weaponModel;
				currentSheathedWeaponModel.gameObject.SetActive(true);
			}
		}

		//currentHeldWeaponModel.gameObject.SetActive(true);
		
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

	private void CollectCorruptionCrystals()
	{
		EnemyCorruptionCrystal[] enemyCorruptionCrystals = GetComponentsInChildren<EnemyCorruptionCrystal>(true);
		corruptionCrystals = new GameObject[enemyCorruptionCrystals.Length];

		for (int i = 0; i < enemyCorruptionCrystals.Length; i++)
		{
			corruptionCrystals[i] = enemyCorruptionCrystals[i].gameObject;
		}
	}
}
