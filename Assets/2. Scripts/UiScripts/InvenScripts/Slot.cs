using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; //마우스 이벤트시 추가

public class Slot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler //마우스 클릭인터페이스, 드래그시작, 드래그중, 드래그 끝, 드롭 , 마우스 원안에들어오고, 나가고
{
    public Item slotItem;   //Item클래스 item으로가져오기
    public Image itemIcon;  //슬롯에 나올 이미지 연결(슬롯밑에 이미지오브젝트를 넣음)

    //아이템버리기 추가
    public GameObject itemPrefab;   //아이템프리펩 연결할 변수
    Inventory inven;    //사용시 인벤에 있는 목록지워야 하는데(일단 인벤토리연결)
     //포톤추가
    PhotonView pv; //RPC호출을 위한 포톤뷰 연결

    //아이템갯수
    public int potionCount; //획득한 아이템의 개수

    // 필요한 컴포넌트.
    [SerializeField]    //하이라이어키에 있는건 여기에 넣어도 소용없음
    private Text text_Count; //아이템 개수 표기

    //아이템사용  장착,사용 , 툴팁
    private ItemEffectDatabase theItemEffectDatabase;   //아이템 사용효과 여기서 받아옴

    //아이템버리기 추가
    //마우스 드래그가 끝났을때 발생
    public Rect invenRect; //인벤토리InventoryUI 이미지의 rect정보 받아옴                         
    private Transform playerTr;   // 자신의 Transform 참조 변수  

    void Awake()
    {
        potionCount = 0;
        pv = GetComponent<PhotonView>();  //PhotonView 컴포넌트를 레퍼런스에 할당 
        //아이템버리기 추가
        //슬롯으로부터 4번째 위에있는 인벤창 rect연결
        invenRect = transform.parent.parent.GetComponent<RectTransform>().rect;

        ////네트워크 포톤추가
    }
    private void Start()
    {
        //아이템사용효과
        theItemEffectDatabase = FindObjectOfType<ItemEffectDatabase>();
        // 플레이어 위치값 불러옴
        playerTr = GameObject.FindGameObjectWithTag("Player").transform.GetChild(5);
        inven = playerTr.root.gameObject.GetComponent<Inventory>();
    }

    public void UpdateSlotUI()  //(Slot스크립트안에 연결해준 이미지오브젝트 활성화)
    {
        itemIcon.sprite = slotItem.itemImage; //Item클래스안에 itemImage로 스프라이트 초기화
        itemIcon.gameObject.SetActive(true);    //itemIcon활성
              //아이템버리기 추가
        itemPrefab = slotItem.itemPrefab; //item클래스안에 프리펩으로

        //포션일 경우에만 카운트
        if (slotItem.itemType == ItemType.Potion)
        {
            // 아이템 카운트
            text_Count.text = potionCount.ToString();
        }
        else
        {
            text_Count.text = "1";  //아니면 숫자는 1으로
        }
    }

    public void AddItemDrag(Item _item, int _count = 1)
    {
        slotItem = _item;
        potionCount = _count;
        itemIcon.sprite = slotItem.itemImage;
        itemIcon.gameObject.SetActive(true);    //itemIcon활성
        //아이템 버리기 추가
        itemPrefab = _item.itemPrefab;  //프리펩추가

        if (slotItem.itemType == ItemType.Potion)
        {
            text_Count.text = potionCount.ToString();
        }
        else
        {
            text_Count.text = "1";
        }
    }

    //마우스 클릭시 아이템 사용 장착
    public void OnPointerClick(PointerEventData eventData)
    {
        //클릭
        if (eventData.button == PointerEventData.InputButton.Right) //우클릭
        {
            if (slotItem != null)   //슬롯칸에 아이템이 있으면
            {
                theItemEffectDatabase.UseItem(slotItem);    //아이템 사용효과 불러옴

                if (slotItem.itemType == ItemType.Potion)  //장비가 아닐경우 사라지게
                {
                    inven.itemsInven.Remove(slotItem);
                    inven.itemCount--;
                    SetSlotCount(-1);   //슬롯카운트 -1 포션은 하나씩 줄고 1일땐 사라지고
                }
                else
                {
                    inven.itemsInven.Remove(slotItem);
                    inven.itemCount--;
                    ClearSlot();
                }                
            }
        }
    }

