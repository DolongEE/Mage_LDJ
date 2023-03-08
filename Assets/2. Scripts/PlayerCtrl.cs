using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.EventSystems;

public class PlayerCtrl : MonoBehaviour
{
    public float playerHP;
    public float playerMaxHP = 100;
    public float moveSpeed = 0.25f;
    public float jumpForce = 40f;
    private bool isJump;
    private bool isAttack;
    private bool isSkillAttack;
    [HideInInspector]
    public bool isMove;

    private MainManager mainManager;
    private PlayerUiManager playerUi;
    public int selectPlayer;
    [HideInInspector]
    public bool isDie;

    public Animator myAnim;

    // 자신의 Transform 참조 변수  
    private Transform myTr;

    // 스킬 프리팹
    public GameObject prefab_skill;
    // 총알을 생성하기위한 프리팹
    public GameObject prefab_bullet;
    // 총알 memory pool위한 리스트 생성
    private List<GameObject> bulletPool = new List<GameObject>();
    // 생성할 총알 갯수
    public int bulletMaxCount = 5;
    // 현재 총알 인덱스
    private int currentBullet;
    // 총 나가는 위치
    private Transform firePos;
    // 총알 담을 오브젝트
    private GameObject magazine;

    #region 네트워크 변수
    Rigidbody rBody;
    // PhotonView 컴포넌트 할당
    PhotonView pv = null;

    // 위치 정보 송수신 시 변수
    Vector3 currPos = Vector3.zero;
    // 현재 위치의 각도 송수신할 변수
    Quaternion currRot = Quaternion.identity;

    #endregion

    [HideInInspector]
    // 카메라의 위치와 회전값
    public Transform eye;
    [HideInInspector]
    public int netNum;

    Transform respawnTr;

    Image playerHpBar;

    [HideInInspector]
    public bool waterJump;

    float wtime = 0;

    float landDam = 0;
    float landTime = 0;

    [HideInInspector]
    public bool getPyro;
    [HideInInspector]
    public bool getAnemo;
    [HideInInspector]
    public bool getGeo;
    [HideInInspector]
    public bool getHydro;

    [Header("효과")]
    //효과음 삽입
    public AudioClip[] playereffectsound;   //효과음 받음 

    private SoundUiManager _sMgr; //SoundManager 컴포넌트 연결   

    //장착이펙 파티클
    public GameObject footeft;     //신발효과

    //맵 들어오면 활성화될 토치, 낫
    public GameObject sickle;   //낫
    public GameObject torch;    //토치

    void Awake()
    {
        playerHP = playerMaxHP;
        netNum = 0;
        isMove = true;
        isJump = false;
        // 플레이어 선택 변수
        selectPlayer = 0;
        // csPhotonCharacterSelect 스크립트에서 좌우 버튼 클릭시 캐릭터 전환되는 정수 받아오기 위한 선언
        mainManager = GameObject.Find("MainManager").GetComponent<MainManager>();
        // 플레이어 위치값 불러옴
        myTr = GetComponent<Transform>();

        // 플레이어 애니메이션 불러옴
        myAnim = GetComponentInChildren<Animator>();

        firePos = transform.GetChild(5);

        // 시네머신 오브젝트를 찾아 스크립트 불러옴
        //cmManager = GameObject.Find("CinemachineManager").GetComponent<CMManager>();
        rBody = GetComponent<Rigidbody>();

        pv = GetComponent<PhotonView>();

        //PhotonView Observed Components 속성에 EnemyCtrl(현재) 스크립트 Component를 연결
        pv.ObservedComponents[0] = this;

        // 데이터 전송 타입 설정
        pv.synchronization = ViewSynchronization.UnreliableOnChange;

        magazine = GameObject.Find("Magazine");

        //playerHpBar = GameObject.Find("Hpline").GetComponent<Image>();

        if (pv.isMine)
        {
            // 카메라에 눈의 위치값을 전송함
            GameObject.FindGameObjectWithTag("CMCAMERA").GetComponent<CinemachineVirtualCameraBase>().Follow = eye;
            // 유저 아이디를 활성화
            GameObject.Find("PlayerUiManager").GetComponent<PlayerUiManager>().playerNickName.text = PlayerPrefs.GetString("USER_ID");
        }
        // 자신의 네트워크 객체가 아닐때
        else
        {
            // 물리력 사용하지 않겠다
            rBody.isKinematic = true;
        }
        pv.RPC("NetCharacter", PhotonTargets.OthersBuffered, netNum);
        // 원격 플레이어 위치, 회전 값 처리 변수 초기값 설정
        currPos = myTr.position;
        currRot = myTr.rotation;

        //효과음 삽입
        _sMgr = GameObject.Find("SoundUiManager").GetComponent<SoundUiManager>();
             
    }

    void Start()
    {
        // 최대 생성갯수 만큼 생성
        for (int i = 0; i < bulletMaxCount; i++)
        {
            // 총알 프리팹 생성
            GameObject bullet = Instantiate<GameObject>(prefab_bullet);
            bullet.transform.SetParent(magazine.transform);
            // 총알 비활성화
            bullet.gameObject.SetActive(false);
            // 총알 메모리풀에 저장
            bulletPool.Add(bullet);
        }
    }

