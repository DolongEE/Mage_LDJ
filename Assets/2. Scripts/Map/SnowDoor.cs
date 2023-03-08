using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowDoor : MonoBehaviour
{

    //애니메이터 레퍼런스 선언
    public Animator anim;
    private GameObject otherAnim;
    // Start is called before the first frame update

    //동굴문 열기
    private ItemEffectDatabase itemEftDb;   //아이템효과디비 연결

    //동굴설명 연결
    public MapTip maptip;

    void Start()
    {
        anim = GetComponent<Animator>();
        otherAnim = GameObject.Find("DoorB");
        itemEftDb = GameObject.Find("EffectSystem").GetComponent<ItemEffectDatabase>();
        maptip = GameObject.Find("PlayerUiManager").GetComponent<MapTip>();
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            if (itemEftDb.ESlotRDImg.sprite == null)
            {
                maptip.cavehelpOpen();
            }               
            //만약 토치장착칸이 널이 아니면 문열림 함수 실행(아이템 이펙효과 스크립트 연결 ESlotRDImg)
            if (itemEftDb.ESlotRDImg.sprite != null)
            {
                //오두막 문 열림
                Debug.Log("Cave 문 열림");
                anim.SetTrigger("DoorOpen");
                GetComponent<BoxCollider>().enabled = false;
                otherAnim.GetComponent<Animator>().SetTrigger("DoorOpen");
                otherAnim.GetComponent<BoxCollider>().enabled = false;
            }          
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            maptip.cavehelpClose();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
