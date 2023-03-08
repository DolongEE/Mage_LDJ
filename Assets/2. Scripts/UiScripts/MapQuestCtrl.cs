using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapQuestCtrl : MonoBehaviour
{
    //퀘스트버튼 연결 MapTip스크립트가져오기
    private MapTip map;
    public Color btnColor;
    public int mapNum;  //색변경할 버튼인덱스 받기
    public Button btntip;

    private void Awake()
    {
        map = GameObject.Find("PlayerUiManager").GetComponent<MapTip>();    //맵팁 스크립트 연결
        btntip = map.questBtn[mapNum];  //버튼연결
    }
    // Start is called before the first frame update
    void Start()
    {
        ColorChange();  //시작시 도움말 버튼색 변경
    }

    //버튼 기본색 바꿔주기
   void ColorChange()  //6CFFAE
    {
        //버튼안에 칼라 연결(ColorBlock로 버튼의 기본색, 눌렀을때 색 등등 조절가능)
        ColorBlock cb = btntip.colors;
    //    cb.normalColor = new Color32(108, 255, 174,255);    //r,g,b,a  이건 지정된 색
        cb.normalColor = btnColor;  //이건 인스펙터창에서 색변경가능
        btntip.colors = cb;
    }
}
