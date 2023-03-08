using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBullet : MonoBehaviour
{
    // 총알을 생성하기위한 프리팹
    public GameObject prefab_bullet;
    // 총알 memory pool위한 리스트 생성
    private List<GameObject> bulletPool = new List<GameObject>();
    // 생성할 총알 갯수
    public int bulletMaxCount = 10;
    // 현재 총알 인덱스
    private int currentBullet;
    // 총알을 저장할 탄창 위치
    private Transform magazine;

    private void Awake()
    {
        magazine = GameObject.Find("Magazine").transform;
    }

    void Start()
    {
        // 최대 생성갯수 만큼 생성
        for (int i = 0; i < bulletMaxCount; i++)
        {
            // 총알 프리팹 생성
            GameObject bullet = Instantiate<GameObject>(prefab_bullet);
            bullet.transform.SetParent(magazine);
            // 총알 비활성화
            bullet.gameObject.SetActive(false);
            // 총알 메모리풀에 저장
            bulletPool.Add(bullet);
        }
    }
    void Update()
    {
        // 왼쪽마우스 버튼을 눌렀을때 총알 발사
        if(Input.GetMouseButtonDown(0))
        {
            // 쏘려는 총알이 이미 발사중인 총알이라면 총알 발사 못함
            if(bulletPool[currentBullet].gameObject.activeSelf)
            {
                return;
            }

            // 총알이 자기자신으로부터 발사 (후에 위치조정 필요)
            bulletPool[currentBullet].transform.position = this.transform.Find("FirePos").position;

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
        }
    }
}
