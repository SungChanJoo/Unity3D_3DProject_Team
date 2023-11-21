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

    //0~3 아이템 갯수, 4 선택아이템 갯수, 5 아이템 정보
    [Header("ItemList순서대로 5번째는 CenterInven")]
    [SerializeField] private Text[] ItemText;

    //방패 가리기
    [SerializeField] private GameObject shield;

    private CameraController player;
    private PlayerData data;
    private Animator ani;

    //선택 커서 인덱스와 슬롯인덱스를 일치시키기 위한 변수
    private int selectIndex = 0;
    private int slotSideLength = 2;
    private int selcetHold = 0;
      
    //아이템을 종류별로    
    private List<IItem> health_P = new List<IItem>();
    private List<IItem> maxHealth_P = new List<IItem>();
    private List<IItem> mana_P = new List<IItem>();
    private List<IItem> maxMana_P = new List<IItem>();

    [Header("포션 이펙트")]    
    [SerializeField] private GameObject health_E;
    [SerializeField] private GameObject MaxHealth_E;
    [SerializeField] private GameObject mana_E;
    [SerializeField] private GameObject MaxMana_E;   

    //UI 창 온오프 조건
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
            Debug.Log("data가 읍다");
        }
    }

    private void Update()
    {
        MoveSlotKey();
    }

    #region // PlayerData로 받은 아이템 데이터를 분배
    private void OnItemAdded(List<IItem> allItems)
    {
        //중복되는 것을 방지
        health_P.Clear();
        maxHealth_P.Clear();
        mana_P.Clear();
        maxMana_P.Clear();

        //var dd =  allItems.Any(x => x.Type.Equal(ItemType.HealthPotion));
        //모든 아이템을 처리
        foreach (var item in allItems)
        {            
            Debug.Log(item.Name);

            if (item.Name == "체력 포션")
            {
                health_P.Add(item);
            }
            else if (item.Name == "최대 체력 포션")
            {
                maxHealth_P.Add(item);
            }
            else if (item.Name == "마나 포션")
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

    #region // 아이템 정보
    public void ShowItemInfo()
    {
        switch (selectIndex)
        {
            case 0:
                ItemText[5].text = "체력 포션 : 체력을 '20' 만큼 회복시켜준다.";
                return;
            case 1:
                ItemText[5].text = "최대 체력 포션 : 최대 체력을 '20' 만큼 늘려준다.";
                return;
            case 2:
                ItemText[5].text = "마나 포션 : 마나를 '20' 만큼 회복시켜준다.";
                return;
            case 3:
                ItemText[5].text = "최대 마나 포션 : 최대 마나를 '20' 만큼 늘려준다.";
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
                StartCoroutine(FadeUI(ui_obj, 0f, 1f, 0.5f)); // 투명도를 0에서 1로 변경 (나타나기)
                StartCoroutine(FadeUI(selectUI_obj, 0f, 1f, 0.5f));
            }
            else
            {                
                Vector3 selectUI_objY = selectUI_obj.transform.position + new Vector3(0, -500, 0);
                Vector3 ui_objY = ui_obj.transform.position + new Vector3(0, -500, 0);
                StartCoroutine(MoveUI(ui_obj, ui_objY, 0.5f));
                StartCoroutine(MoveUI(selectUI_obj, selectUI_objY, 0.5f));
                StartCoroutine(FadeUI(ui_obj, 1f, 0f, 0.5f)); // 투명도를 1에서 0으로 변경 (사라지기)
                StartCoroutine(FadeUI(selectUI_obj, 1f, 0f, 0.5f));
            }
        }
        //UI Object가 true인 경우
        if (on)
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

        }

        //선택 아이템
        if (Input.GetKeyDown(KeyCode.Tab) && !on)
        {
            SelectItem();
        }

        //플레이어가 정지상태가 아니며 포션의 수량이 0보다 클 때 실행
        if (Input.GetKeyDown(KeyCode.Q)&&!playerAttack.hold
            &&playerAttack.attackEnabled&&!player.isRolling)
        {
            List<IItem>[] potionLists = { health_P, maxHealth_P, mana_P, maxMana_P };
            if (potionLists[selcetHold].Count>0)
            {
                //아이템 사용 메서드
                ani.SetTrigger("Drinking");
                StartCoroutine(Drinking());
            }
        }
        //아이템 갯수 출력
        ItemCount();
        ShowItemInfo();
    }

    #region // 퀵슬롯 커서 움직임 로직(selectIndex 반환)
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

    #region // 선택한 아이템으로 Sprite 변경 및 홀드
    private void SelectItem()
    {
        selcet_Item.sprite = ItemImage[selectIndex];
        selcetHold = selectIndex;
    }
    #endregion

    #region // 아이템 수량 출력 메서드
    private void ItemCount()
    {
        //여기에서 아이템 갯수 텍스트로 출력
        ItemText[0].text = $"{ health_P.Count }";
        ItemText[1].text = $"{ maxHealth_P.Count }";
        ItemText[2].text = $"{ mana_P.Count }";
        ItemText[3].text = $"{ maxMana_P.Count }";

        //if문 길게 쓰기 싫어서 list로 줄여봄 그리고 선택된 슬롯은 계속 변하면 안되기에 여기에 선언
        List<IItem>[] potionLists = { health_P, maxHealth_P, mana_P, maxMana_P };

        // selectIndex에 따라 다른 포션의 갯수 출력
        if (selcetHold >= 0 && selcetHold < potionLists.Length)
        {            
            ItemText[4].text = $"{potionLists[selcetHold].Count}";
        }        
    }
    #endregion

    #region // 아이템 사용 메서드
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
        //시전 중 맞을 시 반환
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


    //부드러운 UI 끄고 크기 
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

        canvasGroup.alpha = targetAlpha; // 최종 투명도 보정
    }
}



