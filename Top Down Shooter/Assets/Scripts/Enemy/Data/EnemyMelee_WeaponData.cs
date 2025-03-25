using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Enemy Data/Melee Weapon Data")]
public class EnemyMelee_WeaponData : ScriptableObject
{
    public List<EnemyMeleeAttackData> attackData;
    public float turnSpeed = 10;
}
