using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class MiddleBossControll : MonoBehaviour
{
    public NavMeshAgent agent;      //NavMeshAgent 변수 선언

    public GameObject[] players;
    public Transform playerTarget;
    private Rigidbody rBody;
    public bool isDie;
    private Transform myTr;         //플레이어 위치 값 참조 변수 선언
    private Transform traceTarget;  //추적할 타겟의 위치 변수 선언

    private Animator Anim;          //Animator 변수 선언

    public LayerMask whatIsGround;  //그라운드의 레이어 체크 할 레이어 마스크 변수 선언
    public LayerMask whatIsPlayer;  //플레이어의 레이어를 체크 할 레이어 마스크 변수 선언


    public float hp = 100f;            //에너미 체력 변수 설정

    //Patroling
    public Vector3 walkPoint;       //패트롤 때 사용할 워크 포인트 백터 변수 선언
    bool walkPointSet;              //walkPointSet 상태체크 변수 선언
    public float walkPointRange;    //패트롤 워크 포인트 범위 설정 값 변수 선언

    //Attacking
    public float timeBetweenAttacks = 3f;    //공격 쿨타임 변수 선언
    bool alreadyAttacked;               //어택 여부 확인 변수 선언
    public GameObject SkillBullet;      //일반 공격 및 스킬 변수 선언
    public float RockAddForce = 60f;
    public float RockAddForceUp = 25f;

    //States
    public float sightRange = 60f;            //플레이어 추적 범위 변수 선언
    public float attackRange = 50f;           //플레이어 추적 후 어택 범위 변수 선언
    public bool playerInSightRange;     //플레이어 추적 범위에 들어왔는지 체크하기위한 변수 선언
    public bool playerInAttackRange;    //플레이어 어택 범위에 들어왔는지 체크하기 위한 변수 선언

    //죽었을때 물 원소 활성화
    public GameObject hydro;

    private void Awake()
    {
        rBody = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();               //NavMesh 설정
        Anim = GetComponent<Animator>();                    //Animator 설정
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.3f);
        players = GameObject.FindGameObjectsWithTag("Player");       //플레이어 위치 값 
        playerTarget = players[0].transform;

        yield return null;
    }

    private void Update()
    {
        //Check for sight and attack range
        //플레이어 추적 범위 설정
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        //플레이어 공격 범위 설정
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        if (!isDie)
        {
            //플레이어가 추적범위에 없고, 공격 범위에도 없을 때 몬스터 패트롤
            if (!playerInSightRange && !playerInAttackRange)
            {
                Debug.Log("패트롤");
                Patroling();
            }

            //플레이어가 추적범위에 있고, 공격 범위에 있을 때 플레이어 추적
            else if (playerInSightRange && !playerInAttackRange)
            {
                Debug.Log("추적");
                ChasePlayer();
            }

            //플레이어가 공격범위에 있고, 추적범위에도 있을 때 플레이어 공격 시작
            else if (playerInAttackRange && playerInSightRange)
            {
                Debug.Log("공격 시작");
                Anim.SetBool("isAttack", true);
                Anim.SetBool("isWalk", false);
                //AttackPlayer();
            }
        }
    }

    //몬스터 패트롤
    private void Patroling()
    {
        if (!walkPointSet)       //워크 포인트가 false 이면 워크포인트를 찾는 serachPoint 함수 실행
        {
            SearchWalkPoint();
        }
        if (walkPointSet)           //워크 포인트가 true이면 워크포인트로 매쉬 이동 (패트롤 실행)
        {
            agent.SetDestination(walkPoint);
            Anim.SetBool("isWalk", true);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;   //현재 트랜스폼에서 워크포인트까지 뺀 거리가 워크포인트까지의 이동거리

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f) //워크포인트까지의 이동거리가 1보다 작으면 walkPointSet false 즉, 다시 워크포인트 찾기 실행
        {
            walkPointSet = false;
        }
    }

    //워크포인트 Serach
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    //플레이어 추적
    private void ChasePlayer()
    {
        agent.SetDestination(playerTarget.position);  //플레이어쪽으로 이동
        Anim.SetBool("isWalk", true);
        transform.LookAt(playerTarget);           //플레이어를 바라본다.
    }
    public void AttackPlayer()
    {
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);       //플레이어쪽으로 이동
        Anim.SetBool("isWalk", false);
        transform.LookAt(playerTarget);           //플레이어를 바라본다.

        if (!alreadyAttacked)
        {
            //Attack code here
            //Bullet 생성
            Rigidbody rb = Instantiate(SkillBullet, new Vector3(transform.position.x, transform.position.y + 10f, transform.position.z), Quaternion.identity).GetComponent<Rigidbody>();

            rb.AddForce(transform.forward * RockAddForce, ForceMode.Impulse);        //보는 방향쪽으로 30 Addforce
            rb.AddForce(transform.up * RockAddForceUp, ForceMode.Impulse);              //위쪽 방향으로 8 AddForce

            alreadyAttacked = true;         //공격 중인 것을 체크
            Invoke(nameof(ResetAttack), timeBetweenAttacks);        //공격 리셋을 timeBetweenAttacks 의 시간 설정으로 리셋한다.

        }
    }

    private void ResetAttack()  //공격 리셋
    {
        alreadyAttacked = false;
    }
    private void OnCollisionEnter(Collision col)
    {
        Debug.Log("콜라이더 충돌 체크");
        if (col.gameObject.tag == "Bullet")
        {
            if (!isDie)
            {
                TakeDamage(col.gameObject.GetComponent<BulletCtrl>().power);
                Anim.SetTrigger("getHit");
            }
        }
    }
    private void TakeDamage(int dam) //몬스터 데미지 함수
    {
        hp -= dam;       //TakeDamage가 호출되면 몬스터의 피가 데미지만큼 감소한다

        if (hp <= 0)
        {
            isDie = true;
            IsDie();
        }
    }
    void IsDie()
    {

        Anim.SetTrigger("Die");
        //몬스터의 피가 0보다 작거나 같으면 죽은 것으로 판단하여 인보크함수 5초 후 DestroyEnemy함수가 호출된다.
        Invoke(nameof(DestroyEnemy), 2f);
        rBody.isKinematic = true;
    }

    private void DestroyEnemy()
    {
        //죽었을때 아이템 드랍
        //    PhotonNetwork.Instantiate("ItemPrefabs/Hydro", transform.position, transform.rotation, 0);
        PhotonNetwork.Instantiate("ItemPrefabs/HPpotion", transform.position, transform.rotation, 0);
        //신발은 원소에서 z축으로 +2 만큼 떨어지게 생성
        Vector3 pos;
        pos = transform.position;
        pos.z += 2.0f;  
        PhotonNetwork.Instantiate("ItemPrefabs/Cryoshoes", pos, transform.rotation, 0);

        Destroy(gameObject);        //몬스터 Destroy

        //죽으면 물원소 활성화
        hydro.SetActive(true);
    }


    //몬스터의 플레이어 추적 범위 및 몬스터의 플레이어 공격 사정거리 기즈모 표시
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);


    }
}
