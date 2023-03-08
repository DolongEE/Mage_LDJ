using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 아이템 타입
public enum ItemType
{
    // 특수
    Vision,
    // 장비
    Equipment,
    // 퀘스트
    Quest, 
    // 포션
    Potion
}

[System.Serializable]
public class Item
{
    // 이미지
    public Sprite itemImage;
    // 아이디
    public int itemId;
    // 이름
    public string itemName;
    // 설명
    [TextArea]
    public string itemDes;
    // 아이템 타입(장비, 포션)
    public ItemType itemType;
    public GameObject itemPrefab;   //아이템프리펩

    public Item()   //인자없는 생성자
    {

    }

    // 아이템      (이미지, 아이디, 아이템 이름, 아이템 설명, 아이템 타입)
    public Item(string img, int id, string name, string desc, int type, string prefab)
    {
        // DB에서 문자열로 받아오기때문에 문자열로 가져와 resources에 있는 파일과 비교 및 저장
        itemImage = Resources.Load<Sprite>("ItemIcons/34x34icons180709_" + img);
        // 아이템 아이디
        itemId = id;
        // 아이템 이름
        itemName = name;
        // 아이템 설명
        itemDes = desc;
        // 아이템 프리펩(스트링형을 게임오브젝트형 바꿈)
        itemPrefab = Resources.Load<GameObject>("ItemPrefabs/" + prefab);

        // DB에서 인트형으로 받아와 ENUM형으로 형변환
        if (type == 0)
        {
            itemType = ItemType.Vision;
        }
        else if (type == 1)
        {
            itemType = ItemType.Equipment;
        }
        else if (type == 2)
        {
            itemType = ItemType.Quest;
        }
        else if (type == 3)
        {
            itemType = ItemType.Potion;
        }
    }
}