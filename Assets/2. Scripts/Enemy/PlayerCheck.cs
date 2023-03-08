using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCheck : MonoBehaviour
{
    public GameObject Enemy;
    public Animator EnemyAnim;
    public BossControll bossCtrl;
    public GameObject BossSP;
    public bool bossCreate;

    //효과음 삽입
    public AudioClip[] backsoundChange;   //효과음 받음 
    private SoundUiManager _sMgr; //SoundManager 컴포넌트 연결   
    public int stage;   //배경사운드 인덱스

    void Awake()
    {
        //효과음 삽입
        _sMgr = GameObject.Find("SoundUiManager").GetComponent<SoundUiManager>();
    }

    private void Start()
    {
        bossCreate = false;
        EnemyAnim = GameObject.FindGameObjectWithTag("Enemy").GetComponentInChildren<Animator>();
        Enemy = GameObject.FindGameObjectWithTag("Enemy");
    }
    
    private void OnTriggerEnter(Collider playerCol)
    {
        if(playerCol.gameObject.tag == "Player")
        {
            //배경음악 바꾸기
            _sMgr.audioSource.Stop();
            _sMgr.PlayBackground(stage);      // 몇번째 클립 배경음악 재생

            Debug.Log("플레이어 체크확인 시네마 애니메이션 시작");
            this.gameObject.GetComponent<BoxCollider>().enabled = false;
            EnemyAnim.SetBool("isStandUp",true);
            StartCoroutine(SetBoss());
        }
    }

    IEnumerator SetBoss()
    {
        yield return new WaitForSeconds(1.5f);
        EnemyAnim.SetBool("isWalk", true);
        yield return new WaitForSeconds(2f);
        EnemyAnim.SetBool("isRoar", true);
        yield return new WaitForSeconds(2f);
        EnemyAnim.SetBool("isIdle", true);
        yield return new WaitForSeconds(2f);

        BossInGame();
    }
    void BossInGame()
    {        
        //EnemyAnim.runtimeAnimatorController = Resources.Load("BossInGameAnim") as RuntimeAnimatorController;
        //Enemy.transform.position = new Vector3(-21.81f, 34.83f, 223.78f);
        //Enemy.transform.rotation = Quaternion.Euler(0f, 140f, 0f);
        Debug.Log("보스 포지션 변경");
        Enemy.SetActive(false);
        spBoss();
        
    }
    private void spBoss()
    {
        if (PhotonNetwork.isMasterClient)
        {
            BossSP.transform.GetChild(0).gameObject.SetActive(true);
            bossCreate = true;
        }
    }
}
