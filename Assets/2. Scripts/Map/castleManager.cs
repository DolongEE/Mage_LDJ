using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class castleManager : MonoBehaviour
{
    private PlayerCtrl player;
    // Start is called before the first frame update

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl>();
    }
    void Start()
    {
        player.TorchOff(); //토치 비활성화
        player.SickleOff(); //낫 비활성화
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
