using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class csPhotonInit : MonoBehaviour
{
    //App의 버전 정보
    public string version = "Ver 0.1.0";

    //예상하는 대로 동작하는 것에 대하여 확신이 서면 로그 레벨을 Informational 으로 변경 추천
    public PhotonLogLevel LogLevel = PhotonLogLevel.Full;

    //플레이어의 이름을 입력하는 UI 항목 연결을 위한 레퍼런스
    public InputField userId;

    //룸 이름을 입력받을 UI 항목 연결 레퍼런스
    public InputField roomName;

    //RoomItem이 차일드로 생성될 Parent 객체의 레퍼런스
    public GameObject scrollContents;

    //룸 목록만큼 생성될 RoomItem 프리팹 연결 레퍼런스
    public GameObject roomItem;

    //룸 생성 실패시(이름 중복) SetActive(true)
    public GameObject roomFailObj;

    void Awake()
    {
        if (!PhotonNetwork.connected)
        {
            // 같은 버전만 접속하게 설정
            PhotonNetwork.ConnectUsingSettings(version);
            // 로그 예외사항 설정
            PhotonNetwork.logLevel = PhotonLogLevel.Informational;
            // 플레이어 이름 초기화
            PhotonNetwork.playerName = "Guest_" + Random.Range(0, 999).ToString("000");
        }
        roomFailObj.SetActive(false);
        // 룸이름 초기화
        roomName.text = "Room_" + Random.Range(0, 999).ToString("000");

        // 대기실 pivot 좌표를 최상단 좌측으로 설정
        scrollContents.GetComponent<RectTransform>().pivot = new Vector2(0.0f, 1.0f);
    }

    // 포톤 서버에 정상적으로 로비에 접속시 발생하는 콜백 함수
    void OnJoinedLobby()
    {
        userId.text = GetUserId();
    }

    // 로컬에 저장된 플레이어 이름 반환
    string GetUserId()
    {
        // 유저 아이디 가져옴
        string userId = PlayerPrefs.GetString("USER_ID");

        if (string.IsNullOrEmpty(userId)) 
        {
            // 유저 아이디 초기화
            userId = "USER_" + Random.Range(0, 999).ToString("000");
        }
        // 유저 아이디 반환
        return userId;
    }

    // Make Room 클릭시 방 생성 함수
    public void OnClickCreateRoom()
    {
        string _roomName = roomName.text;

        // 룸 이름이 없거나 null일경우 룸 랜덤 설정
        if (string.IsNullOrEmpty(_roomName))
        {
            _roomName = "ROOM_" + Random.Range(0, 999).ToString("000");
        }

        // 로컬 플레이어 이름 설정
        PhotonNetwork.player.NickName = userId.text;
        // 플레이어 이름을 로컬에 저장
        PlayerPrefs.SetString("USER_ID", userId.text);

        // 공개방 룸 조건 설정
        RoomOptions roomOptions = new RoomOptions();
        // 방 오픈
        roomOptions.IsOpen = true;
        // 방 공개 여부 설정
        roomOptions.IsVisible = true;
        // 최대 입장 인원 설정
        roomOptions.MaxPlayers = 3;
        roomOptions.PublishUserId = true;

        // 비밀방 룸 조건 설정 (추후 추가 필요)
        //roomOptions.IsOpen = false;
        //roomOptions.IsVisible = true;
        //roomOptions.MaxPlayers = 4;
        // 비밀번호 추가

        PhotonNetwork.CreateRoom(_roomName, roomOptions, TypedLobby.Default);
    }

    // 생성된 룸 목록 변경시 콜백함수 호출
    void OnReceivedRoomListUpdate()
    {
        // 룸 목록이 변경되면 이 함수를 호출
        // ROOM_LIST 태그 있는지 반드시 확인!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        foreach(GameObject roomList in GameObject.FindGameObjectsWithTag("ROOM_ITEM")) 
        {
            Destroy(roomList);
        }
        // Grid layout group 컴포넌트의 constraint count 값 증가 변수
        int rowCount = 0;
        // 스크롤 영역 초기화
        scrollContents.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

        // ROOM_LIST 객체 생성
        foreach(RoomInfo _room in PhotonNetwork.GetRoomList())
        {
            // roomItem 동적 생성
            GameObject room = Instantiate<GameObject>(roomItem);
            room.transform.SetParent(scrollContents.transform, false);

            // 생성된 Roomitem에 정보 표시하기위해 정보 전달
            RoomData roomData = room.GetComponent<RoomData>();
            roomData.roomName = _room.Name;
            roomData.connectPlayer = _room.PlayerCount;
            roomData.maxPlayers = _room.MaxPlayers;

            roomData.DisplayRoomData();

            roomData.GetComponent<Button>().onClick.AddListener(delegate { OnClickRoomItem(roomData.roomName); });

            //Grid Layout Group 컴포넌트의 Constraint Count 증가
            scrollContents.GetComponent<GridLayoutGroup>().constraintCount = ++rowCount;
            //스크롤 영역의 높이 증가
            scrollContents.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 20);
        }
    }

    //RoomItem이 클릭되면 호출될 이벤트 연결 함수
    void OnClickRoomItem(string roomName)
    {
        //Debug.Log(roomItem.GetComponent<RoomData>().maxPlayers);
        //// 방에 사람이 꽉차 입장 불가시
        //if (roomItem.GetComponent<RoomData>().connectPlayer >= roomItem.GetComponent<RoomData>().maxPlayers)
        //{

        //    roomFailObj.SetActive(true);
        //}
        //// 방에 입장 가능할때
        //else
        //{
            //로컬 플레이어의 이름을 설정
            PhotonNetwork.player.NickName = userId.text;

            //플레이어 이름을 로컬에 저장
            PlayerPrefs.SetString("USER_ID", userId.text);

            //인자로 전달된 이름에 해당하는 룸으로 입장
            PhotonNetwork.JoinRoom(roomName);
        //}
    }

    // 방만들기 실패했을때 콜백되는 함수(주로 중복이름 사용시 발생함)
    void OnPhotonCreateRoomFailed(object[] codeAndMsg)
    {
        // 추후 추가
        if (PhotonNetwork.GetRoomList().ToString() == roomName.text)
        {
            roomFailObj.SetActive(true);
            return;
        }
    }

    // 방에 입장했을때 콜백되는 함수
    void OnJoinedRoom()
    {
        // 방 입장시 씬 로드
        StartCoroutine(this.LoadStage());
    }

    IEnumerator LoadStage()
    {
        // 씬을 전환하는 동안 클라우드 서버 네트워크 메세지 수신 중단
        // 씬 전환후 다시 true로 변경 Stage
        PhotonNetwork.isMessageQueueRunning = false;

        // 씬 넘어가기 전까지 백그라운드 유지
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("scMain");

        // 씬 로딩 완료시까지 대기
        yield return asyncOperation;

        yield return null;
    }

    void OnGUI()
    {
        // 포톤서버에 잘 입장했는지 확인해주는 메세지
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }    
}
