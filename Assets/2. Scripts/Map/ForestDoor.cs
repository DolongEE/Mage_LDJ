using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestDoor : MonoBehaviour
{

    //애니메이터 레퍼런스 선언
    public Animator anim;
    private GameObject otherAnim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        otherAnim = GameObject.Find("OpenB");
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            //오두막 문 열림
            Debug.Log("오두막 문 열림");
            anim.SetTrigger("DoorOpen");
            GetComponent<BoxCollider>().enabled = false;
            otherAnim.GetComponent<Animator>().SetTrigger("DoorOpen");
            otherAnim.GetComponent<BoxCollider>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
