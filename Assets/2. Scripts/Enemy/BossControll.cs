using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;   //NavMeshAgnet 사용하기 위한 네임스페이스 


public class BossControll : MonoBehaviour
{
    public NavMeshAgent agent;      //NavMeshAgent 변수 선언

    public Transform player;        //플레이어 위치 변수 선언

    private Transform myTr;         //플레이어 위치 값 참조 변수 선언
    private Transform traceTarget;  //추적할 타겟의 위치 변수 선언

    private Animator Anim;          //Animator 변수 선언

    public LayerMask whatIsGround;  //그라운드의 레이어 체크 할 레이어 마스크 변수 선언
    public LayerMask whatIsPlayer;  //플레이어의 레이어를 체크 할 레이어 마스크 변수 선언

    public float health;            //보스 체력 변수 설정

    //Patroling
    public Vector3 walkPoint;       //패트롤 때 사용할 워크 포인트 백터 변수 선언
    bool walkPointSet;              //walkPointSet 상태체크 변수 선언
    public float walkPointRange;    //패트롤 워크 포인트 범위 설정 값 변수 선언

    //Attacking
    public float timeBetweenAttacks;    //공격 쿨타임 변수 선언
    bool alreadyAttacked;               //어택 여부 확인 변수 선언
    public GameObject[] Weapon;         //무기 배열 선언
    public GameObject SkillBullet;      //일반 공격 및 스킬 변수 선언

    //States
    public float sightRange;            //플레이어 추적 범위 변수 선언
    public float attackRange;           //플레이어 추적 후 어택 범위 변수 선언
    public bool playerInSightRange;     //플레이어 추적 범위에 들어왔는지 체크하기위한 변수 선언
    public bool playerInAttackRange;    //플레이어 어택 범위에 들어왔는지 체크하기 위한 변수 선언

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();               //NavMesh 설정
        Anim = GetComponentInChildren<Animator>();          //Animator 설정        
    }

    private void Update()
    {
        //Check for sight and attack range
        //플레이어 추적 범위 설정
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        //플레이어 공격 범위 설정
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);


        //플레이어가 공격범위에 있고, 추적범위에도 있을 때 플레이어 공격 시작
        if (playerInAttackRange && playerInSightRange)
        {
            Debug.Log("공격 시작");
            //agent.isStopped = true;
            transform.LookAt(player);   //플레이어를 바라본다.
            AttackPlayer();
        }
        
        //플레이어가 추적범위에 있고, 공격 범위에 있을 때 플레이어 추적
        else if(playerInSightRange && !playerInAttackRange)
        {
            Debug.Log("추적");
            //agent.isStopped = false;
            Anim.SetBool("InGameWalk", true);   //워크 애니메이션 on
            transform.LookAt(player);   //플레이어를 바라본다.
            ChasePlayer();
        }

        //플레이어가 추적범위에 없고, 공격 범위에도 없을 때 몬스터 패트롤
        else if (!playerInSightRange && !playerInAttackRange)
        {
            Debug.Log("패트롤");
            //agent.isStopped = false;
            Anim.SetBool("InGameWalk", true);   //워크 애니메이션 on
            Patroling();
        }
    }

    //몬스터 패트롤
    private void Patroling()
    {
        if (!walkPointSet)      //워크 포인트가 false 이면 워크포인트를 찾는 serachPoint 함수 실행
        {
            SearchWalkPoint();
        }
        if (walkPointSet)       //워크 포인트가 true이면 워크포인트로 매쉬 이동 (패트롤 실행)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;   //현재 트랜스폼에서 워크포인트까지 뺀 거리가 워크포인트까지의 이동거리

        //Walkpoint reached
        if(distanceToWalkPoint.magnitude < 1f)  //워크포인트까지의 이동거리가 1보다 작으면 walkPointSet false 즉, 다시 워크포인트 찾기 실행
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

        if(Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    //플레이어 추적
    private void ChasePlayer()
    {
        transform.LookAt(player);   //플레이어를 바라본다.
        agent.SetDestination(player.position);      //플레이어쪽으로 이동
        Anim.SetFloat("Speed", GetComponent<Rigidbody>().velocity.z);   
        Anim.SetBool("InGameWalk", true);   //워크 애니메이션 이동
    }
    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        //agent.SetDestination(transform.position);
       
        
        transform.LookAt(player);   //플레이어를 바라본다.

        if(!alreadyAttacked)
        {
            Anim.SetBool("isAttack1", true);    //어택 애니메이션 실행
            //Attack code here

            Debug.Log("어택애니메이션 실행");
            Rigidbody rb = Instantiate(SkillBullet, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), Quaternion.identity).GetComponent<Rigidbody>();

            rb.AddForce(transform.forward * 40f, ForceMode.Impulse);
            rb.AddForce(transform.up * 0f, ForceMode.Impulse); 

            alreadyAttacked = true; //공격 나간 것 체크하는 변수 
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
            
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
        //Anim.SetBool("isAttack1", false);

    }

    private void TakeDamage(int damage)
    {
        health -= damage;

        if(health <= 0)
        {
            Invoke(nameof(DestroyEnemy), 5f);
        }
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
