#define CBT_MODE
#define RELEASE_MODE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//네비게이션을 위한 네임스페이스 추가
using UnityEngine.AI;

// 간단하게 쓰기위해...
using Rand = UnityEngine.Random;
//공부용 네임스페이스 추가 본 소스랑 상관없다 나중에 지워주자
//using UnityEngine.iOS;

/*
 * 클래스는 [System.Serializable] Attribute를 명시 하여야
 * Inspector 뷰에 노출 
 */
[System.Serializable]
//애니메이션 클립을 저장할 클래스 
public class Anim
{
    public AnimationClip idle;
    public AnimationClip walk;

    public AnimationClip attack1;
    public AnimationClip attack2;
    public AnimationClip skillEffect;
    public AnimationClip roar;
    public AnimationClip getHit1;
    public AnimationClip getHit2;
    public AnimationClip fly;
    public AnimationClip die;
    
}

////필요한 컴포넌트를 위한 어트리뷰트 선언 
//[RequireComponent(typeof(AudioSource))]

//다른 게임오브젝트에 현재 제작한 컴포넌트를 연결하고 싶을때 이 어트리뷰트를 사용하면 메뉴에 => Component에 추가된다.
[AddComponentMenu("EnemyCtrl/Follow EnemyCtrl")]

public class BossControllT : MonoBehaviour
{
    //인스펙터의 헤더의 표현을 위한 어트리뷰트 선언
    [Header("ANIMATION")]
    //인스펙터뷰에 노출시킬 Anim 클래스 변수 
    public Anim anims;
    //하위에 있는 모델의 Animation 컴포넌트에 접근하기 위한 레퍼런스
    private Animation _anim;
    //애니메이션 상태 저장 
    AnimationState animState;
   

    //각종 연결 레퍼런스 선언 및 변수 선언

    //NavMeshAgent 연결 레퍼런스
    private NavMeshAgent myTraceAgent;

    //자신과 타겟 Transform 참조 변수  
    private Transform myTr;
    private Transform traceTarget;

    //추적을 위한 변수
    private bool traceObject;
    private bool traceAttack;

    

    //추적 대상 거리체크 변수 
    public float dist1;
    float dist2;

    //플레이어를 찾기 위한 배열 
    [SerializeField]
    private GameObject[] players;
    private Transform playerTarget;



    // public 멤버 인스펙터에 노출을 막는 어트리뷰트
    // 인스펙터에 노출은 막고 외부 노출은 원하는 경우 사용
    [HideInInspector]
    //죽었는지 상태변수 
    public bool isDie;

    //에너미가 받는 데미지 설정
    public float Bulletdamage = 10f; //기본 10값으로 설정

    /* [System.NonSerialized] 와 [HideInInspector]는 기능은 같지만 
     * [HideInInspector]는 인스펙터에서 수정한 값을 그대로 가져가지만
     * [System.NonSerialized]는 인스펙터에서 값을 저장하여도 실행시 디폴트 값이 나오는걸 볼 수있다.
     */

    // Enemy의 현재 상태정보를 위한 Enum 자료형 선언  
    public enum MODE_STATE { idle = 1, move, trace, attack1,attack2,skill, hit, die };

  

    //변수들의 간격을 위한 어트리뷰트 선언(보기 좋다)
    [Space(20)]
    //인스펙터의 헤더의 표현을 위한 어트리뷰트 선언
    [Header("STATE")]
    //Enemy의 상태 셋팅
    public MODE_STATE enemyMode = MODE_STATE.idle;

    //변수들의 간격을 위한 어트리뷰트 선언(보기 좋다)
    [Space(10)]
    //인스펙터의 헤더의 표현을 위한 어트리뷰트 선언
    [Header("몬스터 인공지능")]

    //변수들의 간격을 위한 어트리뷰트 선언(보기 좋다)
    [Space(5)]

