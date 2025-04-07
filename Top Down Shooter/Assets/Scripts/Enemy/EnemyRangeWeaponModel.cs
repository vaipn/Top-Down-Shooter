using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyRange_WeaponHoldType { Common, LowHold, HighHold }
public class EnemyRangeWeaponModel : MonoBehaviour
{
    public EnemyRange_WeaponType weaponType;
	public EnemyRange_WeaponHoldType weaponHoldType;

	public Transform leftHandTarget;
	public Transform leftElbowTarget;
}