    void Update()
    {
        if (selectPlayer != mainManager.selectPlayer && pv.isMine)
        {
            // 현재 자신 캐릭터만 변함
            CharacterSelect();
            pv.RPC("NetCharacter", PhotonTargets.OthersBuffered, netNum);
        }

        //if (Input.GetKeyDown(KeyCode.B))
        //{
        //    PhotonNetwork.LoadLevel("scEarthMap");
        //}
        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    PhotonNetwork.LoadLevel("scForestMap");
        //}
        //if (Input.GetKeyDown(KeyCode.N))
        //{
        //    PhotonNetwork.LoadLevel("scSnowMap");
        //}
        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    PhotonNetwork.LoadLevel("scCastle");
        //}
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    PhotonNetwork.LoadLevel("scFireMap");
        //}
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    GameObject.Find("Queen").GetComponent<ClearQueen>().GameClear();
        //}
        //if(Input.GetKeyDown(KeyCode.X))
        //{
        //    GameObject.FindGameObjectWithTag("Enemy").GetComponent<BossControllT>().hp = 7;
        //}
    }

    // Ray 또는 물리적 연산 처리할때 사용 하는 업데이트
    void FixedUpdate()
    {
        
        if (mainManager.isGameStart)
        {
            if (pv.isMine)
            {
                // UI가 켜져있을때 움직이지 않게 함
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    Move();
                    Attack();
                }
            }
            // 원격 캐릭터 일때
            else
            {
                // 원격 플레이어의 아바타를 수신 받은 위치까지 자연스럽게 이동
                myTr.position = Vector3.Lerp(myTr.position, currPos, Time.deltaTime * 10.0f);
                // 원격 플레이어의 아바타를 수신 받은 각도만큼 자연스럽게 회전
                myTr.rotation = Quaternion.Slerp(myTr.rotation, currRot, Time.deltaTime * 10.0f);
            }
        }
    }
    void Move()
    {
        // 키보드 w,a,s,d로 이동 
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(moveX, 0f, moveZ);

        if(Input.GetKeyDown(KeyCode.Z))
        {
            moveSpeed = 0.5f;

        }
        

        // 보고있는 방향으로 앞으로 이동
        transform.position += transform.TransformDirection(move) * moveSpeed;
        // 카메라가 바라보고 있는방향을 가리킴
        transform.rotation = new Quaternion(0, eye.rotation.y, 0, eye.rotation.w);

        // 움직일때 걷는 애니메이션
        if (Mathf.Abs(moveX) > 0 || Mathf.Abs(moveZ) > 0) 
        {
            myAnim.SetBool("isWalk", true);

            //효과음추가
            if (!_sMgr.asEffect.isPlaying)
            {
                 _sMgr.asEffect.PlayOneShot(playereffectsound[0]); //플레이어 효과음시작 
            //    _sMgr.asEffect.clip = playereffectsound[0]; //사운드매니져의 오디오소스에 플레이어안에 있는오디오 삽입
            //_sMgr.asEffect.Play();
            }
        }
        else
        {
         //   _sMgr.asEffect.Stop();  //걸음 멈추면 효과음 멈춤       
            myAnim.SetBool("isWalk", false);
        }
        
        if(waterJump)
        {
            wtime += Time.deltaTime;
            if (Input.GetButton("Jump") && wtime > 0.5f)
            {
                rBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                wtime = 0;
                myAnim.SetBool("isJump", true);
                //효과음 추가
                _sMgr.asEffect.PlayOneShot(playereffectsound[1]); //플레이어 수중점프때 효과음시작 
            }
        }
        // 점프 구현
        else if (Input.GetButton("Jump") && !isJump)
        {
            isJump = true;
            rBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            myAnim.SetBool("isJump", true);
            //효과음 추가
            _sMgr.asEffect.PlayOneShot(playereffectsound[2]); //플레이어 점프때 효과음시작 
        }

        if(isJump)
        {
            landTime += Time.deltaTime;
            if (landTime > 1f)
            {
                landDam += Time.deltaTime * 8f;
            }
        }
    }

    #region 공격함수

    void Attack()
    {
        // 마우스 왼쪽버튼 눌렀을 때 기본공격(1.3초 쿨타임 있음)
        if (Input.GetMouseButtonDown(0) && !isAttack)
        {
            // 기본공격(네트워크)
            pv.RPC("Normal", PhotonTargets.All, null);
        }
        // 마우스 오른쪽버튼 눌렀을 때 스킬공격(5초 쿨타임)
        if (Input.GetMouseButtonDown(1) && !isSkillAttack)
        {
            // 스킬공격(네트워크)
            pv.RPC("Skill", PhotonTargets.All, null);
        }
    }

    // 다른 사람들에게 총알 정보 보냄
    [PunRPC]
    void Normal()
    {
        // 기본공격 코루틴 시작
        StartCoroutine(AttackNormal());
    }

    // 기본공격
    IEnumerator AttackNormal()
    {
        // 쿨타임을 주기위한 공격 체크
        isAttack = true;
        // 기본공격 애니메이션
        myAnim.SetTrigger("Fire1");
        // 애니메이션과 총알 공격의 모션 맞추기위한 딜레이
        yield return new WaitForSeconds(0.3f);
        // 쏘려는 총알이 이미 발사중인 총알이라면 총알 발사 못함
        if (bulletPool[currentBullet].gameObject.activeSelf)
        {
            yield break;
        }
        // 총알이 자신이 바라보는 방향으로 발사
        bulletPool[currentBullet].transform.position = firePos.position;
        bulletPool[currentBullet].transform.rotation = eye.rotation;

        // 총알 활성화
        bulletPool[currentBullet].gameObject.SetActive(true);

        // 현재 총알 번호가 최대 총알 번호가 되면 다시 초기화
        if (currentBullet >= bulletMaxCount - 1)
        {
            currentBullet = 0;
        }
        else
        {
            currentBullet++;
        }
        // 기본공격 1.3초 딜레이줌
        yield return new WaitForSeconds(1.3f);
        isAttack = false;
    }

    [PunRPC]
    void Skill()
    {
        StartCoroutine(AttackSkill());
    }

    IEnumerator AttackSkill()
    {
        isSkillAttack = true;
        myAnim.SetTrigger("Fire2");
        Instantiate(prefab_skill, firePos.position, eye.rotation);
        _sMgr.asEffect.PlayOneShot(playereffectsound[5], 1.3f); //사운드
        yield return new WaitForSeconds(5.0f);
        isSkillAttack = false;
    } 
    #endregion

    void CharacterSelect()
    {
        transform.GetChild(selectPlayer).gameObject.SetActive(false);
        selectPlayer = mainManager.selectPlayer;
        netNum = selectPlayer;
        transform.GetChild(selectPlayer).gameObject.SetActive(true);
    }

    [PunRPC]
    void NetCharacter(int netNum)
    {
        if (!pv.isMine)
        {
            for (int i = 0; i < 4; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            transform.GetChild(netNum).gameObject.SetActive(true);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "GROUND")
        {
            isJump = false;
            myAnim.SetBool("isJump", false);
            //효과음추가
            _sMgr.asEffect.PlayOneShot(playereffectsound[3]); //플레이어 착지때 효과음시작 

            playerHP -= landDam;
            playerHpBar.fillAmount = playerHP / playerMaxHP;
            landDam = 0;
            landTime = 0;
        }
        if (collision.gameObject.tag == "EnemyWeapon")
        {
            playerHP -= collision.gameObject.GetComponent<EnemyBullet>().power;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "EnemyWeapon")
        {
            playerHP -= other.gameObject.GetComponent<EnemyWeapon>().power;
        }
    }

    public void UpdateHp()
    {
        playerHpBar = GameObject.Find("Hpline").GetComponent<Image>();
        StartCoroutine(NowHp());
    }

    IEnumerator NowHp()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            playerHpBar.fillAmount = playerHP / playerMaxHP;

            if (isDie)
            {
                StartCoroutine(RespawnPlayer());
            }
            else if (playerHP <= 0)
            {
                isDie = true;
            }
            else if (playerHpBar.fillAmount <= 0.4f)
            {
                playerHpBar.color = Color.yellow;
            }
            
            if(isDie)
            {
                yield return null;
            }
        }        
    }

    IEnumerator RespawnPlayer()
    {
        // RespawnTr라는 빈게임 오브젝트와 태그 추가
        respawnTr = GameObject.FindGameObjectWithTag("Respawn").transform;
        this.gameObject.transform.position = respawnTr.position;
        isDie = false;
        
        playerHP = playerMaxHP;
        StartCoroutine(NowHp());
        yield return null;
    }

    public void GetHealPotion(int potionPoint)
    {
        Debug.Log(potionPoint + "회복됨");
        playerHP += potionPoint;
        if(playerHP >= playerMaxHP)
        {
            playerHP = playerMaxHP;
        }
    }

    void OnPhotonInstantiate()
    {
        if (pv.isMine)
        {
            for (int i = 0; i < 4; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    // 네트워크 통신 할때마다 이 함수가 호출됨
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //로컬 플레이어의 위치 정보를 송신
        if (stream.isWriting)
        {
            //박싱
            stream.SendNext(myTr.position);
            stream.SendNext(myTr.rotation);

        }
        //원격 플레이어의 위치 정보를 수신
        else
        {
            //언박싱
            currPos = (Vector3)stream.ReceiveNext();
            currRot = (Quaternion)stream.ReceiveNext();

        }
    }
    //낫 활성화
    public void SickleOn()
    {       
        sickle.SetActive(true);  //낫 활성화               
    }
    public void SickleOff()
    {
        sickle.SetActive(false);  //낫 활성화               
    }
    //토치 활성화
    public void TorchOn()
    {
        torch.SetActive(true);  //토치 활성화
    }
    public void TorchOff()
    {
        torch.SetActive(false);  //토치 활성화
    }
}
