using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapChange : MonoBehaviour
{
    public Text showText;
    private int playerCnt;
    private int inPlayer;
    public int countTime = 3;
    private bool isCount;
    PlayerCtrl playerCtrl;
    Inventory inven;

    void Awake()
    {
        showText = GameObject.FindGameObjectWithTag("portalTxt").GetComponent<Text>();
        playerCnt = PhotonNetwork.room.PlayerCount;
        playerCtrl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl>();
        inven = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }
    private void Update()
    {
        if (playerCtrl.getPyro && gameObject.name == "scFireMap")
        {
           gameObject.SetActive(false);
        }
        if (playerCtrl.getAnemo && gameObject.name == "scForestMap")
        {
            gameObject.SetActive(false);
        }
        if (playerCtrl.getHydro && gameObject.name == "scSnowMap")
        {
            gameObject.SetActive(false);
        }
        if (playerCtrl.getGeo && gameObject.name == "scEarthMap")
        {
            gameObject.SetActive(false);
        }
    }

    void GoMap()
    {
        string name_obj = this.gameObject.name;
        //삼항 연산자
        // "?" : name_obj가 scFireMap 이 맞으면 name_obj = scFireMap
        // ":" : name_obj가 scFireMap 이 아니면 : 뒤에있는 (name_obj == "scForestMap") ? "scForeestMap" : "scSnowMap:" 으로 넘어간다.
        //그럼 반복
        name_obj = (name_obj == "scFireMap") ? "scFireMap" : (name_obj == "scForestMap") ? "scForestMap" : (name_obj == "scSnowMap") ? "scSnowMap" : (name_obj == "scSnowCaveMap") ? "scSnowCaveMap" : "scEarthMap";

        StartCoroutine(CountDown(name_obj, countTime));
    }

    IEnumerator CountDown(string i, int _countTime)
    {
        while (true)
        {
            showText.text = _countTime.ToString();
            yield return new WaitForSeconds(1.5f);
            _countTime--;
            if (_countTime == 0)
            {
                PhotonNetwork.LoadLevel(i); //LoadLevel : 포톤네트워크에서의 Scene 변경 명령어 LoadScene 와 같다.
                showText.text = "";
                yield break;
            }
            if (!isCount)
            {
                showText.text = "";
                yield break;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            inPlayer++;
            if (playerCnt == inPlayer && !isCount)
            {
                isCount = true;
                GoMap();
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            inPlayer--;
            isCount = false;
        }
    }
}