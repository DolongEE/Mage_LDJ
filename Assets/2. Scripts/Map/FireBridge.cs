using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBridge : MonoBehaviour
{
    //효과음연결
    private LevelManager lvelmgr;   //레벨매니저 연결

    void Awake()
    {
        lvelmgr = GameObject.Find("FireManager").GetComponent<LevelManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Invoke("DestroyBridge", 0.5f);
        }
    }

    void DestroyBridge()
    {
        Destroy(gameObject);
        lvelmgr.PlayWaterDnSound(); //backSound 첫번째 효과음 호출

    }
}
