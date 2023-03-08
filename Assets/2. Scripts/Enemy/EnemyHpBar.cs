using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHpBar : MonoBehaviour
{
    //몬스터 라이프
    private int life = 100;
    //자신의 트렌스폼
    private Transform myTr;
    //BossControllT 연결 레퍼런스 보스
    public BossControllT enemy;
    //생명력 바 연결 레퍼런스 (특정 컴포넌트 아니면 Renderer 로 연결가능)
    public MeshRenderer lifeBar;

    //포톤추가
    public PhotonView pv = null;

    void Awake()
    {
        //레퍼런스 할당
        myTr = GetComponent<Transform>();
        //포톤추가
        pv = PhotonView.Get(this); //포톤뷰 할당 다른방식 연결
    }


    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.tag == "Bullet")
        {        
            //포톤추가 각요소를 변수로 저장
            int pow = coll.gameObject.GetComponent<BulletCtrl>().power;

            //모든 네트웍 유저의 몬스터에 RPC 데이타를 전송하며 RPC 함수를 호출, 로컬 플레이어는 로컬 Deamage 함수를 바로 호출 
            pv.RPC("Demage", PhotonTargets.AllBuffered, pow);

            //몬스터 타격 루틴을 위한 호출
         //   enemy.HitEnemy();
        }
    }

    //포톤추가
    [PunRPC]
        void Demage(int dam)
        {
            life -= dam; //맞은 총알의 파워를 가져와 에너미 라이프를 감소
            lifeBar.material.SetFloat("_Progress", life / 100.0f);

            // 생명력이 바닥이면 죽이자
            if (life <= 0)
            {               
                enemy.EnemyDie();
            }
        }


    //    // 추후 보스 체력이랑 연동
    //    public void EnemyBar()
    //{
    //    if (collision.gameObject.tag == "Bullet")
    //    {
    //        playerHpBar.fillAmount = (float)playerHP / (float)playerMaxHP;

    //        if (playerHpBar.fillAmount <= 0.4f)
    //        {
    //            playerHpBar.color = Color.yellow;
    //        }

    //        // 적 무기에 맞았을시에 플레이어 체력 감소 
    //        if (playerHP <= 0)
    //        {
    //            // RespawnTr라는 빈게임 오브젝트와 태그 추가
    //            respawnTr = GameObject.FindGameObjectWithTag("RespawnTr").transform;
    //            this.gameObject.transform.position = respawnTr.position;
    //        }
    //    }
    //}
}
