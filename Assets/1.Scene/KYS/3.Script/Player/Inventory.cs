using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private PlayerData player;

    // 인벤토리에 들어가 있는 모든 아이템의 집합체 (무기, 방어구, 소모품)
    private List<IItem> items;

    // 무기든, 방어구든, 소모품이든 이걸 통함
    public void UseItem(IItem item)
    {
        item.Use(player);

        items.Remove(item);
        // Destroy(item);
    }

    // 선택한 무기 성장 시킬 수 있는 메소드 구현할 수도 있음.
}
