using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class ItemDB: MonoBehaviour
{
    public static ItemDB instance;
    void Awake()
    {
        instance = this;
    }

    // 데이터베이스 정보 리스트 생성
    public List<Item> itemDB = new List<Item>();

    //인터넷에서 아이템디비 제이슨 연결
    private string itemListURL = "http://ldj.dothome.co.kr/mageItemDB.json.php";

    void Start()
    {
        // 아이템 획득
        Getitem();
    }

    void Getitem()
    {
        StartCoroutine("GetItemList");
    }

    //웹에서 sql문 다운
    public IEnumerator GetItemList()
    {
        WWWForm form = new WWWForm();

        var www = new WWW(itemListURL, form);
        yield return www;
        DisplayItemList(www.text);
    }

    //sql문 items테이블 안에 칼럼들 연결해서 리스트 items에 넣기
    void DisplayItemList(string strJsonData)
    {
        var jSon = JSON.Parse(strJsonData);

        for (int i = 0; i < jSon.Count; i++)
        {
            string itemImage = jSon[i]["itemIcon"];
            int itemId = jSon[i]["itemID"].AsInt;
            string itemName = jSon[i]["itemName"];    
            string itemDes = jSon[i]["itemDes"];
            int itemType = jSon[i]["itemType"].AsInt;
            string itemPrefab = jSon[i]["itemPrefab"];

            //Item클래스(이미지이름, 아이템Id, 아이템이름, 아이템 설명, 아이템 타입)
            itemDB.Add(new Item(itemImage, itemId, itemName,  itemDes, itemType, itemPrefab));
        }
    }

    // 아이템 정보를 넘겨주는 함수
    public Item GetItemInfo(string itemName)
    {
        int i = 0;
        // DB정보 나열
        for (i = 0; i < itemDB.Count-1; i++)
        {
            // 오브젝트 이름과 DB에 저장된 이름 비교
            if (itemDB[i].itemName == itemName)
            {
                break;
            }
            // 동일한 이름이 없을경우
            else if (i == itemDB.Count - 1)
            {
                Debug.LogError("아이템 정보가 없음");
            }
        }
        // 저장된 아이템 정보 보냄
        return itemDB[i];
    }
}
