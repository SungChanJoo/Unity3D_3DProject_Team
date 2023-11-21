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

    //0~3 ������ ����, 4 ���þ����� ����, 5 ������ ����
    [Header("ItemList������� 5��°�� CenterInven")]
    [SerializeField] private Text[] ItemText;

    //���� ������
    [SerializeField] private GameObject shield;

    private CameraController player;
    private PlayerData data;
    private Animator ani;

    //���� Ŀ�� �ε����� �����ε����� ��ġ��Ű�� ���� ����
    private int selectIndex = 0;
    private int slotSideLength = 2;
    private int selcetHold = 0;
      
    //�������� ��������    
    private List<IItem> health_P = new List<IItem>();
    private List<IItem> maxHealth_P = new List<IItem>();
    private List<IItem> mana_P = new List<IItem>();
    private List<IItem> maxMana_P = new List<IItem>();

    [Header("���� ����Ʈ")]    
    [SerializeField] private GameObject health_E;
    [SerializeField] private GameObject MaxHealth_E;
    [SerializeField] private GameObject mana_E;
    [SerializeField] private GameObject MaxMana_E;   

    //UI â �¿��� ����
    private bool on = false;
    private bool play_C = false;

    [SerializeField] private GameObject[] hideUI_obj;

    private void Start()
    {
        player = FindObjectOfType<CameraController>();
        data = FindObjectOfType<PlayerData>();
        ani =player.GetComponent<Animator>();
        

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

    #region // PlayerData�� ���� ������ �����͸� �й�
    private void OnItemAdded(List<IItem> allItems)
    {
        //�ߺ��Ǵ� ���� ����
        health_P.Clear();
        maxHealth_P.Clear();
        mana_P.Clear();
        maxMana_P.Clear();

        //var dd =  allItems.Any(x => x.Type.Equal(ItemType.HealthPotion));
        //��� �������� ó��
        foreach (var item in allItems)
        {            
            Debug.Log(item.Name);

            if (item.Name == "ü�� ����")
            {
                health_P.Add(item);
            }
            else if (item.Name == "�ִ� ü�� ����")
            {
                maxHealth_P.Add(item);
            }
            else if (item.Name == "���� ����")
            {
                mana_P.Add(item);
            }
            else
            {
                maxMana_P.Add(item);
            }
        }
        
    }
    #endregion

    #region // ������ ����
    public void ShowItemInfo()
    {
        switch (selectIndex)
        {
            case 0:
                ItemText[5].text = "ü�� ���� : ü���� '20' ��ŭ ȸ�������ش�.";
                return;
            case 1:
                ItemText[5].text = "�ִ� ü�� ���� : �ִ� ü���� '20' ��ŭ �÷��ش�.";
                return;
            case 2:
                ItemText[5].text = "���� ���� : ������ '20' ��ŭ ȸ�������ش�.";
                return;
            case 3:
                ItemText[5].text = "�ִ� ���� ���� : �ִ� ������ '20' ��ŭ �÷��ش�.";
                return;
            default:
                break;
        }
    }
    #endregion

    private void MoveSlotKey()
    {        
        if (Input.GetKeyDown(KeyCode.Tab)&&!play_C)
        {
            on = !on;
            if (on)
            {
                Vector3 selectUI_objY = selectUI_obj.transform.position + new Vector3(0, 500, 0);
                Vector3 ui_objY = ui_obj.transform.position + new Vector3(0, 500, 0);
                StartCoroutine(MoveUI(ui_obj, ui_objY, 0.5f));
                StartCoroutine(MoveUI(selectUI_obj, selectUI_objY, 0.5f));
                StartCoroutine(FadeUI(ui_obj, 0f, 1f, 0.5f)); // ������ 0���� 1�� ���� (��Ÿ����)
                StartCoroutine(FadeUI(selectUI_obj, 0f, 1f, 0.5f));
            }
            else
            {                
                Vector3 selectUI_objY = selectUI_obj.transform.position + new Vector3(0, -500, 0);
                Vector3 ui_objY = ui_obj.transform.position + new Vector3(0, -500, 0);
                StartCoroutine(MoveUI(ui_obj, ui_objY, 0.5f));
                StartCoroutine(MoveUI(selectUI_obj, selectUI_objY, 0.5f));
                StartCoroutine(FadeUI(ui_obj, 1f, 0f, 0.5f)); // ������ 1���� 0���� ���� (�������)
                StartCoroutine(FadeUI(selectUI_obj, 1f, 0f, 0.5f));
            }
        }
        //UI Object�� true�� ���
        if (on)
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

        }

        //���� ������
        if (Input.GetKeyDown(KeyCode.Tab) && !on)
        {
            SelectItem();
        }

        //�÷��̾ �������°� �ƴϸ� ������ ������ 0���� Ŭ �� ����
        if (Input.GetKeyDown(KeyCode.Q)&&!playerAttack.hold
            &&playerAttack.attackEnabled&&!player.isRolling)
        {
            List<IItem>[] potionLists = { health_P, maxHealth_P, mana_P, maxMana_P };
            if (potionLists[selcetHold].Count>0)
            {
                //������ ��� �޼���
                ani.SetTrigger("Drinking");
                StartCoroutine(Drinking());
            }
        }
        //������ ���� ���
        ItemCount();
        ShowItemInfo();
    }

    #region // ������ Ŀ�� ������ ����(selectIndex ��ȯ)
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
    #endregion    

    #region // ������ ���������� Sprite ���� �� Ȧ��
    private void SelectItem()
    {
        selcet_Item.sprite = ItemImage[selectIndex];
        selcetHold = selectIndex;
    }
    #endregion

    #region // ������ ���� ��� �޼���
    private void ItemCount()
    {
        //���⿡�� ������ ���� �ؽ�Ʈ�� ���
        ItemText[0].text = $"{ health_P.Count }";
        ItemText[1].text = $"{ maxHealth_P.Count }";
        ItemText[2].text = $"{ mana_P.Count }";
        ItemText[3].text = $"{ maxMana_P.Count }";

        //if�� ��� ���� �Ⱦ list�� �ٿ��� �׸��� ���õ� ������ ��� ���ϸ� �ȵǱ⿡ ���⿡ ����
        List<IItem>[] potionLists = { health_P, maxHealth_P, mana_P, maxMana_P };

        // selectIndex�� ���� �ٸ� ������ ���� ���
        if (selcetHold >= 0 && selcetHold < potionLists.Length)
        {            
            ItemText[4].text = $"{potionLists[selcetHold].Count}";
        }        
    }
    #endregion

    #region // ������ ��� �޼���
    private void UseItemSet()
    {
        switch (selcetHold)
        {
            case 0:
                if (health_P.Count>0)
                {
                    data.UseItem(health_P[0]);
                }
                break;
            case 1:
                if (maxHealth_P.Count > 0)
                {
                    data.UseItem(maxHealth_P[0]);
                }
                break;
            case 2:
                if (mana_P.Count>0)
                {
                    data.UseItem(mana_P[0]);
                }                
                break;
            case 3:
                if (maxMana_P.Count>0)
                {
                    data.UseItem(maxMana_P[0]);
                }
                break;
        }
        StartCoroutine(PotionEffect(selcetHold));
    }
    #endregion

    private IEnumerator Drinking()
    {
        player.rolling = false;
        playerAttack.attackEnabled = false;
        shield.SetActive(false);
        yield return new WaitForSeconds(1.8f);
        player.rolling = true;
        playerAttack.attackEnabled= true;
        shield.SetActive(true);
        ani.SetTrigger("Default");
        //���� �� ���� �� ��ȯ
        if (data.stop)
        {
            data.stop = false;
            yield return null;
        }
        UseItemSet();
    }
    private IEnumerator PotionEffect(int type)
    {
        switch (type)
        {
            case 0 :
                health_E.SetActive(true);
                break;
            case 1:
                MaxHealth_E.SetActive(true);
                break;
            case 2:
                mana_E.SetActive(true);
                break;
            case 3:
                MaxMana_E.SetActive(true);
                break;                
        }
        yield return new WaitForSeconds(3f);
        health_E.SetActive(false);
        MaxHealth_E.SetActive(false);
        mana_E.SetActive(false);
        MaxMana_E.SetActive(false);
    }


    //�ε巯�� UI ���� ũ�� 
    private IEnumerator MoveUI(GameObject uiObject, Vector3 targetPosition, float duration)
    {
        play_C = true;
        float timeElapsed = 0f;
        Vector3 initialPosition = uiObject.transform.position;

        while (timeElapsed < duration)
        {
            uiObject.transform.position = Vector3.Lerp(initialPosition, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        uiObject.transform.position = targetPosition;
        play_C = false;
    }
    IEnumerator FadeUI(GameObject uiObject, float startAlpha, float targetAlpha, float duration)
    {
        CanvasGroup canvasGroup = uiObject.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = uiObject.AddComponent<CanvasGroup>();
        }

        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = targetAlpha; // ���� ���� ����
    }
}



