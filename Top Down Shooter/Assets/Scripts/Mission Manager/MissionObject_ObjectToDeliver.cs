using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionObject_ObjectToDeliver : MonoBehaviour
{
    public static event Action OnObjectDelivery;

    public void InvokeOnObjectDelivery() => OnObjectDelivery?.Invoke();
}
