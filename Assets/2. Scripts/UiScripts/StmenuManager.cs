using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StmenuManager : MonoBehaviour
{

    
    // 설명 패널 변수
    public GameObject explain;
    public GameObject explainOpenbtn;
    public GameObject startBtn;

    // 게임 설명 내부 설명버튼
    public GameObject[] gmaeExplainPnl;

    // 게임 시작시 로그인
    public GameObject LoginPnl;
    // 회원 가입 로그인
    public GameObject newAccountBtn;
    public GameObject accountPnl;

    void Awake()
    {
        GameObject.Find("SoundUiManager").GetComponent<AudioSource>().Play();
  
        //   GameObject.Find("SoundCanvas").GetComponent<Canvas>().enabled = true;
    }
    private void Start()
    {
        LoginPnl.SetActive(false);
    }

    //함수 실행순서 맞춰 주는게 좋음
    public void ExplainOpen() //설명오픈
    {
        explainOpenbtn.SetActive(false);   // 설명오픈버튼 비활성
        startBtn.SetActive(false);   //다른버튼비활성

        GameObject.Find("SoundUiCanvas").GetComponent<Canvas>().enabled = false;  //옵션버튼 비활성

        explain.SetActive(true);    //설명창 활성화
    }

    public void ExlainClose()    //설명클로즈
    {
        explain.SetActive(false);    //설명창 비활성화

        GameObject.Find("SoundUiCanvas").GetComponent<Canvas>().enabled = true;    //옵션버튼 활성
        explainOpenbtn.SetActive(true);   // 설명오픈버튼 활성
        startBtn.SetActive(true);   // 다른버튼 활성
    }

    public void StoryBtnClick()
    {
        gmaeExplainPnl[0].SetActive(true);
        gmaeExplainPnl[1].SetActive(false);
        gmaeExplainPnl[2].SetActive(false);
    }
    public void CharacterBtnClick()
    {
        gmaeExplainPnl[0].SetActive(false);
        gmaeExplainPnl[1].SetActive(true);
        gmaeExplainPnl[2].SetActive(false);
    }
    public void ControlBtnClick()
    {
        gmaeExplainPnl[0].SetActive(false);
        gmaeExplainPnl[1].SetActive(false);
        gmaeExplainPnl[2].SetActive(true);
    }

    public void LoginBtnClick()
    {
        LoginPnl.SetActive(true);
    }

    public void CreateAccountBtnClick()
    {
        accountPnl.SetActive(true);
    }
}
