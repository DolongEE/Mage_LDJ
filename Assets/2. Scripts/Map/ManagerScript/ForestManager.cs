using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestManager : MonoBehaviour
{
    //애니메이터 변수 선언
    private Animator anim1;
    private Animator anim2;
    public GameObject water;
    //효과음연결
    public LevelManager lvelmgr;   //레벨매니저 연결

    public Transform respawn;
    public GameObject portal;
    public GameObject clearObject;
    public bool isClear;

    //맵 들어오면 활성화될 토치, 낫
    private PlayerCtrl player;
    private ItemEffectDatabase itemEftDb;   //아이템효과디비 연결

    void Awake()
    {
        isClear = false;
        lvelmgr = GetComponent<LevelManager>();
        //잔디 애니메이터 설정
        anim1 = GameObject.Find("Grass").GetComponent<Animator>();
        //물 애니메이터 설정
        anim2 = GameObject.Find("Water").GetComponent<Animator>();
        itemEftDb = GameObject.Find("EffectSystem").GetComponent<ItemEffectDatabase>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl>();
    }

    void Start()
    {
        //애니메이션  false 값으로 초기화
        anim2.SetBool("WaterUp", false);       
    }

    private void Update()
    {
        if (clearObject == null && !isClear)
        {
            lvelmgr.PlayWaterDnSound();  //물다운 효과음 호출           

            isClear = true;
            water.GetComponent<ForestWater>().gameEnd = true;
            anim1.SetBool("GrassClear", true);      //잔디 애니메이션 실행
            //anim2.SetBool("WaterUp", true);         //물 애니메이션 실행 기존 물 올라오는 애니메이션
            anim2.SetBool("WaterDown", true);
            StartCoroutine(SceneClear());
        }

        //장착슬롯에 낫 있으면 낫 활성화(얻데이트에서 계속 활성화 해주니까 비활성화가 안먹힘)
        if ((itemEftDb.ESlotRUImg.sprite != null))
        {
            player.SickleOn();
        }
    }

    IEnumerator SceneClear()
    {
        yield return new WaitForSeconds(15f);
        portal.GetComponent<ClearMap>().EndGame();
        lvelmgr.PlayPotalDnSound();  //물다운 효과음 호출

        yield return null ;
    }   
}
