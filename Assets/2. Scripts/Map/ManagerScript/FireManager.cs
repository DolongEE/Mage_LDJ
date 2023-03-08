using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireManager : MonoBehaviour
{
    //효과음연결
    private LevelManager lvelmgr;   //레벨매니저 연결

    public Transform respawn;
    public GameObject portal;
    public GameObject clearObject;
    public bool isClear;

    void Awake()
    {
        isClear = false;
        lvelmgr = gameObject.GetComponent<LevelManager>();
    }

    void Start()
    {

    }

    private void Update()
    {
        if (clearObject == null && !isClear)
        {
            //효과음추가
            lvelmgr.PlayPotalDnSound(); //포탈내려오는   효과음 호출

            isClear = true;
            portal.GetComponent<ClearMap>().EndGame();
        }
    }
}