    //변수에 팁을 달아줄 수  있다.(인스펙터에서 확인)
    [Tooltip("몬스터의 HP")]
    //Enemy의 HP 셋팅 
    [Range(0, 1000)] public float hp = 100f;

    //변수에 팁을 달아줄 수  있다.(인스펙터에서 확인)
    [Tooltip("몬스터의 속도")]
    //Enemy의 속도 셋팅, [SerializeField] 는 [HideInInspector] 와 반대 속성
    [Range(1f, 30f)] public float speed = 10f;

    //거리에 따른 상태 체크 변수 
    
    [Tooltip("몬스터 추적거리!!!")]
    [Range(100f, 200f)] [SerializeField] float traceDist = 150f;
    [Tooltip("몬스터 단거리 소드 공격거리")]
    [Range(10f, 100f)] [SerializeField] float attack1Dist = 25f;
    [Tooltip("몬스터 원거리 채찍 공격거리")]
    [Range(10f, 100f)] [SerializeField] float attack2Dist = 50f;


    //인공지능 부여 변수(테스트 변수로 나중에 private 선언)
    [Header("TEST")]
    // [SerializeField] 는 [HideInInspector] 와 반대 속성
    [SerializeField] private bool isHit;
   
    
   

    // 포톤 추가///////////////////////////////////////////////////////////////////////

    //참조할 컴포넌트를 할당할 레퍼런스 (미리 할당하는게 좋음)
    Rigidbody rigid;

    //PhotonView 컴포넌트를 할당할 레퍼런스 
    PhotonView pv = null;

    //위치 정보를 송수신할 때 사용할 변수 선언 및 초기값 설정 
    Vector3 currPos = Vector3.zero;
    Quaternion currRot = Quaternion.identity;

    // 애니메이션 동기화를 위한 변수
    // RPC로 처리해도 됨...선택은 상황에 따라서...
    int net_Aim = 0;

    [Header("효과음")]
    //효과음 삽입
    public AudioClip[] Bosseffectsound;   //효과음 받음 
    private SoundUiManager _sMgr; //SoundManager 컴포넌트 연결

    //죽었을때 대지 원소 활성화
    public GameObject geo;

    void Awake()
    {

        // 포톤 추가
        // 만약 Start 함수에서 초기화 했다면 Start 함수를 Awake 함수로 변경 ( 기존  Start 함수에서 처리 할 경우 
        // myTr = GetComponent<Transform>(); 전에 OnPhotonSerializeView 콜백 함수가 먼저 호출될 경우 Null Reference 오류 발생)


        ///////////////////////////////////////////////////////////////
        //레퍼런스 할당 
        myTraceAgent = GetComponent<NavMeshAgent>();

        //자신의 자식에 있는 Animation 컴포넌트를 찾아와 레퍼런스에 할당 
        _anim = GetComponentInChildren<Animation>();

        // 포톤 추가
        // 네트워크 동기화를 위한 애니메이션
        net_Aim = 0;

        //자기 자신의 Transform 연결
        myTr = GetComponent<Transform>();

        rigid = GetComponent<Rigidbody>();

    
        //PhotonView 컴포넌트 할당 
        pv = GetComponent<PhotonView>();


        //PhotonView Observed Components 속성에 PlayerCtrl(현재) 스크립트 Component를 연결
        pv.ObservedComponents[0] = this;

        //데이타 전송 타입을 설정
        pv.synchronization = ViewSynchronization.UnreliableOnChange;

        //자신의 네트워크 객체가 아닐때...(마스터 클라이언트가 아닐때)
        if (!PhotonNetwork.isMasterClient)
        {
      
            //원격 네트워크 플레이어의 아바타는 물리력을 이용하지 않음 (마스터 클라이언트가 아닐때)
            //(원래 게임이 이렇다는거다...우리건 안해도 체크 돼있음...)
            rigid.isKinematic = true;
            //네비게이션도 중지
            //myTraceAgent.isStopped = true; 이걸로 하면 off Mesh Link 에서 에러 발생 그냥 비활성 하자
            myTraceAgent.enabled = false;
        }

  
        currPos = myTr.position;
        currRot = myTr.rotation;

        ///////////////////////////////////////////////////////////////
       
        //효과음삽입
        _sMgr = GameObject.Find("SoundUiManager").GetComponent<SoundUiManager>();
    }

