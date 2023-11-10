using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private PlayerData player;

    private List<IItem> items = new List<IItem>();

    private void Update()
    {
        // test
        if (Input.GetKeyDown(KeyCode.Alpha0)
            && items.Count > 0)
        {
            Debug.Log($"현재 아이템 개수 : {items.Count}, 사용하게 될 아이템은 {items[0].Name}");

            UseItem(items[0]);

            Debug.Log($"사용하게 되어서 남은 아이템 개수는 {items.Count}");
        }
    }

    // 무기든, 방어구든, 소모품이든 이걸 통함
    public void UseItem(IItem item)
    {
        item.Use(player);
        items.Remove(item);
    }

    public void ShowItemInfo()
    {
        // 마우스/선택이 해당 아이템에 올라갔나 안 올라갔나 확인하고 위의 행동을 하기.

        // example
        if(items.Count == 0)
        {
            Debug.Log(items[0].Name + " : " + items[0].Description);
        }
    }

    // 아이템을 만났을 때 저장하는 것도 인벤토리에서 체크함
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out IItem item)) return;

        items.Add(item);
        Destroy(other.gameObject);

        Debug.Log(item.Name + "을 획득. 인벤토리에 아이템이 " + items.Count+ "만큼 있음");
    }
}
