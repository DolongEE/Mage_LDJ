using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerUiManager : MonoBehaviour
{
    public GameObject playerUi;
    //채팅추가
    public InputField chatMsg; //받을 메시지(InputField넣음)
    public Text txtChatMsg;     //채팅 메시지 표시(Text창)
    //포톤추가
    PhotonView pv; //RPC호출을 위한 포톤뷰 연결

    // 닉네임을 받을 Text 변수
    public Text playerNickName;

    //인벤토리관련
    public GameObject uiInven;      //인벤 패널변수
    public GameObject uihelp;       //전체 도움말 창
    public GameObject[] playBtn;    //playUi 화면에 있는 버튼 참조할 배열

    void Awake()
    {
        //네트워크 포톤추가
        pv = GetComponent<PhotonView>();  //PhotonView 컴포넌트를 레퍼런스에 할당 
    }

    void Start()
    {
        // 플레이어 Ui 게임 시작 전까지 비활성화
        playerUi.SetActive(false);
    }
    //인벤창
    public void InvenOpen() //인벤오픈
    {
        playBtn[0].SetActive(false);    //현재 버튼1개(인벤오픈버튼) 비활성
//      playBtn[1].SetActive(false);    //두번째버튼
        uiInven.SetActive(true);        //인벤창 활성화
    }

    public void InvenClose()    //인벤클로즈
    {
        uiInven.SetActive(false);    //인벤창 비활성화
        playBtn[0].SetActive(true);     //현재 버튼1개(인벤오픈버튼) 활성
        //playBtn[1].SetActive(true);     //두번째 버튼
    }
    public void HelpOpen() // 전체 도움말 오픈
    {
       // playBtn[0].SetActive(false); //현재 버튼1개 비활성
        playBtn[1].SetActive(false);    //두번째버튼
        uihelp.SetActive(true);        // 전체 도움말 활성화
    }

    public void HelpClose()    // 전체 도움말 클로즈
    {
        uihelp.SetActive(false);    // 전체 도움말 비활성화
       // playBtn[0].SetActive(true);     //현재 버튼1개 활성
        playBtn[1].SetActive(true);     //두번째 버튼
    }

    //채팅메세지
    [PunRPC] //포톤추가
    void ChatMsg(string ctmsg)
    {
        txtChatMsg.text = txtChatMsg.text + ctmsg; //채팅 메시지 text UI에 텍스트를 누적시켜표시
    }

    public void OnclickChatMsgSend()
    {
        //네트워크시(포톤추가2줄)
        string ctmsg = "\n\t<color=#0000ff> [" + PhotonNetwork.player.NickName + "] </color>" + chatMsg.text;
        pv.RPC("ChatMsg", PhotonTargets.All, ctmsg);

        chatMsg.text = ""; //입력 누르고 나면 빈칸으로
    }
}
