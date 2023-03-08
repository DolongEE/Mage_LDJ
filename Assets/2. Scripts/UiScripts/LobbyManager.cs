using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  //씬 로드할때 필요한 씬매니져

public class LobbyManager : MonoBehaviour
{
    // photoninit 추가 해야됨

    public GameObject roomFail;     //룸 이름 중복창
    public GameObject secretInput;  //비밀번호입력창

    void Awake()
    {      
     //   playerUi.SetActive(true);   //2번 유저인터페이스 활성화
    }   

    public void WindowBtnClose()    //중복이름 클로즈
    {
        roomFail.SetActive(false);  //중복이름 창 비활성화
    }

    public void SecretWindowBtnClose()  //비밀번호 입력창 클로즈
    {
        secretInput.SetActive(false);
    }
}
