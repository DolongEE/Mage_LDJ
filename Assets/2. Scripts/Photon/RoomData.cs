using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//포톤 추가
public class RoomData : MonoBehaviour
{
    [HideInInspector]
    public string roomName = ""; //방이름
    [HideInInspector]
    public int connectPlayer = 0; //현재 접속 유저수
    [HideInInspector]
    public int maxPlayers = 0; //룸의 최대 접속자수

    public Text textRoomName; //룸이름 표시할 텍스트UI항목 연결
    public Text textConnectInfo; //룸최대접속자 수와 현재 접속자수를 표시할 텍스트UI항목연결

    //룸 정보를 전달한 후 Text UI 항목에 룸 정보를 표시하는 함수
    public void DisplayRoomData()
    {
        textRoomName.text = roomName;
        textConnectInfo.text = "{" + connectPlayer.ToString() + "/" + maxPlayers.ToString() + ")";
    }
}