    //아이템 갯수조정
    public void SetSlotCount(int potion)
    {
        if (slotItem.itemType == ItemType.Potion)
        {
            potionCount += potion;
            text_Count.text = potionCount.ToString();
            if (potionCount <= 0)  //카운트가 0이거나 작으면
            {
                ClearSlot();
            }
        }        
    }

    public int GetSlotCount()
    {
        return potionCount;
    }

    public void ClearSlot()    //슬롯초기화
    {
        slotItem = null;
        itemIcon.sprite = null;
        itemIcon.gameObject.SetActive(false);
        itemPrefab = null;  //프리펩   
    }


    #region 슬롯드래그

    public void OnBeginDrag(PointerEventData eventData)
    {
        
        //아이템 널이 아닐때, 슬롯의 위치는 이벤트가 발생한 위치로 넣어줌
        if (slotItem != null)
        {
            DragSlot.inst.drgSlot = this;
            DragSlot.inst.DragSettingImage(itemIcon, itemPrefab);   //이미지 바꿔줌
            DragSlot.inst.transform.position = eventData.position;  // 드래그슬롯 위치값
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        //아이템 널이 아닐때, 슬롯의 위치는 이벤트가 발생한 위치로 넣어줌
        if (slotItem != null)
        {
            DragSlot.inst.transform.position = eventData.position; //슬롯이 마우스 위치 따라다니게
        }
    }

    public void OnEndDrag(PointerEventData eventData) //초기화만 되면됨
    {
        //아에템버리기 추가(인벤밖 설정) 범위밖이면 버리기
        if (DragSlot.inst.transform.localPosition.x < invenRect.xMin ||
            DragSlot.inst.transform.localPosition.x > invenRect.xMax ||
            DragSlot.inst.transform.localPosition.y < invenRect.yMin ||
            DragSlot.inst.transform.localPosition.y > invenRect.yMax)
        {
            if (DragSlot.inst.drgSlot != null)  
            {
                GameObject dropItem = PhotonNetwork.Instantiate("ItemPrefabs/" + DragSlot.inst.drgSlot.itemPrefab.name, playerTr.position, playerTr.rotation, 0);
                DragSlot.inst.drgSlot.ClearSlot();  //드래그슬롯 초기화
                  //DragSlot.inst.drgSlot = null;
            }
        }
        else
        {
            DragSlot.inst.SettingColor(0);
            DragSlot.inst.drgSlot = null;
        }

        //아이템 버리기 추가 전에 있던 2줄
        //DragSlot.inst.SettingColor(0);
        //DragSlot.inst.drgSlot = null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log(2222222);
        if (DragSlot.inst.drgSlot != null)
        {
            ChangeSlot();
        }
    }
    private void ChangeSlot()
    {
        Item _tempItem = slotItem;  //현재 아이템 임시에 넣고
        int _tempItemCount = potionCount; //아이템카운트 임시에 넣고

        //드래그중인 아이템을 현재슬롯에 추가
        AddItemDrag(DragSlot.inst.drgSlot.slotItem, DragSlot.inst.drgSlot.potionCount);

        if (_tempItem != null)
        {
            DragSlot.inst.drgSlot.AddItemDrag(_tempItem, _tempItemCount); //전슬롯에 임시에 넣어놓은아이템추가
        }
        else
        {
            DragSlot.inst.drgSlot.ClearSlot();
        }
    }      

    #endregion

    //툴팁
    //슬롯에서 아이템이펙디비로 넘겨주고 거기서 다시 슬롯으로 넘겨주고
    // 마우스가 슬롯에 들어갈 때 발동.
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (slotItem != null)
        {
            theItemEffectDatabase.ShowToolTop(slotItem, transform.position);
        }
    }

    // 슬롯에서 빠져나갈 때 발동.
    public void OnPointerExit(PointerEventData eventData)
    {
        theItemEffectDatabase.HideToolTip();
    }

    ////버린아이템 네트워크로 알리기 (포톤추가) 위에 먹었을때 생성을 여기로 옮김
    //[PunRPC]
    //void ItemDump()
    //{
    //    //플레이어 앞에 생성
    //    //  currPos.x += 1.0f;
    //    currPos.y += -0.8f; //플레이어 y값에서 뺀거임
    //    Instantiate(DragSlot.inst.drgSlot.itemPrefab, currPos, Quaternion.identity);
    //    currPos = playerTr.position;    //플레이어 위치 다시 받으려고했는데 수치만 초기화되고, 플레이어위치는 고정
    //    DragSlot.inst.drgSlot.ClearSlot();
    //    //  DragSlot.inst.drgSlot = null;
    //}
}
