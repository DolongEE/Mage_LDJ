using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

//현재 스크립트에서 넓게는 현재 게임오브젝트에서 반드시 필요로하는 컴포넌트를 Attribute로 명시하여 해당 컴포넌트의 자동 생성 및 삭제되는 것을 막는다.
//이 스크립트 추가하면 오디오소스 같이 따라붙음
[RequireComponent(typeof(AudioSource))]
public class SoundUiManager : MonoBehaviour
{
    //배경음악제어
    public AudioClip[] soundFile; //오디오 클립 저장 배열 선언

    public float soundVolume = 1.0f;    //사운드 Volume 설정 변수
    public bool isSoundMute = false;    //사운드 Mute 설정 변수

    public Slider sl;   //슬라이더 컴포넌트 연결 변수
    public Toggle tg;   //토글 컴포넌트 연결 변수

    public GameObject Sound;    //Sound 오브젝트 연결변수
    public GameObject PlaySoundBth; //Sound ui버튼 오브젝트 연결 변수

    public AudioSource audioSource;   //오디오 소스의 오디오 사용

    public GameObject soundCanvas;

    //효과음제어
    public AudioClip[] effectsoundFile; //효과음에 쓸 오디오 클립저장배열
    public AudioSource asEffect;

    public Slider Efsl;   //슬라이더 컴포넌트 연결 변수
    public Toggle Eftg;   //토글 컴포넌트 연결 변수


    public float effectsoundVolume = 1.0f;    //사운드 Volume 설정 변수
    public bool effectisSoundMute = false;    //사운드 Mute 설정 변수


    [HideInInspector]
    public bool isEscapeKeyDown;

    public GameObject playerUi;
    void Awake()
    {
        isEscapeKeyDown = false;
        audioSource = GetComponent<AudioSource>();
        // 게임 사운드 로드 (사운드셋팅 저장된값 불러옴)
        LoadSoundData();
     //   LoadEffectSoundData();
    }

