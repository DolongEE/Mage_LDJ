using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

public class ItemCtrl : MonoBehaviour
{
    private ItemDB itemDB;
    private string objectName;

    //Item 정보 가져옴
    public Item item;
    //Item 이미지연결
    public Sprite image;

    public GameObject itemPrefab; //버릴때 필요한 아이템 프리펩연결    

    //포톤추가
    PhotonView pv = null;

    string itemName;

    private void Awake()
    {
        //포톤추가
        pv = GetComponent<PhotonView>(); //포톤뷰 컴포넌트 할당
        itemDB = GameObject.FindGameObjectWithTag("ItemDB").GetComponent<ItemDB>();
        objectName = gameObject.name;
    }

    IEnumerator Start()
    {
        Debug.Log("어웨이크 생성 : " + objectName);
        
        if(objectName.Contains("(Clone)"))
        {
            string substring = "(Clone)";
            int startindex = objectName.IndexOf(substring);
            objectName = objectName.Remove(startindex, substring.Length);
            Debug.Log(objectName);
        }
        gameObject.name = objectName;
        // 네트워크에서 정보 받아올 시간 확보
        yield return new WaitForSeconds(1f);
        // 현재 오브젝트 이름과 데이터 베이스 이름 비교
        // 이름이 일치시 아이템 정보 가져옴        
        SetItem(itemDB.GetItemInfo(objectName));

        yield return null;
    }
    //아이템정보 받아옴(Item클래스 안에 목록받아올거임)
    public void SetItem(Item _item)
    {
        // 아이템 이미지 저장
        item.itemImage = _item.itemImage;

        // 아이템 id 출력
        item.itemId = _item.itemId;
        // 아이템 이름 출력
        item.itemName = _item.itemName;
        // 아이템 설명 출력
        item.itemDes = _item.itemDes;
        // 아이템 타입 출력(enum형)
        item.itemType = _item.itemType;
        //아이템 프리펩 출력
        item.itemPrefab = _item.itemPrefab;

        // 인벤토리에 추가할 아이템 이미지
        image = item.itemImage;
        //위에 프리펩변수에 넣어줌
        itemPrefab = item.itemPrefab;
    }

    //아이템얻기
    public Item GetItem()
    {
        return item;
    }


    public void DestItem()
    {
        pv.RPC("DestroyItem", PhotonTargets.MasterClient, null);
    }

    [PunRPC]
    //아이템 사라짐
    public void DestroyItem()
    {
     //   Destroy(gameObject);
        //포톤추가 디스트로이함수 네트워크에선 포톤네트워크. 으로 바꿔야함       
        PhotonNetwork.Destroy(gameObject); //자신과 네트워크상의 모든 아바타를 삭제
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {

        }
        else
        {

        }
    }
}
