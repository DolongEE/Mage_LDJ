using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class InventoryUI : MonoBehaviour//, IBeginDragHandler //, IPointerClickHandler
{
    Inventory inven;

    // 인벤토리 패널 연결
    public GameObject inventoryPanel;
    bool activeInventory = false;
    // 도움말 패널 연결
    public GameObject helpPanel;
    bool activeHelp = false;
    public GameObject EffectSystem;

    //슬롯 연결
    public Slot[] slots;
    public Transform slotHolder;

    public int invenCnt;
    bool isAct;
    public string mapName;

    void Awake()
    {
        inven = Inventory.instance;

        slots = slotHolder.GetComponentsInChildren<Slot>();
    }

    void Start()
    {
        inventoryPanel.SetActive(activeInventory);
        helpPanel.SetActive(activeHelp);    // 도움말 ui처음에 비활성
        invenCnt = slotHolder.childCount;
    }

    // 씬 변경시 Slot 초기화 함수들
    #region 씬 변경시 Slot초기화
    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneLoadClear;
        if (!isAct)
        {
            Debug.Log("실행함");
            SceneManager.sceneLoaded += OnDB;
        }
        SceneManager.sceneLoaded += MapName;
    }
    void SceneLoadClear(Scene scene, LoadSceneMode mode)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            // 쓰레기 값이 있을때 초기화, 아이템일 경우에는 초기화 안함
            if (slots[i].slotItem != null && slots[i].slotItem.itemId == 0)
            {
                slots[i].ClearSlot();
            }
        }
        activeInventory = false;
        inventoryPanel.SetActive(activeInventory);
    }
    void OnDB(Scene scene, LoadSceneMode mode)
    {
        if(SceneManager.GetActiveScene().name == "scMain")
        {
            EffectSystem.SetActive(true);
            isAct = true;
        }
    }
    void MapName(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "scSnowCaveMap")
        {
            mapName = "scSnowCaveMap";
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneLoadClear;
        if (!isAct)
        {
            SceneManager.sceneLoaded -= OnDB;
        }
        SceneManager.sceneLoaded -= MapName;
    } 

    #endregion

    void Update()
    {
        // i 키 누를때마다 인벤토리 활성 비활성
        if (Input.GetKeyDown(KeyCode.I))
        {
            activeInventory = !activeInventory;
            inventoryPanel.SetActive(activeInventory);
        }
        // h 키 누를때마다 팁창 활성 비활성
        if (Input.GetKeyDown(KeyCode.H))
        {
            activeHelp = !activeHelp;
            helpPanel.SetActive(activeHelp);
        }
    }

    //아이템 먹으면 인벤에 이미지 띄우기 
    public void RedrawSlotUI(Item _item)
    {
        
        if (_item.itemType == ItemType.Potion)
        {
            for (int i = 0; i < invenCnt; i++)
            { 
                if (slots[i].slotItem != null && slots[i].slotItem.itemType == ItemType.Potion)
                {
                    slots[i].SetSlotCount(1);
                    return;
                }
            }
            Debug.Log(_item.itemName);
            SetSlot(_item);
        }
        else
        {
            SetSlot(_item);
        }
    }
    void SetSlot(Item _item)
    {
        for (int i = 0; i < invenCnt; i++)
        {
            if (slots[i].slotItem == null) // 인벤토리에 빈공간이 있으면
            {
                Debug.Log("아이템 정보 입력");
                slots[i].slotItem = _item; // 해당 슬롯에 정보 입력
                slots[i].UpdateSlotUI();    // 인벤토리 슬롯 칸에 아이템이미지 활성화
                slots[i].SetSlotCount(1);
                return;
            }
        }
    }
}