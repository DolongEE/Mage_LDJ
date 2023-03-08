using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffect : MonoBehaviour
{
    public GameObject Effect;
    public GameObject[] players;
    public Transform playerTarget;


    private Transform myTr;         //플레이어 위치 값 참조 변수 선언
    private Transform traceTarget;  //추적할 타겟의 위치 변수 선언

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.2f);
        players = GameObject.FindGameObjectsWithTag("Player");       //플레이어 위치 값 
        playerTarget = players[0].transform;
        

        yield return null;
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            Instantiate(Effect, new Vector3(playerTarget.position.x, playerTarget.position.y, playerTarget.position.z), Quaternion.identity);
        }
    }

}
