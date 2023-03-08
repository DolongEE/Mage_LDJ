using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearQueen : MonoBehaviour
{
    public GameObject clearEffect; //게임오브젝트 클리어 이펙트 받아오는 변수 선언
    [SerializeField]
    Animator[] anim;


    public bool clear;
    private void Awake()
    {
        anim = gameObject.GetComponentsInChildren<Animator>(); 
        clear = false;
        clearEffect = GameObject.Find("ClearEffect").GetComponent<Transform>().GetChild(0).gameObject;
    }

    private void Update()
    {
        if (clear)
        {            
            anim[0].SetBool("isClear", true);  //모델 애니메이션 Attack3 애니메이션 실행
            anim[2].SetBool("clearCollider", true);    //모델
            clearEffect.SetActive(true);
        }
    }
    public void GameClear()
    {
        anim[0].SetBool("isClear", true);  //모델 애니메이션 Attack3 애니메이션 실행
        anim[2].SetBool("clearCollider", true);    //모델
        clearEffect.SetActive(true);
    }
}
