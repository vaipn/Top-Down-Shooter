using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerWeaponVisuals : MonoBehaviour
{
	private Animator animator;
	private bool isGrabbingWeapon;

	#region Gun transforms region
	[SerializeField] private Transform[] gunTransforms;

	[SerializeField] private Transform pistol;
	[SerializeField] private Transform revolver;
	[SerializeField] private Transform autoRifle;
	[SerializeField] private Transform shotGun;
	[SerializeField] private Transform sniperRifle;

	private Transform currentGun;
	#endregion

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

		SwitchOn(pistol);
		//SwitchAnimationLayer(1);
	}

	private void Update()
	{
		WeaponSwitch();

		if (Input.GetKeyDown(KeyCode.R) && !isGrabbingWeapon)
		{
			animator.SetTrigger("Reload");
			ReduceRigWeight();
		}

		if (shouldIncreaseRigWeight)
		{
			rig.weight += rigWeightIncreaseRate * Time.deltaTime;

			if (rig.weight >= 1)
				shouldIncreaseRigWeight = false;
		}
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

	private void WeaponSwitch()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			SwitchOn(pistol);
			SwitchAnimationLayer(1);
			PlayWeaponGrabAnimation(GrabType.SideGrab);
		}

		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			SwitchOn(revolver);
			SwitchAnimationLayer(1);
			PlayWeaponGrabAnimation(GrabType.SideGrab);
		}

		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			SwitchOn(autoRifle);
			SwitchAnimationLayer(1);
			PlayWeaponGrabAnimation(GrabType.BackGrab);
		}

		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			SwitchOn(shotGun);
			SwitchAnimationLayer(2);
			PlayWeaponGrabAnimation(GrabType.BackGrab);
		}

		if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			SwitchOn(sniperRifle);
			SwitchAnimationLayer(3);
			PlayWeaponGrabAnimation(GrabType.BackGrab);
		}
	}

	private void SwitchOn(Transform gunTransform)
	{
		SwitchOffGuns();
		gunTransform.gameObject.SetActive(true);

		currentGun = gunTransform;

		AttachLeftHand();
	}

	private void SwitchOffGuns()
	{
		for (int i = 0; i < gunTransforms.Length; i++)
		{
			gunTransforms[i].gameObject.SetActive(false);
		}
	}

	private void AttachLeftHand()
	{
		Transform targetTransform = currentGun.GetComponentInChildren<LeftHandTargetTransform>().transform;

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

public enum GrabType { SideGrab, BackGrab};
