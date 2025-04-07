using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHeldWeaponModel : MonoBehaviour
{
    public EnemyMelee_WeaponType meleeWeaponType;
	public EnemyRange_WeaponType rangeWeaponType;
	public Enemy_WeaponName weaponName;
    

    public EnemyMelee_WeaponData weaponData;

    public AnimatorOverrideController overrideController;
    [SerializeField] private GameObject[] trailEffects;

	public void EnableTrailEffect(bool enable)
    {
        foreach (var effect in trailEffects)
            effect.SetActive(enable);
    }
}
