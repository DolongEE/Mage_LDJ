using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public int damage;
    public Transform target;    //Player 추적 Target 변수 선언
    public bool attack;        //attack 변수 선언

    Rigidbody rigid;            //Rigidbody 변수 선언
    BoxCollider boxCollider;    //boxCollider 변수 선언

    Animator anim;              //Animation 사용할 변수 선언
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        anim = GetComponent<Animator>();

       
    }



    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "GROUND")
        {
            Destroy(gameObject, 3);
        }
        else if(col.gameObject.tag=="Wall")
        {
            Destroy(gameObject);
        }
        else if(col.gameObject.tag=="Player")
        {
            Destroy(gameObject);
        }
    }
}
