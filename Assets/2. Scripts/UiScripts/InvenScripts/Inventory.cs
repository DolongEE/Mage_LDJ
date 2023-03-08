using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    PhotonView pv;
    public static Inventory instance;
    private InventoryUI invenUi;

    //E키로 먹게 추가
    [SerializeField]
    private float range; // 습득 가능한 최대 거리
    private bool pickupActivated = false; // 습득 가능할 시 true.
    private RaycastHit hitInfo; // 충돌체 정보 저장.

    // 아이템 레이어에만 반응하도록 레이어 마스크를 설정.
    [SerializeField]
    private LayerMask layerMask;

    public Transform eye;

    // 필요한 컴포넌트.
    [SerializeField]
    private Text actionText; //행동을 보여주는 텍스트(E키를 누르시오,,)

    public List<Item> itemsInven = new List<Item>(); //획득한 아이템을 담을 리스트
    public int itemCount;

    private int slotCnt; //슬롯갯수 정하기


    void Awake()
    {
        pv = GetComponent<PhotonView>();
        
        if (pv.isMine)
        {
            //E키로 먹게 추가
            actionText = GameObject.FindGameObjectWithTag("actionTxt").GetComponent<Text>();
        }
        instance = this;

        invenUi = GameObject.Find("PlayerUiCanvas").GetComponent<InventoryUI>();

        slotCnt = invenUi.invenCnt;
        itemCount = 0;
    }

    //E키로 먹게 추가
    void Update()
    {
        if (pv.isMine)
        {
            CheckItem();
            TryAction();
        }
    }

    private void TryAction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckItem(); //E 키가 눌리면 아이템이 있는지 없는지 확인
            CanPickUp(); //아이템을 줍는다

        }
    }
    private void CheckItem()
    {
        if (Physics.Raycast(eye.position, eye.TransformDirection(Vector3.forward), out hitInfo, range, layerMask))
        {
            if (hitInfo.transform.tag == "Item" || hitInfo.transform.tag == "SpItem"|| hitInfo.transform.tag == "Quest")
            {
                ItemInfoAppear();
            }
        }
        else
        {
            InfoDisappear();
        }
    }

    //먹었다고 네트워크로 쏴줌
  
    private void CanPickUp()
    {
        if (pickupActivated)
        {
            if (hitInfo.transform != null)
            {
                ItemCtrl _item = hitInfo.transform.GetComponent<ItemCtrl>();
                Debug.Log(_item.item.itemName + " 획득했습니다");                
                AddItem(_item.GetItem());
                _item.DestItem();
                InfoDisappear();
            }
        }
    }


    //아이템 리스트에 아이템을 추가할 수 있는 메서드
    public bool AddItem(Item _item)
    {
        // 아이템 개수가 현재슬롯(SlotCnt)보다 작을 경우에만 아이템 추가
        if (itemCount < slotCnt)
        {
            itemsInven.Add(_item);
            invenUi.RedrawSlotUI(itemsInven[itemCount]);
            itemCount++;

            return true;    //성공시 true
        }
        return false;   //실패시 false
    }

    private void ItemInfoAppear()
    {
        pickupActivated = true;
        actionText.text = "";
        actionText.text = hitInfo.transform.GetComponent<ItemCtrl>().item.itemName + " 획득 " + "<color=yellow>" + "(E)" + "</color>";
    }
    private void InfoDisappear()
    {
        pickupActivated = false;
        actionText.text = "";
    }
}
