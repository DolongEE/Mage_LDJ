using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthManager : MonoBehaviour
{
    //효과음연결
    private LevelManager lvelmgr;   //레벨매니저 연결

    public Transform respawn;
    public GameObject portal;
    public GameObject clearObject;
    public bool isClear;
    public PlayerCheck playerCheck;
    bool isAct;

    void Awake()
    {
        isAct = false ;
        isClear = false;
        lvelmgr = gameObject.GetComponent<LevelManager>();
    }

    private void Update()
    {
        if(playerCheck.bossCreate)
        {
            if (!isAct)
            {
             //   clearObject = GameObject.FindGameObjectWithTag("Enemy");
                isAct = true;
            }
        }

        if(clearObject == null && !isClear && isAct)
        {
            //효과음
            lvelmgr.PlayPotalDnSound(); //포탈내려오는 효과음 호출
            isClear = true;
            portal.GetComponent<ClearMap>().EndGame();


        }
    }
}