    // Use this for initialization
    IEnumerator Start()
    {


        //Animation 컴포넌트의 clip속성에 idle1 애니메이션 클립 지정 
        _anim.clip = anims.idle;
        //지정한 애니메이션 클립(애니메이션) 실행 
        _anim.Play();

        //포톤 추가//////////////////////////////////
        // 마스터 클라이언트만 수행
        if (PhotonNetwork.isMasterClient)
        {
            yield return new WaitForSeconds(0.3f);            

            players = GameObject.FindGameObjectsWithTag("Player");  //추가  630번째 줄 스크립트 복사
            //일단 첫 Base의 Transform만 연결
            traceTarget = players[0].transform;

            //추적하는 대상의 위치(Vector3)를 셋팅하면 바로 추적 시작 (가독성이 좋다)
            myTraceAgent.SetDestination(traceTarget.position);
            // 위와 같은 동작을 수행하지만...가독성이 별로다
            // myTraceAgent.destination = traceObj.position;

            // 정해진 시간 간격으로 Enemy의 Ai 변화 상태를 셋팅하는 코루틴
            StartCoroutine(ModeSet());

            // Enemy의 상태 변화에 따라 일정 행동을 수행하는 코루틴
            StartCoroutine(ModeAction());

            // 일정 간격으로 주변의 가장 가까운 Base와 플레이어를 찾는 코루틴 
            StartCoroutine(this.TargtSetting());

            
        }
        // 포톤 추가
        // 마스터 클라이언트가 아닐때 네트워크 객체를 일정 간격으로 애니메이션을 동기화 하는 코루틴
        else
        {
            StartCoroutine(this.NetAnim());
        }

        ////////////////////////////////////////////////

        yield return null;
    }
    private void OnCollisionEnter(Collision col)
    {
        Debug.Log("Bullet 콜라이더 충돌 체크");
        if(col.gameObject.tag == "Bullet")
        {
            if (!isDie)
            {
                isHit = true;
                //공격 받았을 경우 
                hp -= Bulletdamage;
                if (hp <= 0)
                {
                    //효과음추가 (다이)
                    _sMgr.asEffect.PlayOneShot(Bosseffectsound[2]);        
                    
                    isDie = true;
                    EnemyDie();
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {               

        ////공격 받았을 경우 
        //if (isHit)
        //{
        //    hp -= Bulletdamage;
        //    if (hp <= 0)
        //    {
        //        isDie = true;
        //        EnemyDie();
        //    }
        //}


        //적을 봐라봄  
        /*  if (enemyLook)
            {
                if (Time.time > enemyLookTime)
                {
                    //enemyLookRotation.eulerAngles = new Vector3(myTr.rotation.x, enemyLookRotation.eulerAngles.y, myTr.rotation.z); // 버그시...

                    //	enemyLookRotation = Quaternion.LookRotation(-(enemyTr.forward)); // - 해줘야 바라봄  
                    enemyLookRotation = Quaternion.LookRotation(enemyTr.position - myTr.position); // - 해줘야 바라봄  
                    myTr.rotation = Quaternion.Lerp(myTr.rotation, enemyLookRotation, Time.deltaTime * 10.0f);
                    enemyLookTime = Time.time + 0.01f;
                }
            }*/

        //포톤 추가
        // 마스터 클라이언트가 직접 Ai 및 애니메이션 로직 수행
        // pv.isMine 해도 됨
        if (PhotonNetwork.isMasterClient)
        {
            // 처리
        }
        //포톤 추가
        //원격 플레이어일 때 수행
        else
        {
            //원격 플레이어의 아바타를 수신받은 위치까지 부드럽게 이동시키자
            myTr.position = Vector3.Lerp(myTr.position, currPos, Time.deltaTime * 3.0f);
            //원격 플레이어의 아바타를 수신받은 각도만큼 부드럽게 회전시키자
            myTr.rotation = Quaternion.Slerp(myTr.rotation, currRot, Time.deltaTime * 3.0f);
        }


    }

    /*
     *  Enemy의 변화 상태에 따라 일정 행동을 수행하는 코루틴
     */
    IEnumerator ModeSet()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.2f);

            //자신과 Player의 거리 셋팅 
            float dist = Vector3.Distance(myTr.position, traceTarget.position);
            dist1 = dist;
            // 순서 중요
            if (isHit)  //공격 받았을시
            {
                enemyMode = MODE_STATE.hit;
            }
            else if (dist <= attack1Dist) // Attack 사거리에 들어왔는지 ??
            {
                enemyMode = MODE_STATE.attack1; //몬스터의 상태를 공격으로 설정 
            }
            else if (dist <= attack2Dist) // Attack 사거리에 들어왔는지 ??
            {
                enemyMode = MODE_STATE.attack2; //몬스터의 상태를 공격으로 설정 
            }
            else if (traceAttack)  // 몬스터를 추적중이라면...
            {
                enemyMode = MODE_STATE.trace; //몬스터의 상태를 추적으로 설정
            }
            else if (dist <= traceDist) // Trace 사거리에 들어왔는지 ??
            {
                enemyMode = MODE_STATE.trace; //몬스터의 상태를 추적으로 설정 
            }    
            
            else
            {
                enemyMode = MODE_STATE.idle; //몬스터의 상태를 idle 모드로 설정 
            }
        }
    }
    /*
	 * Enemy의 상태 변화에 따라 일정 행동을 수행하는 코루틴
	 */
    IEnumerator ModeAction()
    {
        while (!isDie)
        {
            switch (enemyMode)
            {
                //Enemy가 Idle 상태 일때... 
                case MODE_STATE.idle:

                    //네비게이션 멈추고 (추적 중지)
                    myTraceAgent.isStopped = true;
                    net_Aim = 0;
                    break;

                //Enemy가 Trace 상태 일때... 
                case MODE_STATE.trace:

                    // 네비게이션 재시작(추적)
                    myTraceAgent.isStopped = false;
                    // 추적대상 설정(플레이어)
                    myTraceAgent.destination = traceTarget.position;

                    //네비속도 및 애니메이션 속도 제어
                    

                        // 네비게이션의 추적 속도를 현재보다 1.0배
                        myTraceAgent.speed = speed * 1.0f;

                        //애니메이션 속도 변경
                        _anim[anims.walk.name].speed = 1.0f;

                        //run 애니메이션 
                        _anim.CrossFade(anims.walk  .name, 0.3f);

                        // 포톤 추가
                        // 애니메이션 동기화
                        net_Aim = 1;

                    break;

                //근거리 공격 상태
                case MODE_STATE.attack1:

                    //사운드 (공격) 검
                  //  _sMgr.asEffect.PlayOneShot(Bosseffectsound[0]);              

                    //추적 중지(선택사항)
                    //네비게이션 멈추고 (추적 중지) 
                    myTraceAgent.isStopped = true;

                    //애니메이션 속도 변경
                    _anim[anims.attack1.name].speed = 1.0f;

                    //walk 애니메이션 
                    _anim.CrossFade(anims.attack1.name, 0.3f);

                    //공격할때 적을 봐라 봐야함 
                    // myTr.LookAt(traceTarget.position); // 바로 봐라봄
                    // enemyLookRotation.eulerAngles = new Vector3(myTr.rotation.x, enemyLookRotation.eulerAngles.y, myTr.rotation.z);
                    Quaternion enemyLookRotation1 = Quaternion.LookRotation(traceTarget.position - myTr.position); // - 해줘야 바라봄  
                    myTr.rotation = Quaternion.Lerp(myTr.rotation, enemyLookRotation1, Time.deltaTime * 10.0f);
                    net_Aim = 2;

                    break;

                //원거리 공격 상태
                case MODE_STATE.attack2:

                    //사운드 (공격) 채찍
                    //_sMgr.asEffect.PlayOneShot(Bosseffectsound[1]);

                    //추적 중지(선택사항)
                    //네비게이션 멈추고 (추적 중지) 
                    myTraceAgent.isStopped = true;

                    //애니메이션 속도 변경
                    _anim[anims.attack2.name].speed = 1.0f;

                    //walk 애니메이션 
                    _anim.CrossFade(anims.attack2.name, 0.3f);

                    //공격할때 적을 봐라 봐야함 
                    // myTr.LookAt(traceTarget.position); // 바로 봐라봄
                    // enemyLookRotation.eulerAngles = new Vector3(myTr.rotation.x, enemyLookRotation.eulerAngles.y, myTr.rotation.z);
                    Quaternion enemyLookRotation2 = Quaternion.LookRotation(traceTarget.position - myTr.position); // - 해줘야 바라봄  
                    myTr.rotation = Quaternion.Lerp(myTr.rotation, enemyLookRotation2, Time.deltaTime * 10.0f);
                    net_Aim = 3;

                    break;

                //이동 상태 
                case MODE_STATE.move:

                    // 네비게이션 재시작(추적)
                    myTraceAgent.isStopped = false;
               
                        // 네비게이션의 추적 속도를 현재 속도로...
                        myTraceAgent.speed = speed;

                        //애니메이션 속도 변경
                        _anim[anims.walk.name].speed = 1.0f;

                        //walk 애니메이션 
                        _anim.CrossFade(anims.walk.name, 0.3f);

                        // 포톤 추가
                        // 애니메이션 동기화
                        net_Aim = 4;


                    break;

              

                //hit 상태  (여러가지 생각해야함 )
                //Enemy가 hit 상태 일때... 
                case MODE_STATE.hit:

                    //네비게이션 멈추고 (추적 중지)
                    myTraceAgent.isStopped = true;

                    //애니메이션 속도 변경
                    _anim[anims.getHit1.name].speed = 1.0f;

                    //getHit 애니메이션 
                    _anim.CrossFade(anims.getHit1.name, 0.3f);
                    isHit = false;
                    net_Aim = 5;
                    break;

            }


            yield return null;
        }
    }

    // MODE_STATE.surprise 에서 호출 되는 함수 (이거없으면 계속 으르렁됨)
    IEnumerator TraceObject()
    {
        yield return new WaitForSeconds(2.5f);
        traceAttack = true;

        yield return new WaitForSeconds(5.5f);
        traceAttack = false;
        traceObject = false;
    }

    // 자신과 가장 가까운 적을 찾음...플레이어가 베이스보다 우선순위가 높게 셋팅 추가
    IEnumerator TargtSetting()
    {
        while (!isDie)
        {

            yield return new WaitForSeconds(0.2f);

            // 자신과 가장 가까운 플레이어 찾음
            players = GameObject.FindGameObjectsWithTag("Player");

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


        }
    }

    /* Vetor3.Distance || Vetor3.sqrMagnitude 
     * 
     * Vetor3.Distance : 두 점간의 거리를 구해주는 메소드
     * Vetor3.sqrMagnitude : 두 점간의 거리의 제곱에 값 (두 점간의 거리의 차이를 2차원 함수의 값으로 계산)
     *                       루트 연산을 하지 않으므로, 연산속도가 빠르다. 하지만 정확한 거리가 측정되지 않기 때문에
     *                       정확한 거리는 몰라도 되고 거리를 비교할 때 사용된다.
     * 
     * 연산속도 : Vetor3.Distance < Vetor3.sqrMagnitude (퍼포먼스 향상)
     * 정확성   : Vetor3.Distance > Vetor3.sqrMagnitude
     */

    //로밍 함수
    

    // (추가)

    // 몬스터 사망 처리
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
        //죽는 애니메이션 시작
        _anim.CrossFade(anims.die.name, 0.3f);

        // 포톤 추가
        // 애니메이션 동기화
        net_Aim = 6;

        //Enemy의 모드를 die로 설정
        enemyMode = MODE_STATE.die;

        //네비게이션 멈추고 (추적 중지) 
        myTraceAgent.isStopped = true;

        //4.5 초후 오브젝트 삭제
        yield return new WaitForSeconds(2f);

        //죽으면 대지원소 활성화
        geo.SetActive(true);

        // 포톤 추가
        //죽었을때 아이템(심장) 드랍
        Vector3 pos;
        pos = transform.position;
        pos.y += 2.0f; //위쪽로 2만큼 위치에
        PhotonNetwork.Instantiate("ItemPrefabs/Orichalcumheart", pos, transform.rotation, 0);
        // 자신과 네트워크상의 모든 아바타를 삭제
        PhotonNetwork.Destroy(gameObject);
    }
      

    

