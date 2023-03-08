using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    //열거형 타입 사용
    public enum Type { Melee, Range};   // Melee : 근접 공격, Range : 원거리 공격 
    public Type type;                   // 공격 타입 설정할 변수 선언
    public int damage;                  // 공격에 따른 데미지 설정 변수 선언
    public float rate;                  // 공격속도 변수 선언
    public BoxCollider meleeArea;       // 근접공격 범위 (사거리) 선언
    public TrailRenderer trailEffect;   // 공격 효과 

    public void WeaponUse()
    {
        if(type==Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");

        }
    }

    IEnumerator Swing() //열거형 함수 클래스
    {
        //yield break;    //yield break 로 코루틴 탈출 가능

        //1 Frame
        yield return new WaitForSeconds(0.1f);   // 0.1초 프레임 대기
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        //2 Frame
        yield return new WaitForSeconds(0.3f);   //0.3초 프레임 대기
        meleeArea.enabled = false;

        //3 Frame
        yield return new WaitForSeconds(0.5f);   //0,5초 프레임 대기
        trailEffect.enabled = false;
    }

    //Use() 함수를 메인 루틴이라고 한다. 
    //메인 루틴 Use() 함수에서 -> Swing()함수를 호출한다. //Swing()함수는 서브루틴이라고 한다.
    //Swing() 서브루틴이 끝나면 다시 메인 루틴인 Use()를 실행한다.
    //Use() -> Swing() -> Use();

    //하지만  코루틴을 사용하면
    //Use() 메인루틴 + Swing()  코루틴(Co-Op) 이렇게 사용된다.




    void Update()
    {
        
    }
}
