using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlot : MonoBehaviour
{
    [Header("On/Off 슬롯창")]
    [SerializeField] private GameObject ui_obj;
    [SerializeField] private GameObject selectUI_obj;


    [Header("슬롯 인벤토리")]
    [SerializeField] private GameObject[] uiSlot_obj;

    [Header("Player 스크립트")]
    [SerializeField] private PlayerAttack playerAttack;

    [Header("선택 슬롯 오브젝트")]
    [SerializeField] private Image selcet_Item;

    [Header("ItemList(체력 - 최대체력 - 마나 - 최대마나 순서)")]
    [SerializeField] private GameObject[] quickSlotItem;
    [SerializeField] private Sprite[] ItemImage;
    [Header("ItemList순서대로 5번째는 CenterInven")]
    [SerializeField] private Text[] ItemText;


    private PlayerData data;

    //선택 커서 인덱스와 슬롯인덱스를 일치시키기 위한 변수
    private int selectIndex = 0;
    private int slotSideLength = 2;
    private int selcetHold = 0;

    //아이템 받을 변수
    private List<IItem> items = new List<IItem>();

    //아이템을 종류별로    
    private List<IItem> Health_P = new List<IItem>();
    private List<IItem> MaxHealth_P = new List<IItem>();
    private List<IItem> Mana_P = new List<IItem>();
    private List<IItem> MaxMana_P = new List<IItem>();


    private void Start()
    {
        data = FindObjectOfType<PlayerData>();

        if (data != null)
        {

            data.ItemChangedEvent = OnItemAdded;

        }
        else
        {
            Debug.Log("data가 읍다");
        }
    }

    private void Update()
    {
        MoveSlotKey();

    }
    private void OnItemAdded(List<IItem> allItems)
    {
        //중복되는 것을 방지
        Health_P.Clear();
        MaxHealth_P.Clear();
        Mana_P.Clear();
        MaxMana_P.Clear();

        //var dd =  allItems.Any(x => x.Type.Equal(ItemType.HealthPotion));
        //모든 아이템을 처리
        foreach (var item in allItems)
        {
            Debug.Log(item.Name);

            if (item.Name == "체력 포션")
            {
                Health_P.Add(item);
            }
            else if (item.Name == "최대 체력 포션")
            {
                MaxHealth_P.Add(item);
            }
            else if (item.Name == "마나 포션")
            {
                Mana_P.Add(item);
            }
            else
            {
                MaxMana_P.Add(item);
            }
        }
    }
    //public void ShowItemInfo()
    //{
    //    // 마우스/선택이 해당 아이템에 올라갔나 안 올라갔나 확인하고 위의 행동을 하기.
    //    // example
    //    if (items.Count == 0)
    //    {
    //        Debug.Log(items[0].Name + " : " + items[0].Description);
    //    }
    //}
    private void MoveSlotKey()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ui_obj.SetActive(!ui_obj.activeSelf);
            selectUI_obj.SetActive(!selectUI_obj.activeSelf);

        }
        //UI Object가 true인 경우
        if (ui_obj.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                //왼쪽으로 이동
                MoveSelectUI(selectIndex - 1);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                //아래로 이동
                MoveSelectUI(selectIndex + slotSideLength);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                //오른쪽으로 이동
                MoveSelectUI(selectIndex + 1);
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                //위로 이동
                MoveSelectUI(selectIndex - slotSideLength);
            }

            //선택 아이템
            if (Input.GetKeyDown(KeyCode.E))
            {
                SelectItem();
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            //아이템 사용 메서드
            UseItemSet();
        }
        //아이템 갯수 출력
        ItemCount();
    }

    private int MoveSelectUI(int offset)
    {
        if (offset < 0)
        {
            if (selectIndex == 1)
            {
                offset = 1;
            }
            else
            {
                offset = 0;
                selectIndex = offset;
            }
        }
        else if (offset > uiSlot_obj.Length - 1)
        {
            if (selectIndex == 2)
            {
                offset = 2;
            }
            else
            {
                offset = uiSlot_obj.Length - 1;
                selectIndex = offset;
            }
        }
        else
        {
            selectIndex = offset;
        }

        selectUI_obj.transform.position =
            uiSlot_obj[offset].transform.position + new Vector3(-6f, 60f, 0);

        return selectIndex;

    }
    //선택한 아이템으로 Sprite 변경
    private void SelectItem()
    {
        selcet_Item.sprite = ItemImage[selectIndex];
        selcetHold = selectIndex;
    }
    private void ItemCount()
    {

        //여기에서 아이템 갯수 텍스트로 출력
        ItemText[0].text = $"{ Health_P.Count }";
        ItemText[1].text = $"{ MaxHealth_P.Count }";
        ItemText[2].text = $"{ Mana_P.Count }";
        ItemText[3].text = $"{ MaxMana_P.Count }";

        //if문 길게 쓰기 싫어서 list로 줄여봄 그리고 선택된 슬롯은 계속 변하면 안되기에 여기에 선언
        List<IItem>[] potionLists = { Health_P, MaxHealth_P, Mana_P, MaxMana_P };

        // selectIndex에 따라 다른 포션의 갯수 출력
        if (selcetHold >= 0 && selcetHold < potionLists.Length)
        {            
            ItemText[4].text = $"{potionLists[selcetHold].Count}";
        }
    }
    private void UseItemSet()
    {
        switch (selcetHold)
        {
            case 0:
                if (Health_P.Count>0)
                {
                    data.UseItem(Health_P[0]);
                }
                return;
            case 1:
                if (MaxHealth_P.Count > 0)
                {
                    data.UseItem(MaxHealth_P[0]);
                }
                return;
            case 2:
                if (Mana_P.Count>0)
                {
                    data.UseItem(Mana_P[0]);
                }                
                return;
            case 3:
                if (MaxMana_P.Count>0)
                {
                    data.UseItem(MaxMana_P[0]);
                }
                return;
            default:
                break;
        }
    }
    
}
    