    void Start()
    {
        // 사운드 볼륨 초기화
        soundVolume = sl.value;
        // 사운드 음소거 초기화
        isSoundMute = tg.isOn;

        //효과음
        //효과음 초기화
        effectsoundVolume = Efsl.value;
        effectisSoundMute = Eftg.isOn;

        //asEffect.loop = false;  //반복 끔
        asEffect.playOnAwake = true;    //플레이시 재생

        AudioSet();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isEscapeKeyDown)
        {
            SoundUiOpen();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isEscapeKeyDown)
        {
            SoundUiClose();
        }
    }

    //배경음악 초기화
    public void SetSound() //호출시 사운드초기화
    {
        soundVolume = sl.value; //슬라이더에 밸류값
        isSoundMute = tg.isOn; //토글에 ison값

        effectsoundVolume = Efsl.value; //슬라이더에 밸류값
        effectisSoundMute = Eftg.isOn; //토글에 ison값
        AudioSet();
    }

    // ui에서 소리조절
    void AudioSet()
    {
        audioSource.volume = soundVolume; //audioSource의 볼륨 셋팅
        audioSource.mute = isSoundMute;   //audioSource의 Mute셋팅

        //효과음
        asEffect.volume = effectsoundVolume;
        asEffect.mute = effectisSoundMute;
    }

    //사운드 UI창 오픈
    public void SoundUiOpen()
    {
        soundCanvas.GetComponent<Canvas>().sortingOrder = 99;
        Sound.SetActive(true);     //사운드 ui 활성화
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        isEscapeKeyDown = true;
    }

    //사운드 UI창 닫음
    public void SoundUiClose()
    {
        soundCanvas.GetComponent<Canvas>().sortingOrder = 1;
        Sound.SetActive(false);    //사운드 ui 비활성화

        SaveSoundData(); //사운드설정값 세이브
        isEscapeKeyDown = false;
    }

    //스테이지 시작시 호출되는 함수
    public void PlayBackground(int stage) //스테이지별 인덱스로 다른 사운드 재생
    {
        //audioSource의 사운드 연결
        //GetComponent<AudioSource>().clip = soundFile[stage-1];
        audioSource.clip = soundFile[stage - 1];

        AudioSet();   //audioSource 셋팅

        //사운드 플레이 Mute설정시 사운드 온/오프
        //GetComponent<AudioSource>().Play();
        audioSource.Play();
    }

    //효과음 클립 호출되는 함수
    public void PlayEffectSound(int num)
    {
        asEffect.clip = soundFile[num - 1];
        AudioSet();
    }

    //사운드 멀어지고 죽는 객체쪽에서 소리가 나야함
    public void PlayEffct(Vector3 pos, AudioClip sfx)
    {
        //Mute 옵션 설정시 이 함수를 바로 빠져나가자. 음소거
        if (isSoundMute)
        {
            return;
        }

        GameObject _soundObj = new GameObject("sfx"); //게임오브젝트의 동적생성
        _soundObj.transform.position = pos; //사운드 발생 위치 지정

        //생성한 게임오브젝트에 audioSource 컴포넌트 추가
        AudioSource _audioSource = _soundObj.AddComponent<AudioSource>();

        //AudioSource 속성 설정
        _audioSource.clip = sfx; //사운드 파일 연결
        _audioSource.volume = soundVolume; //설정 되어있는 볼륨 적용 즉 soundVolume 으로 게임전체 사운드 볼륨 조절.
        _audioSource.minDistance = 15.0f;   //사운드 3d 셋팅에 최소범위 설정
        _audioSource.maxDistance = 30.0f;   //사운드 3d셋팅에 최대 범위 설정

        _audioSource.Play(); //사운드 실행

        Destroy(_soundObj, sfx.length + 0.2f); //모든사운드가 플레이 종료되면 동적생성 게임오브젝트 삭제
    }

    public void SaveSoundData()  //배경 사운드 데이타 저장
    {
        PlayerPrefs.SetFloat("SOUNDVOLUME", soundVolume);
        //bool형 데이타는 형변환을 해야  PlayerPrefs.SetInt() 함수를 사용가능
        PlayerPrefs.SetInt("ISSOUNDMUTE", System.Convert.ToInt32(isSoundMute));

        PlayerPrefs.SetFloat("EFFECTSOUNDVOLUME", effectsoundVolume);
        PlayerPrefs.SetInt("EFFECTISSOUNDMUTE", System.Convert.ToInt32(effectisSoundMute));
    } 

    //바로 사운드 UI슬라이드와 토글에 적용
    public void LoadSoundData()    //배경 사운드 데이타 불러오기  
    {
        sl.value = PlayerPrefs.GetFloat("SOUNDVOLUME");
        Efsl.value = PlayerPrefs.GetFloat("EFFECTSOUNDVOLUME");

        //int형 데이터는 bool형으로 형변환
        tg.isOn = System.Convert.ToBoolean(PlayerPrefs.GetInt("ISSOUNDMUTE"));
        Eftg.isOn = System.Convert.ToBoolean(PlayerPrefs.GetInt("EFFECTISSOUNDMUTE"));

        //첫 세이브시 설정 -> 이 로직 없으면 첫 시작시 사운드 볼륨 0
        int isSave = PlayerPrefs.GetInt("ISSAVE");
        int isEfSave = PlayerPrefs.GetInt("EFFECTISSAVE");
        if (isSave == 0 || isEfSave == 0)
        {
            sl.value = 1.0f;    //사운드 맥스
            tg.isOn = false;    //뮤트 음소거해제
            Efsl.value = 1.0f;    //사운드 맥스
            Eftg.isOn = false;    //뮤트 음소거해제
            //첫 세이브는 soundVolume = 1.0; isSoundMute = false; 이 디폴트 값으로 저장 된다.
            SaveSoundData();
            PlayerPrefs.SetInt("ISSAVE", 1);
            PlayerPrefs.SetInt("EFFECTISSAVE", 1);
        } 
    }

    // 메인화면으로 이동(방 퇴장)
    public void GoMaiMenu()
    {
        GameObject.Find("SoundUiManager").GetComponent<AudioSource>().Stop();     //1
        //GameObject.Find("SoundCanvas").GetComponent<Canvas>().enabled = false;  //2
        playerUi.SetActive(false);
        if (SceneManager.GetActiveScene().name != "scStmenu")
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene("scStmenu");

            if (SceneManager.GetActiveScene().name != "scLobby")
            {
                Destroy(GameObject.FindGameObjectWithTag("CMCAMERA"));
                Destroy(Camera.main.gameObject);
                Destroy(GameObject.Find("Magazine"));
            }
        }
        Sound.SetActive(false);
    }

    // 나가기 버튼 클릭
    public void ExitBtnClick()
    {
        Application.Quit(); // 애플리케이션 종료
    }
}

