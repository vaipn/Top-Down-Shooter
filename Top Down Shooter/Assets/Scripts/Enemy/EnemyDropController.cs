using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDropController : MonoBehaviour
{
   
    public void DropItems()
    {
        Debug.Log("dropped some items");
    }

    private void CreateItem(GameObject go)
    {
        GameObject newItem = Instantiate(go, transform.position + Vector3.up, Quaternion.identity);
    }
}
