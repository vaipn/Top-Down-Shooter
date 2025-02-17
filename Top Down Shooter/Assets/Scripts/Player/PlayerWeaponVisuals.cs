using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerWeaponVisuals : MonoBehaviour
{
	private Player player;

	private Animator animator;
	private bool isGrabbingWeapon;


	[SerializeField] private WeaponModel[] weaponModels;


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

		player = GetComponent<Player>();

		//SwitchAnimationLayer(1);
	}

	private void Update()
	{

		if (shouldIncreaseRigWeight)
		{
			rig.weight += rigWeightIncreaseRate * Time.deltaTime;

			if (rig.weight >= 1)
				shouldIncreaseRigWeight = false;
		}
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

	public void PlayReloadAnimation()
	{
		if (isGrabbingWeapon)
			return;

		animator.SetTrigger("Reload");
		ReduceRigWeight();
	}

	private void ReduceRigWeight()
	{
		rig.weight = 0;
	}

	private void PlayWeaponGrabAnimation(GrabType grabType)
	{
		ReduceRigWeight();
		animator.SetFloat("Weapon Grab Type", (float)grabType);
		animator.SetTrigger("Grab Weapon");

		SetIsGrabbingWeaponTo(true);
	}

	public void FinishedGrabbingWeapon()
	{
		SetIsGrabbingWeaponTo(false);
	}

	private void SetIsGrabbingWeaponTo(bool busy)
	{
		isGrabbingWeapon = busy;
		animator.SetBool("isGrabbingWeapon", isGrabbingWeapon);
	}

	public void ReturnRigWeightToOne() => shouldIncreaseRigWeight = true;

	//private void WeaponSwitch()
	//{
	//	if (Input.GetKeyDown(KeyCode.Alpha1))
	//	{
	//		SwitchOn();
	//		SwitchAnimationLayer(1);
	//		PlayWeaponGrabAnimation(GrabType.SideGrab);
	//	}

	//	if (Input.GetKeyDown(KeyCode.Alpha2))
	//	{
	//		SwitchOn();
	//		SwitchAnimationLayer(1);
	//		PlayWeaponGrabAnimation(GrabType.SideGrab);
	//	}

	//	if (Input.GetKeyDown(KeyCode.Alpha3))
	//	{
	//		SwitchOn();
	//		SwitchAnimationLayer(1);
	//		PlayWeaponGrabAnimation(GrabType.BackGrab);
	//	}

	//	if (Input.GetKeyDown(KeyCode.Alpha4))
	//	{
	//		SwitchOn();
	//		SwitchAnimationLayer(2);
	//		PlayWeaponGrabAnimation(GrabType.BackGrab);
	//	}

	//	if (Input.GetKeyDown(KeyCode.Alpha5))
	//	{
	//		SwitchOn();
	//		SwitchAnimationLayer(3);
	//		PlayWeaponGrabAnimation(GrabType.BackGrab);
	//	}
	//}

	private void SwitchOn()
	{
		SwitchOffGuns();

		CurrentWeaponModel().gameObject.SetActive(true);

		AttachLeftHand();
	}

	private void SwitchOffGuns()
	{
		for (int i = 0; i < weaponModels.Length; i++)
		{
			weaponModels[i].gameObject.SetActive(false);
		}
	}

	private void AttachLeftHand()
	{
		Transform targetTransform = CurrentWeaponModel().holdPoint;

		leftHandTarget.localPosition = targetTransform.localPosition;
		leftHandTarget.localRotation = targetTransform.localRotation;
	}

	private void SwitchAnimationLayer(int layerIndex)
	{
		for (int i = 1; i < animator.layerCount; i++)
		{
			animator.SetLayerWeight(i, 0);
		}

		animator.SetLayerWeight(layerIndex, 1);
	}
}
