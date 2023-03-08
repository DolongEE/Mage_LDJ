using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearReadyEnd : MonoBehaviour
{
    public ClearQueen clearQueen;
    private SphereCollider spCol;
    bool isAct;

    private void Awake()
    {
        isAct = false;
        spCol = GetComponent<SphereCollider>();
    }
    private void Start()
    {
        spCol.enabled = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Quest")
        {
            ItemCtrl itemCtrl = other.gameObject.GetComponent<ItemCtrl>();
            if (itemCtrl.item.itemName == "Orichalcumheart" && !isAct)
            {
                clearQueen.GameClear();
                isAct = true;
            }
        }
    }
}
