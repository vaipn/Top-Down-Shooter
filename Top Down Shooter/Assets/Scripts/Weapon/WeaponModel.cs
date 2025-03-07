using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum HoldType { CommonHold = 1, LowHold, HighHold };
public enum EquipType { SideEquip, BackEquip };

public class WeaponModel : MonoBehaviour
{
    public WeaponType weaponType;
    public EquipType equipType;
    public HoldType holdType;

    public Transform gunPoint;
    public Transform holdPoint;
}


