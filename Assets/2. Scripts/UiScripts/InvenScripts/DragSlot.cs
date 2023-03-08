using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragSlot : MonoBehaviour {

    static public DragSlot inst;

    public Slot drgSlot;

    // 아이템 이미지.
    [SerializeField]
    private Image dragItem;

    //아이템버리기 추가
    private GameObject dragitemPrefab;

    void Start()
    {
        inst = this;
    }

	public void DragSettingImage(Image _itemImage, GameObject _itemPrefab)
    {
        dragItem.sprite = _itemImage.sprite;
        dragitemPrefab = _itemPrefab;
        SettingColor(1);
    }

    public void SettingColor(float _alpha)
    {
        Color color = dragItem.color;
        color.a = _alpha;
        dragItem.color = color;
    }
}
