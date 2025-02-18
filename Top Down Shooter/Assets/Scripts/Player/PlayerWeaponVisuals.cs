using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerWeaponVisuals : MonoBehaviour
{
	private Player player;

	private Animator animator;
	private bool isEquipingWeapon;


	[SerializeField] private WeaponModel[] weaponModels;
	[SerializeField] private BackupWeaponModel[] backupWeaponModels;

	[Header("Rig")]
	[SerializeField] private float rigWeightIncreaseRate;
	private bool shouldIncreaseRigWeight;
	private Rig rig;


	[Header("Left hand IK")]
	[SerializeField] private Transform leftHandTarget;

	private void Start()
	{
		animator = GetComponentInChildren<Animator>();
		rig = GetComponentInChildren<Rig>();
		weaponModels = GetComponentsInChildren<WeaponModel>(true);
		backupWeaponModels = GetComponentsInChildren<BackupWeaponModel>(true);

		player = GetComponent<Player>();

		//SwitchAnimationLayer(1);
	}

	private void Update()
	{
		IncreaseRigWeight();
	}

	public void PlayReloadAnimation()
	{
		if (isEquipingWeapon)
			return;

		animator.SetTrigger("Reload");
		ReduceRigWeight();
	}

	public void PlayWeaponEquipAnimation()
	{
		EquipType equipType = CurrentWeaponModel().equipType;

		ReduceRigWeight();
		animator.SetFloat("Weapon Equip Type", (float)equipType);
		animator.SetTrigger("Equip Weapon");

		SetIsEquipingWeaponTo(true);
	}

	public void FinishedEquipingWeapon()
	{
		SetIsEquipingWeaponTo(false);
	}

	private void SetIsEquipingWeaponTo(bool busy)
	{
		isEquipingWeapon = busy;
		animator.SetBool("isEquipingWeapon", isEquipingWeapon);
	}

	public void SwitchOnCurrentWeaponModelObject()
	{
		SwitchOffWeaponModelsObjects();

		SwitchOffBackupWeaponModelsObjects();

		if (!player.weaponController.HasOnlyOneWeapon())
			SwitchOnBackupWeaponModelObject();

		SwitchAnimationLayer((int)CurrentWeaponModel().holdType);

		CurrentWeaponModel().gameObject.SetActive(true);

		AttachLeftHand();
	}

	public void SwitchOffWeaponModelsObjects()
	{
		for (int i = 0; i < weaponModels.Length; i++)
		{
			weaponModels[i].gameObject.SetActive(false);
		}
	}

	private void SwitchOffBackupWeaponModelsObjects()
	{
		foreach (BackupWeaponModel backupWeaponModel in backupWeaponModels)
		{
			backupWeaponModel.gameObject.SetActive(false);
		}
	}

	public void SwitchOnBackupWeaponModelObject()
	{
		WeaponType weaponType = player.weaponController.BackupWeapon().weaponType;

		foreach (BackupWeaponModel backupWeaponModel in backupWeaponModels)
		{
			if (backupWeaponModel.weaponType == weaponType)
				backupWeaponModel.gameObject.SetActive(true);
		}
	}

	private void SwitchAnimationLayer(int layerIndex)
	{
		for (int i = 1; i < animator.layerCount; i++)
		{
			animator.SetLayerWeight(i, 0);
		}

		animator.SetLayerWeight(layerIndex, 1);
	}

	public WeaponModel CurrentWeaponModel()
	{
		WeaponModel weaponModel = null;

		WeaponType weaponType = player.weaponController.CurrentWeapon().weaponType;

		for (int i = 0; i < weaponModels.Length; i++)
		{
			if (weaponModels[i].weaponType == weaponType)
			{
				weaponModel = weaponModels[i];
			}
		}

		return weaponModel;
	}

	#region Animation Rigging methods
	private void ReduceRigWeight()
	{
		rig.weight = 0;
	}
	public void ReturnRigWeightToOne() => shouldIncreaseRigWeight = true;

	private void IncreaseRigWeight()
	{
		if (shouldIncreaseRigWeight)
		{
			rig.weight += rigWeightIncreaseRate * Time.deltaTime;

			if (rig.weight >= 1)
				shouldIncreaseRigWeight = false;
		}
	}

	private void AttachLeftHand()
	{
		Transform targetTransform = CurrentWeaponModel().holdPoint;

		leftHandTarget.localPosition = targetTransform.localPosition;
		leftHandTarget.localRotation = targetTransform.localRotation;
	}
	#endregion
}
