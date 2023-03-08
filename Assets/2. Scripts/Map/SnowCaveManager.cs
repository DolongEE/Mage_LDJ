using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowCaveManager : MonoBehaviour
{
    //맵 들어오면 활성화될 토치, 낫
    public PlayerCtrl player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //플레이어가 나중에 생성되므로 시작시 연결
        player.TorchOn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