    //객체 소멸시 정리가 필요한 부분은 여기서...
    void OnDestroy()
    {
        //Debug.Log("Destroy");
        //모든 코루틴을 정지시키자
        StopAllCoroutines();
    }      


    // 포톤 추가///////////////////////////////////////////////////////////////////////

    /*
	 * 마스터 클라이언트가 아닐때 애니메이션 상태를 동기화 하는 로직
     * RPC 로도 애니메이션 동기화 가능~!
	 */
    IEnumerator NetAnim()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.2f);

            if (!PhotonNetwork.isMasterClient)
            {
                if (net_Aim == 0)
                {
                    _anim.CrossFade(anims.idle.name, 0.3f);
                }
                else if (net_Aim == 1)
                {
                    //attack1 애니메이션 
                    _anim.CrossFade(anims.attack1.name, 0.3f);
                }
                else if (net_Aim == 2)
                {
                    //attack2 애니메이션 
                    _anim.CrossFade(anims.attack2.name, 0.3f);
                }
                else if (net_Aim == 3)
                {
                    //attack3 애니메이션 
                    _anim.CrossFade(anims.skillEffect.name, 0.3f);
                }

                else if (net_Aim == 4)
                {
                    //애니메이션 속도 변경
                    _anim[anims.walk.name].speed = 1.0f;

                    //walk 애니메이션 
                    _anim.CrossFade(anims.walk.name, 0.3f);
                }
   
                else if (net_Aim == 5)
                {
                    // hit1 애니메이션 
                    _anim.CrossFade(anims.getHit1.name, 0.3f);
                }
               
                else if (net_Aim == 6)
                {
                    //죽는 애니메이션 시작
                    _anim.CrossFade(anims.die.name, 0.3f);

                    // 코루틴 함수를 빠져나감(종료)
                    yield break;
                }
            }
        }
    }

    /*
     * PhotonView 컴포넌트의 Observe 속성이 스크립트 컴포넌트로 지정되면 PhotonView
     * 컴포넌트는 데이터를 송수신할 때, 해당 스크립트의 OnPhotonSerializeView 콜백 함수를 호출한다. 
     */
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //로컬 플레이어의 위치 정보를 송신
        if (stream.isWriting)
        {
            //박싱
            stream.SendNext(myTr.position);
            stream.SendNext(myTr.rotation);
            stream.SendNext(net_Aim);
        }
        //원격 플레이어의 위치 정보를 수신
        else
        {
            //언박싱
            currPos = (Vector3)stream.ReceiveNext();
            currRot = (Quaternion)stream.ReceiveNext();
            net_Aim = (int)stream.ReceiveNext();
        }
    }     
}
