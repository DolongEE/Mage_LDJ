using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWater : MonoBehaviour
{
    PhotonView pv = null;

    [HideInInspector]
    //죽었는지 상태변수 
    public bool isDie;

    private Animator anim;

    public float moveSpeed;
    Rigidbody rigid;

    //자신과 타겟 Transform 참조 변수  
    private Transform myTr;
    private Transform traceTarget;

    //추적 대상 거리체크 변수 
    public float dist1;

    //플레이어를 찾기 위한 배열 
    private GameObject[] players;
    private Transform playerTarget;

    [Tooltip("몬스터 공격거리")]
    [Range(10f, 30f)] [SerializeField] float attackDist = 30.0f;

    [SerializeField] private bool isHit;

    public enum MODE_STATE { IDLE = 1,TRACE, ATTACK, HIT, DIE };

    //인스펙터의 헤더의 표현을 위한 어트리뷰트 선언
    [Header("STATE")]
    //Enemy의 상태 셋팅
    public MODE_STATE enemyMode = MODE_STATE.IDLE;

    float wtime;
    bool isAttack;

    Quaternion enemyLookRotation;

    Vector3 currPos = Vector3.zero;
    Quaternion currRot = Quaternion.identity;
    void Awake()
    {
        pv = GetComponent<PhotonView>();
        isAttack = false;
        wtime = 0;
        // 위치값 불러옴
        myTr = GetComponent<Transform>();
   
        // 애니메이션 불러옴
        anim = GetComponentInChildren<Animator>();

        // 자신과 가장 가까운 플레이어 찾음
        players = GameObject.FindGameObjectsWithTag("Player");

        rigid = GetComponent<Rigidbody>();

        pv.ObservedComponents[0] = this;
        //데이타 전송 타입을 설정
        pv.synchronization = ViewSynchronization.UnreliableOnChange;

        if (!PhotonNetwork.isMasterClient)
        {
            rigid.isKinematic = true;
        }

        currPos = myTr.position;
        currRot = myTr.rotation;
    }

    private void Update()
    {
        if (PhotonNetwork.isMasterClient)
        {
            wtime += Time.deltaTime;
            if (wtime > 3f && isAttack)
            {
                isAttack = false;
            }
            else
            {
                // 정해진 시간 간격으로 Enemy의 Ai 변화 상태를 셋팅하는 코루틴
                StartCoroutine(ModeSet());

                StartCoroutine(TargetSetting());
                // Enemy의 상태 변화에 따라 일정 행동을 수행하는 코루틴
                StartCoroutine(ModeAction());

                wtime = 0;
            }
        }
        else
        {
            //원격 플레이어의 아바타를 수신받은 위치까지 부드럽게 이동시키자
            myTr.position = Vector3.Lerp(myTr.position, currPos, Time.deltaTime * 3.0f);
            //원격 플레이어의 아바타를 수신받은 각도만큼 부드럽게 회전시키자
            myTr.rotation = Quaternion.Slerp(myTr.rotation, currRot, Time.deltaTime * 3.0f);
        }
    }

    // Enemy 상태 함수
    IEnumerator ModeSet()
    {
        yield return new WaitForSeconds(0.2f);

        //자신과 Player의 거리 셋팅 
        float dist = Vector3.Distance(myTr.position, traceTarget.position);

        if (isHit)
        {
            enemyMode = MODE_STATE.HIT;
        }
        else if (dist <= attackDist)
        {
            enemyMode = MODE_STATE.ATTACK;
            isAttack = true;
        }
        else
        {
            enemyMode = MODE_STATE.TRACE;
        }
        yield return null;
    }

    // Enemy 행동 함수
    IEnumerator ModeAction()
    {
        while (!isDie)
        {
            switch (enemyMode)
            {
                //Enemy가 Trace 상태 일때... 
                case MODE_STATE.TRACE:
                    anim.SetBool("isWalk", true);
                    anim.SetBool("isAttack", false);
                    myTr.rotation = Quaternion.Lerp(myTr.rotation, enemyLookRotation, Time.deltaTime * 10.0f);
                    transform.position += transform.TransformDirection(Vector3.forward) * moveSpeed;
                    break;
                //공격 상태
                case MODE_STATE.ATTACK:
                    // 사운드 (공격)
                    anim.SetBool("isWalk", false);
                    anim.SetBool("isAttack", true);
                    transform.position += transform.TransformDirection(traceTarget.position) * 0;

                    myTr.rotation = Quaternion.Lerp(myTr.rotation, enemyLookRotation, Time.deltaTime * 10.0f);

                    break;
                //Enemy가 hit 상태 일때... 
                case MODE_STATE.HIT:
                    // 사운드 (피격)
                    anim.SetTrigger("damaged");
                    anim.SetBool("isAttack", false);
                    break;
            }
            yield return null;
        }
        yield return null;
    }

    IEnumerator TargetSetting()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.1f);

            //플레이어가 있을경우 
            if (players.Length != 0)
            {
                playerTarget = players[0].transform;
                dist1 = (playerTarget.position - myTr.position).sqrMagnitude;
                foreach (GameObject _players in players)
                {
                    if ((_players.transform.position - myTr.position).sqrMagnitude < dist1)
                    {
                        playerTarget = _players.transform;
                        dist1 = (playerTarget.position - myTr.position).sqrMagnitude;
                    }
                }
            }
            traceTarget = playerTarget;
            enemyLookRotation = Quaternion.LookRotation(traceTarget.position - myTr.position); // - 해줘야 바라봄
            yield return null;
        }
        yield return null;
    }


    public void EnemyDie()
    {
        // 포톤 추가
        if (pv.isMine)
        {
            StartCoroutine(this.Die());
        }
    }

    // Enemy의 사망 처리
    IEnumerator Die()
    {
        // Enemy의를 죽이자
        isDie = true;

        //Enemy의 모드를 die로 설정
        enemyMode = MODE_STATE.DIE;
        //네비게이션 멈추고 (추적 중지) 
        transform.position += transform.TransformDirection(traceTarget.position) * 0;

        //4.5 초후 오브젝트 삭제
        yield return new WaitForSeconds(1f);

        PhotonNetwork.Instantiate("ItemPrefabs/HPpotion", transform.position, transform.rotation, 0);
        // 자신과 네트워크상의 모든 아바타를 삭제
        PhotonNetwork.Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Bullet")
        {

            EnemyDie();
        }
    }
    private void OnParticleCollision(GameObject other)
    {
        EnemyDie();
    }

    void OnDestroy()
    {
        //모든 코루틴을 정지시키자
        StopAllCoroutines();
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //로컬 플레이어의 위치 정보를 송신
        if (stream.isWriting)
        {
            stream.SendNext(myTr.position);
            stream.SendNext(myTr.rotation);
        }
        //원격 플레이어의 위치 정보를 수신
        else
        {            
            currPos = (Vector3)stream.ReceiveNext();
            currRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
