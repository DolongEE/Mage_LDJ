using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GoLobby : MonoBehaviour
{
    public GameObject[] exPnl;  //로딩시(설명)panel 연결변수
    public Text progressText;   //로딩시 text컴포넌트 연결변수
    public GameObject[] canvasObj;
    public bool login;

    private void Update()
    {
        Lobby();
    }

    public void Lobby()
    {
        if (login)
        {
            GameObject.Find("SoundUiManager").GetComponent<AudioSource>().Stop(); //1 
            GameObject.Find("SoundUiCanvas").GetComponent<Canvas>().enabled = false; //2
            for (int i = 0; i < canvasObj.Length; i++)
            {
                canvasObj[i].SetActive(false);
            }

            exPnl[Random.Range(0, 2)].SetActive(true); //1번 로딩패널 랜덤으로나오게

            StartCoroutine(this.Loading());

        }
    }

    IEnumerator Loading()
    {
        login = false;
        yield return new WaitForSeconds(1.0f);  //1초대기(1초 화면에 설명 띄우기)

        // 비동기 방식 씬 불러오기
        AsyncOperation async = SceneManager.LoadSceneAsync("scLobby", LoadSceneMode.Additive);

        //로딩중
        while (!async.isDone)    //싱크가 완료되지 않았으면
        {
            float progress = async.progress * 100.0f;   //0~1이라 100곱해서 %값 얻음
            int pRounded = Mathf.RoundToInt(progress); //float받아 올림해서 인트형으로 반환

            progressText.text = "Loading..." + pRounded.ToString() + "%"; //숫자 올라가며 %로 출력
            yield return true;
        }

        progressText.text = "Loading..." + Mathf.RoundToInt(async.progress * 100.0f).ToString() + "%";
        //로딩완료후 설명서 비활성화, 사운드ui 활성화
        exPnl[0].transform.parent.gameObject.SetActive(false);
        GameObject.Find("SoundUiCanvas").GetComponent<Canvas>().enabled = true;

        yield return null;
    }
}
