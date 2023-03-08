using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSpawn : MonoBehaviour
{
    PhotonView pv = null;
    InventoryUI invenUI;

    public Transform playerSpawn;

    private GameObject[] player;

    void Awake()
    {
        pv = GetComponent<PhotonView>();

        player = GameObject.FindGameObjectsWithTag("Player");
        invenUI = GameObject.Find("PlayerUiCanvas").GetComponent<InventoryUI>();
    }

    void Start()
    {
       SpawnPlayer();
    }

    void SpawnPlayer()
    {
        Room room = PhotonNetwork.room;
        if (invenUI.mapName == "scSnowCaveMap" && SceneManager.GetActiveScene().name == "scSnowMap")
        {
            for (int j = 3; j < room.PlayerCount + 3; j++)
            {
                // 플레이어 스폰위치 설정
                player[j - 3].transform.position = playerSpawn.GetChild(j).position;
            }
        }
        else
        {
            for (int i = 0; i < room.PlayerCount; i++)
            {
                // 플레이어 스폰위치 설정
                player[i].transform.position = playerSpawn.GetChild(i).position;
            }
        }
    }
}
