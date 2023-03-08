using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClearMap : MonoBehaviour
{
    public Text showText;
    private int playerCnt;
    private int inPlayer;
    public int countTime = 3;
    private bool isCount;
    private Animator anim;
    private SphereCollider boxCol;
    public GameObject child;  

    void Awake()
    {
        showText = GameObject.FindGameObjectWithTag("portalTxt").GetComponent<Text>();
        playerCnt = PhotonNetwork.room.PlayerCount;
        anim = child.GetComponent<Animator>();
        boxCol = GetComponent<SphereCollider>();    
    }

    void Start()
    {
        boxCol.enabled = false;
    }

    public void EndGame()
    {
        child.SetActive(true);
        anim.SetTrigger("Clear");
        boxCol.enabled = true;
        Debug.Log(playerCnt);
    }

    void GoMap()
    {
        StartCoroutine(CountDown(countTime));     
    }

    IEnumerator CountDown(int _countTime)
    {
        while (true)
        {
            showText.text = _countTime.ToString();
            yield return new WaitForSeconds(1.5f);
            _countTime--;
            if (_countTime == 0)
            {
                PhotonNetwork.LoadLevel("scCastle");
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
