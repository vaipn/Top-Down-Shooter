using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponVisualController : MonoBehaviour
{
	private Animator animator;

	[SerializeField] private Transform[] gunTransforms;

	[SerializeField] private Transform pistol;
	[SerializeField] private Transform revolver;
	[SerializeField] private Transform autoRifle;
	[SerializeField] private Transform shotGun;
	[SerializeField] private Transform sniperRifle;

	private Transform currentGun;

	[Header("Left hand IK")]
	[SerializeField] private Transform leftHandTarget;

	private void Start()
	{
		animator = GetComponentInParent<Animator>();

		SwitchOn(pistol);
		//SwitchAnimationLayer(1);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			SwitchOn(pistol);
			SwitchAnimationLayer(1);
		}

		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			SwitchOn(revolver);
			SwitchAnimationLayer(1);
		}

		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			SwitchOn(autoRifle);
			SwitchAnimationLayer(1);
		}

		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			SwitchOn(shotGun);
			SwitchAnimationLayer(2);
		}

		if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			SwitchOn(sniperRifle);
			SwitchAnimationLayer(3);
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
