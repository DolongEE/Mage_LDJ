using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestWater : MonoBehaviour
{
    public float massF;
    public float dragF;
    private int playerCnt;

    Transform[] EnemySpawnPoints;
    private GameObject[] Enemys;

    bool isStart;
    bool isGet;
    public bool gameEnd;

    private void Awake()
    {
        isStart = false;
        gameEnd = false;
        isGet = false;
        playerCnt = 0;
        EnemySpawnPoints = GameObject.FindGameObjectWithTag("EnemySpawnPoint").GetComponentsInChildren<Transform>();        
    }
    private void Update()
    {
        if (playerCnt > 0)
        {
            isStart = true;
        }

        if(gameEnd && isStart)
        {
            if (!isGet)
            {
                StopAllCoroutines();
                Enemys = GameObject.FindGameObjectsWithTag("Enemy");
                for (int i = 1; i < Enemys.Length; i++)
                {
                    PhotonNetwork.Destroy(Enemys[i]);
                }                
                isGet = true;
            }
        }
    }   

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerCnt++;
            Debug.Log("입장");
            
            if (PhotonNetwork.isMasterClient)
            {
                // 몬스터 스폰 코루틴 호출
                StartCoroutine(this.CreateEnemy());
            }
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerCtrl playerCtrl = other.gameObject.GetComponent<PlayerCtrl>();
            playerCtrl.waterJump = true;
            playerCtrl.playerHP -= 0.01f;
        }       

        Rigidbody rigid = other.gameObject.GetComponent<Rigidbody>();
        rigid.mass = massF;
        rigid.drag = dragF;
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rigid = other.gameObject.GetComponent<Rigidbody>();
        rigid.mass = 1f;
        rigid.drag = 0f;
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerCtrl>().waterJump = false;
            playerCnt--;
            Debug.Log("퇴장");
        }
    }

    IEnumerator CreateEnemy()
    {
        yield return new WaitForSeconds(0.3f);

        //게임중 일정 시간마다 계속 호출됨 
        while (!gameEnd)
        {
            // 스테이지 총 몬스터 객수 제한을 위하여 찾자~
            Enemys = GameObject.FindGameObjectsWithTag("Enemy");

            //리스폰 타임 10초
            yield return new WaitForSeconds(10.0f);     
            // 스테이지 총 몬스터 객수 제한
            if (Enemys.Length < 8)
            {
                for (int i = 1; i < EnemySpawnPoints.Length - 1; i++)
                {
                    // 네트워크 플레이어를 Scene 에 귀속하여 생성
                    PhotonNetwork.InstantiateSceneObject("WaterEnemy", EnemySpawnPoints[i].position, EnemySpawnPoints[i].rotation, 0, null);
                }
            }
        }
        yield return null;
    }

    
}
