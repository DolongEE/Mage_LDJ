//아이템사용 효과(장비, 포션사용, 퀘스트, 툴팁) 스크립트

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemEffectDatabase : MonoBehaviour
{
    //아이템장착슬롯(배열로받을까 하다 알아보기 쉽게 따로 함)
    public Image ESlotLUImg;    //장비슬롯 왼쪽위 안에 이미지
    public Image ESlotLDImg;    //장비슬롯 왼쪽아래
    public Image ESlotRUImg;    //장비슬롯 오른쪽위
    public Image ESlotRDImg;    //장비슬롯 오른쪽아래

    //원소 연결
    public Image[] SpSlots;     //불, 물, 바람, 대지 ,심장    

    //툴팁
    [SerializeField]
    private SlotToolTip theSlotToolTip;
    
    //포션사용
    public int healingPoint = 0;

    //원소 장착시 포톤추가
    PhotonView pv = null;

    public PlayerCtrl playerCtrl;
    //원소장착시 이펙
    public GameObject footeffect;  //플레이어 밑에 발 파티클연결
    public MapTip mtp;  //퀘스트없애주기 위해 맵팁연결

    int netSp;
    private void Awake()
    {
        netSp = 0;
        pv = GetComponent<PhotonView>();
        mtp = GameObject.Find("PlayerUiManager").GetComponent<MapTip>();
    }

    IEnumerator Start()
    {       
        yield return new WaitForSeconds(0.5f);
        if (pv.isMine)
        {
            playerCtrl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl>();
            footeffect = playerCtrl.footeft;    //플레이어에 연결 효과 가져와서 여기서 조절
        }

        yield return null;
    }
    //툴팁
    //슬롯에서 여기로 넘겨주고 여기서 다시 슬롯으로 넘겨주고
    public void ShowToolTop(Item _item, Vector3 _pos)
    {
        theSlotToolTip.ShowToolTip(_item, _pos);
    }

    //사라지는
    public void HideToolTip()
    {
        theSlotToolTip.HideToolTip();
    }

    public void UseItem(Item _item)
    {
        //넘어온 아이템이 장비인지 유즈인지 비교
        if (_item.itemType == ItemType.Equipment)  //장비일경우
        {
            //장비슬롯에 데이터와 함께 이동시키고, 플레이어 발에 이펙나오게 해야함
            Debug.Log("장비착용");
            EquipmentEffect(_item);
        }
        else if (_item.itemType == ItemType.Potion) //포션일경우
        {
            Debug.Log("아이템 사용 체력이 올랐습니다");
            playerCtrl.GetHealPotion(healingPoint);
            //Hpplus();
        }
        else if (_item.itemType == ItemType.Vision)  //특수아이템일경우
        {
            Debug.Log("원소아이템 획득");
            SpecilEffect(_item);        }
    }
    
     //장비 이펙트효과 함수
    void EquipmentEffect(Item _item)
    {
       // pSlotLUImg.gameObject.SetActive(true);  //이미지 활성화
        //장비슬롯
        if (_item.itemName == "DendroSeed")     //대지씨앗
        {
            ESlotLUImg.sprite = _item.itemImage; //아이템이미지 연결
            ESlotLUImg.gameObject.SetActive(true);  //이미지 활성화
            //만약 플레이어가 퀘스트 아이템이 있으면 어느 위치에서 애니 트리거 발동
            // 또는 가지고 있으면 애니발동
         //   효과.gameObject.SetActive(true);    //효과
        }
        else if (_item.itemName == "Cryoshoes")     //얼음신발
        {
            ESlotLDImg.sprite = _item.itemImage; //아이템이미지 연결
            ESlotLDImg.gameObject.SetActive(true);  //인벤 장착 이미지 활성화
            footeffect.gameObject.SetActive(true);    //효과(발에 이펙트 활성화)
        }
        else if (_item.itemName == "Sickle")    //낫
        {
            ESlotRUImg.sprite = _item.itemImage; //아이템이미지 연결
            ESlotRUImg.gameObject.SetActive(true);  //이미지 활성화
             //만약 플레이어가 퀘스트 아이템이 있으면 어느 위치에서 애니 트리거 발동
            // 또는 가지고 있으면 애니발동
         //   효과.gameObject.SetActive(true);    //효과
        }
        else if (_item.itemName == "Torch")     //토치
        {
            ESlotRDImg.sprite = _item.itemImage; //아이템이미지 연결
            ESlotRDImg.gameObject.SetActive(true);  //이미지 활성화
             //만약 플레이어가 퀘스트 아이템이 있으면 어느 위치에서 애니 트리거 발동
            // 또는 가지고 있으면 애니발동
         //   효과.gameObject.SetActive(true);    //효과
        }
    }

    //플레이어 hp연결 +해주는 함수
    void Hpplus()
    {
        //if (hp <= 맥스값)
        //{  
        //    healingPoint = 설정값(+20)한hp
        //    플레이어 hp = healingPoint
        //}
        //else
        //{
        //    플레이어 hp = 플레이어 hp
        //}
    }

    [PunRPC]
    void NetSpEffect(int spc)
    {
        Debug.Log("hi");
        SpSlots[spc].gameObject.SetActive(true);
        if(PhotonNetwork.isMasterClient)
        {
            if (spc == 0)
            {
                playerCtrl.getPyro = true;
            }
            else if (spc == 1)
            {
                playerCtrl.getHydro = true;
            }
            else if (spc == 2)
            {
                playerCtrl.getAnemo = true;
            }
            else if (spc == 3)
            {
                playerCtrl.getGeo = true;
            }
        }
    }

    public void SpecilEffect(Item _item)
    {
        if (_item.itemName == "Pyro")    //불
        {
            //SpSlots[0].gameObject.SetActive(true);
            netSp = 0;
            //퀘스트창 원소획득시 삭제
        //    Button fire = mtp.questBtn[1];
            GameObject fire = GameObject.Find("fireButton");
            Destroy(fire);
        }
        else if (_item.itemName == "Hydro") //물
        {
            //SpSlots[1].gameObject.SetActive(true);
            netSp = 1;

            //퀘스트창 원소획득시 삭제
            //   Button ice = mtp.questBtn[2];
            GameObject ice = GameObject.Find("iceButton");
            Destroy(ice);
        }
        else if (_item.itemName == "Anemo") //바람
        {
            //SpSlots[2].gameObject.SetActive(true);
            netSp = 2;

            //퀘스트창 원소획득시 삭제
            //   Button wind = mtp.questBtn[3];
            GameObject wind = GameObject.Find("foreastButton");
            Destroy(wind);
        }
        else if (_item.itemName == "Geo") //대지
        {
            //SpSlots[3].gameObject.SetActive(true);
            netSp = 3;

            //퀘스트창 원소획득시 삭제
            //   Button earth = mtp.questBtn[4];
            GameObject earth = GameObject.Find("earthButton");
            Destroy(earth);
        }
        //else if (_item.itemName == "Orichalcumheart") //심장
        //{
        //    //SpSlots[4].gameObject.SetActive(true);
        //    netSp = 4;           
        //}
        pv.RPC("NetSpEffect", PhotonTargets.All, netSp);
    }
}