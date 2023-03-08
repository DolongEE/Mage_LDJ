using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class BulletCtrl : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip bulletClip;
    public AudioClip explosionClip;
    public ParticleSystem particle;
    // 날아가는 속도 조절
    public float speed = 0.6f;
    // trigger를 위한 리지드바디
    private Rigidbody rigid;
    // 처음 생성됬을때 콜라이더 범위에 적을 찾기위한 콜라이더
    private BoxCollider boxCollider;
    // 적과 자신 위치 거리
    private float dis;
    // 적을 찾는 최대거리
    public float findMaxDis;
    // 적 게임오브젝트 찾음
    private GameObject[] enemies;
    // 적 위치
    public Transform enemyTr;
    public int power = 10;

    bool first1;
    bool first2;
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        audioSource = GameObject.Find("SoundUiManager").GetComponent<SoundUiManager>().asEffect;
    }

    void FixedUpdate()
    {
        FollowBullet();
    }

    // 총알이 적을 따라가는 함수 구현
    void FollowBullet()
    {
        // 적위치 못찾았을때 앞으로 이동
        if (enemyTr == null)
        {
            // 앞으로 이동
            transform.position += transform.forward * speed;
            return;
        }
        // 앞으로 이동
        transform.position += transform.forward * speed;

        // 매 프레임 마다 총알이 적이 있는 방향을 바라봄(적을 따라가게 됨)
        Quaternion enemyLookRotation = Quaternion.LookRotation(enemyTr.position - this.transform.position);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, enemyLookRotation, Time.deltaTime * 10.0f);
    }

    // 가장 가까운 적을 찾음
    void TargetSetting()
    {
        // 적 오브젝트들을 받아옴
        enemies = GameObject.FindGameObjectsWithTag("EnemyPoint");
        //적이 있을경우 
        if (enemies.Length != 0)
        {
            // 가장 가까운 적 설정
            enemyTr = enemies[0].transform;
            // 적과 나의 거리를 절대값으로(sqrManitued) 계산 
            dis = (enemyTr.position - this.transform.position).sqrMagnitude;
            // 적이 찾는 범위 내에 있을때
            if (dis > findMaxDis)
            {
                foreach (GameObject _enemies in enemies)
                {
                    // 제일 가까운 적을 찾음
                    if ((_enemies.transform.position - this.transform.position).sqrMagnitude < dis)
                    {
                        enemyTr = _enemies.transform;
                    }
                }
            }
        }
    }

    // 총알 생성시 0.5초내에 적이 범위에 없으면 콜라이더 제거
    void CheckEnemy()
    {
        // 적을 찾는 콜라이더 제거
        boxCollider.enabled = false;
    }

    // 총알 활성화 시간
    void SetFalseTime()
    {
        // 총알 유지후 비활성화
        this.gameObject.SetActive(false);
    }

    // 처음 생성시 적이 트리거에 걸리면 유도탄 발사
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            TargetSetting();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        gameObject.SetActive(false);
    }

    // 총알이 활성화 되었을때
    void OnEnable()
    {
        // 총알 생성시 0.5초내에 적이 범위에 없으면 콜라이더 비활성화
        Invoke("CheckEnemy", 1f);
        // 총알이 활성화 되는 시간
        Invoke("SetFalseTime", 2f);
        if (!first1)
        {
            first1 = true;
        }
        else
        {
            particle.Play();
            audioSource.PlayOneShot(bulletClip);
        }
    }

    // 총알이 비활성화 되었을때
    void OnDisable()
    {
        enemyTr = null;
        boxCollider.enabled = true;
        enemies = null;
        if (!first2)
        {
            first2 = true;
        }
        else
        {
            particle.Play();
            audioSource.PlayOneShot(explosionClip);
        }
    }
}
