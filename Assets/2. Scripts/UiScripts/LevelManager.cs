 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int stage;   //스테이지
    private SoundUiManager _sMgr; //SoundManager 컴포넌트 연결

    public int effnum=0;
    public AudioClip[] effectsound;   //효과음 받음    

    void Awake()
    {
        _sMgr = GameObject.Find("SoundUiManager").GetComponent<SoundUiManager>();
    }


    // Start is called before the first frame update
    void Start()
    {      
            _sMgr.PlayBackground(stage);      // 몇번째 클립 배경음악 재생
    }
    
   void playeffect()
    {
        _sMgr.PlayEffectSound(effnum);  //몇번째 효과음 재생
        //
    //   _sMgr.GetComponent<AudioSource>()
    //이걸 함수로 해놓고 필요한데서 부름
    //   _sMgr.asEffect.PlayOneShot(재생할 음악effectsound); /위에서 받은 오디오
    }

    //스타트메뉴 효과음소리
    public void PlaybtnEffectSnd() 
    {
        _sMgr.asEffect.PlayOneShot(effectsound[0]);
    }
        public void PlaybtnExp()   
    {
        _sMgr.asEffect.PlayOneShot(effectsound[1]);
    }
    public void PlaybtnEffectSnd2()
    {
        _sMgr.asEffect.PlayOneShot(effectsound[2]);
    }

    //포탈내려올때 효과음
    public void PlayPotalDnSound()
    {
        _sMgr.asEffect.PlayOneShot(effectsound[0]); //포탈다운효과음 처음에 있어야함
    }

    //숲맵 물내려올때 효과음(시간17초)
    public void PlayWaterDnSound()
    {
        _sMgr.asEffect.PlayOneShot(effectsound[1]); //물다운효과음 처음에 있어야함
    }   
    //불맵 다리 부서질때
    public void BrokenBridgeSound()
    {
        _sMgr.asEffect.PlayOneShot(effectsound[1]); //다리 부서지는 소리
    }


}
