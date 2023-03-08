using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapTip : MonoBehaviour
{
    //도움말 창
    // public GameObject uihelp;       
    public GameObject froesthelp;
    public GameObject icehelp;
    public GameObject firehelp;
    public GameObject earthhelp;
    public GameObject castlehelp;
    public GameObject cavehelp;


    //퀘스트버튼 연결
    public Button[] questBtn;

    //숲맵 도움말 열고 닫기
    public void forestMapOpen()
    {
        froesthelp.SetActive(true);        //도움말창 활성화
        icehelp.SetActive(false);    //도움말창 비활성화
        firehelp.SetActive(false);        //도움말창 비활성화
        earthhelp.SetActive(false);    //도움말창 비활성화
        castlehelp.SetActive(false);
    }

    public void forestMapClose()
    {
        froesthelp.SetActive(false);    //도움말창 비활성화
    }

    //아이스맵 도움말 열고 닫기
    public void iceMapOpen()
    {
        icehelp.SetActive(true);        //도움말창 활성화
        froesthelp.SetActive(false);    //도움말창 비활성화
        firehelp.SetActive(false);        //도움말창 비활성화
        earthhelp.SetActive(false);    //도움말창 비활성화
        castlehelp.SetActive(false);
    }

    public void iceMapClose()
    {
        icehelp.SetActive(false);    //도움말창 비활성화
    }

    //불맵 도움말 열고 닫기
    public void fireMapOpen()
    {
        firehelp.SetActive(true);        //도움말창 활성화
        froesthelp.SetActive(false);    //도움말창 비활성화
        icehelp.SetActive(false);        //도움말창 비활성화
        earthhelp.SetActive(false);    //도움말창 비활성화
        castlehelp.SetActive(false);

    }

    public void fireMapClose()
    {
        firehelp.SetActive(false);    //도움말창 비활성화
    }

    //대지맵 도움말 열고 닫기
    public void earthMapOpen()
    {
        earthhelp.SetActive(true);        //도움말창 활성화
        froesthelp.SetActive(false);    //도움말창 비활성화
        icehelp.SetActive(false);        //도움말창 비활성화
        firehelp.SetActive(false);    //도움말창 비활성화
        castlehelp.SetActive(false);
    }

    public void earthMapClose()
    {
        earthhelp.SetActive(false);    //도움말창 비활성화
    }

    //메인성 도움말 열고 닫기
    public void castleMapOpen()
    {
        castlehelp.SetActive(true);
        earthhelp.SetActive(false);        //도움말창 활성화
        froesthelp.SetActive(false);    //도움말창 비활성화
        icehelp.SetActive(false);        //도움말창 비활성화
        firehelp.SetActive(false);    //도움말창 비활성화
    }

    public void castleMapClose()
    {
        castlehelp.SetActive(false);    //도움말창 비활성화
    }

    //동굴설명창
    public void cavehelpOpen()
    {
        cavehelp.SetActive(true);    //동굴 비활성화
    }
    public void cavehelpClose()
    {
        cavehelp.SetActive(false);    //동굴 비활성화
    }

}
