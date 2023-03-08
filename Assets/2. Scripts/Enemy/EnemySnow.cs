using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySnow : MonoBehaviour
{
    [HideInInspector]
    //죽었는지 상태변수 
    public bool isDie;
    public float hp = 10; //몬스터 체력 변수 선언
    private Animator anim;  //애니메이션 레퍼런스 변수

    public LayerMask whatIsGround;  //그라운드의 레이어 체크 할 레이어 마스크 변수 선언
    public LayerMask whatIsPlayer;  //플레이어의 레이어를 체크 할 레이어 마스크 변수 선언

    #region Nav관련 변수
    //NavMeshAgent 연결 레퍼런스
    private NavMeshAgent myTraceAgent;

    //자신과 타겟 Transform 참조 변수  
    private Transform myTr;
    private Transform traceTarget;

    public Vector3 walkPoint;       //패트롤 때 사용할 워크 포인트 백터 변수 선언
    bool walkPointSet;              //walkPointSet 상태체크 변수 선언
    public float walkPointRange =10f;    //패트롤 워크 포인트 범위 설정 값 변수 선언



    //추적을 위한 변수
    private bool traceObject;
    private bool traceAttack;

    //추적 대상 거리체크 변수 
    public float dist1;

    //플레이어를 찾기 위한 배열 
    public GameObject[] players;
    public Transform playerTarget;
    #endregion

    //거리에 따른 상태 체크 변수 
    [Tooltip("몬스터 발견거리")]
    [Range(1f, 100f)] [SerializeField] float findDist = 40.0f;
    [Tooltip("몬스터 추적거리")]
    [Range(1f, 50f)] [SerializeField] float traceDist = 20.0f;
    [Tooltip("몬스터 공격거리")]
    [Range(1f, 10f)] [SerializeField] float attackDist = 10.0f;

    [SerializeField] private bool isHit;

    public enum MODE_STATE { IDLE = 1, TRACE, ATTACK, HIT, DIE, PATROLL };

    //인스펙터의 헤더의 표현을 위한 어트리뷰트 선언
    [Header("STATE")]
    //Enemy의 상태 셋팅
    public MODE_STATE enemyMode = MODE_STATE.IDLE;

    Rigidbody rBody;

    Transform frontCheck;
    void Awake()
    {
        // 위치값 불러옴
        myTr = GetComponent<Transform>();
        // Nav에이전트 불러옴
        myTraceAgent = GetComponent<NavMeshAgent>();
        // 애니메이션 불러옴
        anim = GetComponentInChildren<Animator>();

        // 자신과 가장 가까운 플레이어 찾음
        
        rBody = GetComponent<Rigidbody>();
        frontCheck = transform.GetChild(2).transform;
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.3f);
        players = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log("Player 게임 오브젝트 찾기 완료");
        // 정해진 시간 간격으로 Enemy의 Ai 변화 상태를 셋팅하는 코루틴
        StartCoroutine(ModeSet());
        
        // Enemy의 상태 변화에 따라 일정 행동을 수행하는 코루틴
        StartCoroutine(ModeAction());

        StartCoroutine(TargetSetting());                    
        //// 추적 시작
        //myTraceAgent.SetDestination(traceTarget.position);

        yield return null;
    }   

    private void OnCollisionEnter(Collision col)
    {
        Debug.Log("콜라이더 충돌 체크");
        if (col.gameObject.tag == "Bullet")
        {
            int bulletPower = col.gameObject.GetComponent<BulletCtrl>().power;
            isHit = true;
            TakeDamage(bulletPower);
        }
    }

    private void TakeDamage(int dam)       //몬스터 데미지 함수
    {
        hp -= dam;       //TakeDamage가 호출되면 몬스터의 피가 데미지만큼 감소한다
        isHit = false;
        if (hp <= 0)
        {
           
            anim.SetBool("isDie", true);
            Invoke(nameof(DestroyEnemy), 0.5f);   //몬스터의 피가 0보다 작거나 같으면 죽은 것으로 판단하여 인보크함수 3초 후 DestroyEnemy함수가 호출된다.

        }
    }
    private void DestroyEnemy()
    {
        PhotonNetwork.Instantiate("ItemPrefabs/HPpotion", transform.position, transform.rotation, 0);
        Destroy(gameObject);        //몬스터 Destroy
    }
    // Enemy 상태 함수
    IEnumerator ModeSet()
    {
        while (!isDie)
        {
            Debug.Log("ModeSet 함수 실행");
            yield return new WaitForSeconds(0.2f);

            //자신과 Player의 거리 셋팅 
            float dist = Vector3.Distance(myTr.position, traceTarget.position);

            if (isHit)
            {
                enemyMode = MODE_STATE.HIT;
            }
            else if (dist <= attackDist && hp > 0)
            {
                enemyMode = MODE_STATE.ATTACK;
            }
            else if (dist <= traceDist && hp > 0)
            {
                enemyMode = MODE_STATE.TRACE;
            }            
            else if (hp <= 0)
            {
                enemyMode = MODE_STATE.DIE;
            }
            else
            {
                enemyMode = MODE_STATE.IDLE;
            }
        }
    }

    // Enemy 행동 함수
    IEnumerator ModeAction()
    {
        Debug.Log("ModeAction 함수 실행");
        while (!isDie)
        {
            switch (enemyMode)
            {
                //Enemy가 Idle 상태 일때... 
                case MODE_STATE.IDLE:
                    //네비게이션 멈추고 (추적 중지)
                    Patroling();
                    anim.SetBool("isAttack", false);
                    break;

                //Enemy가 Trace 상태 일때... 
                case MODE_STATE.TRACE:
                    anim.SetBool("isWalk", true);
                    anim.SetBool("isAttack", false);
                    // 네비게이션 재시작(추적)
                    myTraceAgent.isStopped = false;
                    myTraceAgent.SetDestination(traceTarget.position);
                    // 추적대상 설정(플레이어)
                    myTraceAgent.destination = traceTarget.position;
                    break;

                //공격 상태
                case MODE_STATE.ATTACK:
                    // 사운드 (공격)
                    anim.SetBool("isWalk", false);
                    anim.SetBool("isAttack", true);
                    //네비게이션 멈추고 (추적 중지) 
                    myTraceAgent.isStopped = true;
                    //this.myTraceAgent.updatePosition = false;
                    //this.myTraceAgent.updateRotation = false;
                    this.myTraceAgent.velocity = Vector3.zero;
                    //공격할때 적을 봐라 봐야함 
                    Quaternion enemyLookRotation = Quaternion.LookRotation(traceTarget.position - myTr.position); // - 해줘야 바라봄  
                    myTr.rotation = Quaternion.Lerp(myTr.rotation, enemyLookRotation, Time.deltaTime * 10.0f);

                    break;

                //Enemy가 hit 상태 일때... 
                case MODE_STATE.HIT:
                    // 사운드 (피격)
                    anim.SetTrigger("getHit");
                    anim.SetBool("isAttack", false);
                    //네비게이션 멈추고 (추적 중지)
                    myTraceAgent.isStopped = true;
                    //this.myTraceAgent.updatePosition = false;
                    //this.myTraceAgent.updateRotation = false;
                    this.myTraceAgent.velocity = Vector3.zero;

                    break;

                    //Enemy가 Die할 때
                case MODE_STATE.DIE:
                    // 사운드 (다이)
                    anim.SetBool("isDie", true);
                    break;

                case MODE_STATE.PATROLL:
                        //사운드 다이
                        anim.SetBool("isWalk", true);
                    Patroling();


                    break;
            }
            yield return null;
        }
    }

    IEnumerator TargetSetting()
    {
        Debug.Log("TargetSetting 함수 실행");
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
                traceTarget = playerTarget;
            }          
            
        }
    }
    //몬스터 패트롤
    private void Patroling()
    {
        Debug.Log("Patroling 함수 실행");
       
        if (!walkPointSet)       //워크 포인트가 false 이면 워크포인트를 찾는 serachPoint 함수 실행
        {
            SearchWalkPoint();
        }
        if (walkPointSet)           //워크 포인트가 true이면 워크포인트로 매쉬 이동 (패트롤 실행)
        {
            myTraceAgent.SetDestination(walkPoint);
            anim.SetBool("isWalk", true);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;   //현재 트랜스폼에서 워크포인트까지 뺀 거리가 워크포인트까지의 이동거리

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f) //워크포인트까지의 이동거리가 1보다 작으면 walkPointSet false 즉, 다시 워크포인트 찾기 실행
        {
            walkPointSet = false;
        }
    }
    private void SearchWalkPoint()
    {
        Debug.Log("SearchWalkPoint 함수 실행");
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }
}
