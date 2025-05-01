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

    [Header("Damage attributes")]
    public Transform[] damagePoints;
    public float attackRadius;

    [ContextMenu("Assign damage point transforms")]
    private void GetDamagePoints()
    {
        damagePoints = new Transform[trailEffects.Length];

        for (int i = 0; i < trailEffects.Length; i++)
        {
            damagePoints[i] = trailEffects[i].transform;
        }
    }

	public void EnableTrailEffect(bool enable)
    {
        foreach (var effect in trailEffects)
            effect.SetActive(enable);
    }

	private void OnDrawGizmos()
	{
		if (damagePoints.Length > 0)
        {
            foreach (Transform point in damagePoints)
            {
                Gizmos.DrawWireSphere(point.position, attackRadius);
            }
        }
	}
}
