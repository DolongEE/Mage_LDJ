using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;
public class MainManager : MonoBehaviour
{
    // 포톤뷰 스크립트
    PhotonView pv = null;

    // 게임 레디, 스타트 변수
    private int readyCount;
    public GameObject gameStartBtn;
    public GameObject readyBtn;
    public GameObject readyCancelBtn;

    // 플레이어 스폰 위치 설정
    private Transform[] playerSpawn;

    // 캔버스 제어
    public Canvas RoomCanvas;
    public GameObject playerUiCanvas;

    // 캐릭터 생성 변수
    [HideInInspector]
    public int selectPlayer = 0;
    public GameObject player;

    // 시네머신 카메라 제어
    public GameObject cmMain;
    public GameObject cmPlayer;

    public Text roomNametxt;
    public bool isGameStart;
    void Awake()
    {
        isGameStart = false;
        // 게임 레디 인원수
        readyCount = 0;
        pv = GetComponent<PhotonView>();
        // 플레이어 스폰위치 
        playerSpawn = GameObject.FindGameObjectWithTag("PLAYER_SPAWN").GetComponentsInChildren<Transform>();
        //포톤 클라우드로부터 네트워크 메시지 수신을 다시 연결
        PhotonNetwork.isMessageQueueRunning = true;
        // MasterClient에 따라 씬이동
        PhotonNetwork.automaticallySyncScene = true;
        // 방 이름 받아옴
        roomNametxt.text = PhotonNetwork.room.Name;

        if (PhotonNetwork.isMasterClient)
        {
            readyCancelBtn.SetActive(false);
            readyBtn.SetActive(true);
            gameStartBtn.SetActive(true);
            gameStartBtn.GetComponent<Image>().canvasRenderer.SetColor(Color.gray);
        }
        else
        {
            gameStartBtn.SetActive(false);
            readyBtn.SetActive(true);
            readyCancelBtn.SetActive(false);
        }
        StartCoroutine(CreatePlayer());
    }
    void Start()
    {
        playerUiCanvas = GameObject.Find("PlayerUiCanvas");
    }

    void Update()
    {
        if (PhotonNetwork.isMasterClient && readyCount == PhotonNetwork.room.PlayerCount)
        {
            gameStartBtn.GetComponent<Button>().enabled = true;
            gameStartBtn.GetComponent<Image>().canvasRenderer.SetColor(Color.white);
        }
        else
        {
            gameStartBtn.GetComponent<Button>().enabled = false;
            gameStartBtn.GetComponent<Image>().canvasRenderer.SetColor(Color.gray);
        }
    }
    // 플레이어 생성
    IEnumerator CreatePlayer()
    {
        yield return new WaitForSeconds(0.3f);
        // 룸 정보를 받아옴
        Room currRoom = PhotonNetwork.room;

        player = PhotonNetwork.Instantiate("Player", playerSpawn[currRoom.PlayerCount].position, playerSpawn[currRoom.PlayerCount].rotation, 0);

        yield return null;
    }


    // 캐릭터 선택 우버튼
    public void OnClickNextBtn()
    {
        if (selectPlayer >= 3)
        {
            selectPlayer = 0;
        }
        else
        {
            selectPlayer++;
        }
    }

    // 캐릭터 선택 좌버튼
    public void OnClickPrevBtn()
    {
        if (selectPlayer <= 0)
        {
            selectPlayer = 3;
        }
        else
        {
            selectPlayer--;
        }
    }

    #region 게임 준비 및 시작 함수

    // 게임 레디 버튼
    public void OnClickGameReady()
    {
        // 게임 레디를 방장에게 정보를 보냄
        pv.RPC("GameReady", PhotonTargets.MasterClient, null);
        readyBtn.SetActive(false);
        readyCancelBtn.SetActive(true);        
    }

    [PunRPC]
    void GameReady()
    {
        readyCount++;
    }

    // 게임 레디 취소 버튼
    public void OnClickGameReadyCancel()
    {
        // 게임 레디 취소를 방장에게 정보를 보냄
        pv.RPC("GameReadyCancel", PhotonTargets.MasterClient, null);
        readyCancelBtn.SetActive(false);
        readyBtn.SetActive(true);
    }

    [PunRPC]
    void GameReadyCancel()
    {
        readyCount--;
    }

    // 게임 스타트 버튼 클릭(내부에 RPC사용)
    public void OnClickGameStart()
    {
        // 방장이면서 레디 수가 접속자수와 같을때 시작버튼 클릭 가능
        if (PhotonNetwork.isMasterClient && readyCount == PhotonNetwork.room.PlayerCount)
        {
            // 방장이 게임스타트 눌렀을때 모두 게임스타트
            pv.RPC("GameStart", PhotonTargets.All, null);
        }
    }

    // 캔버스 끄고 게임스타트
    [PunRPC]
    void GameStart()
    {
        isGameStart = true;
        // 방 Ui캔버스 비활성화
        RoomCanvas.gameObject.SetActive(false);
        // 플레이어 Ui캔버스 활성화
        playerUiCanvas.transform.GetChild(0).gameObject.SetActive(true);
        player.GetComponent<PlayerCtrl>().UpdateHp();
        // 게임이 시작됬을때 외부에서 방 접근 방지
        PhotonNetwork.room.IsVisible = false;
        player.GetComponent<PlayerCtrl>().myAnim = player.GetComponentInChildren<Animator>();
        // 시네머신 시작
        StartCoroutine(CMStart());
    }

    IEnumerator CMStart()
    {
        // scMain씬에 있는 시네머신 카메라 시작
        cmMain.GetComponent<CMMain>().GameStart();
        yield return new WaitForSeconds(2.5f);
        // 플레이어 cm카메라 활성화
        cmPlayer.GetComponent<CMPlayer>().enabled = true;
        // 방장이 방 이동시 다른 플레이어도 따라감
        PhotonNetwork.LoadLevel("scCastle");

        yield return null;
    }

    #endregion


    // 룸 퇴장 버튼
    public void OnClickExitRoom()
    {
        // 현재 룸을 퇴장하며 생성한 모든 네트워크 객체 삭제
        PhotonNetwork.LeaveRoom();
    }

    // 룸에서 접속종료하면 호출하는 콜백 함수
    void OnLeftRoom()
    {
        // 로비씬 불러옴
        SceneManager.LoadScene("scLobby");
    }

}
