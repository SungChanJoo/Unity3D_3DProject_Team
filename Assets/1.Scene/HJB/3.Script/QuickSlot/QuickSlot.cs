using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlot : MonoBehaviour
{
    [Header("On/Off ����â")]
    [SerializeField] private GameObject ui_obj;
    [SerializeField] private GameObject selectUI_obj;


    [Header("���� �κ��丮")]
    [SerializeField] private GameObject[] uiSlot_obj;

    [Header("Player ��ũ��Ʈ")]
    [SerializeField] private PlayerAttack playerAttack;

    [Header("���� ���� ������Ʈ")]
    [SerializeField] private Image selcet_Item;

    [Header("ItemList(ü�� - �ִ�ü�� - ���� - �ִ븶�� ����)")]
    [SerializeField] private GameObject[] quickSlotItem;
    [SerializeField] private Sprite[] ItemImage;
    [Header("ItemList������� 5��°�� CenterInven")]
    [SerializeField] private Text[] ItemText;


    private PlayerData data;

    //���� Ŀ�� �ε����� �����ε����� ��ġ��Ű�� ���� ����
    private int selectIndex = 0;
    private int slotSideLength = 2;
    private int selcetHold = 0;

    //������ ���� ����
    private List<IItem> items = new List<IItem>();

    //�������� ��������    
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
            Debug.Log("data�� ����");
        }
    }

    private void Update()
    {
        MoveSlotKey();

    }
    private void OnItemAdded(List<IItem> allItems)
    {
        //�ߺ��Ǵ� ���� ����
        Health_P.Clear();
        MaxHealth_P.Clear();
        Mana_P.Clear();
        MaxMana_P.Clear();

        //var dd =  allItems.Any(x => x.Type.Equal(ItemType.HealthPotion));
        //��� �������� ó��
        foreach (var item in allItems)
        {
            Debug.Log(item.Name);

            if (item.Name == "ü�� ����")
            {
                Health_P.Add(item);
            }
            else if (item.Name == "�ִ� ü�� ����")
            {
                MaxHealth_P.Add(item);
            }
            else if (item.Name == "���� ����")
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
    //    // ���콺/������ �ش� �����ۿ� �ö󰬳� �� �ö󰬳� Ȯ���ϰ� ���� �ൿ�� �ϱ�.
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
        //UI Object�� true�� ���
        if (ui_obj.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                //�������� �̵�
                MoveSelectUI(selectIndex - 1);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                //�Ʒ��� �̵�
                MoveSelectUI(selectIndex + slotSideLength);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                //���������� �̵�
                MoveSelectUI(selectIndex + 1);
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                //���� �̵�
                MoveSelectUI(selectIndex - slotSideLength);
            }

            //���� ������
            if (Input.GetKeyDown(KeyCode.E))
            {
                SelectItem();
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            //������ ��� �޼���
            UseItemSet();
        }
        //������ ���� ���
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
    //������ ���������� Sprite ����
    private void SelectItem()
    {
        selcet_Item.sprite = ItemImage[selectIndex];
        selcetHold = selectIndex;
    }
    private void ItemCount()
    {

        //���⿡�� ������ ���� �ؽ�Ʈ�� ���
        ItemText[0].text = $"{ Health_P.Count }";
        ItemText[1].text = $"{ MaxHealth_P.Count }";
        ItemText[2].text = $"{ Mana_P.Count }";
        ItemText[3].text = $"{ MaxMana_P.Count }";

        //if�� ��� ���� �Ⱦ list�� �ٿ��� �׸��� ���õ� ������ ��� ���ϸ� �ȵǱ⿡ ���⿡ ����
        List<IItem>[] potionLists = { Health_P, MaxHealth_P, Mana_P, MaxMana_P };

        // selectIndex�� ���� �ٸ� ������ ���� ���
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
    