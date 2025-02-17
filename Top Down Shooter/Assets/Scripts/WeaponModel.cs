using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponModel : MonoBehaviour
{
    public WeaponType weaponType;
    public GrabType grabType;
    public HoldType holdType;

    public Transform gunPoint;
    public Transform holdPoint;
}

public enum HoldType { CommonHold = 1, LowHold, HighHold };
public enum GrabType { SideGrab, BackGrab };
