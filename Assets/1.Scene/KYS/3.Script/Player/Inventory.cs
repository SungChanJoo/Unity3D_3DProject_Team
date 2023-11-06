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

    // Item이 가지고 있는 정보 (string name, string desc 등)을 Item이 가지고 있는 Canvas에 보이게 하는 함수가 IItem 내에!! 있고
    // (-> 이걸 위해서는 IItem이 아니라 ItemBase가 되어야겠는디)
    // Item 클래스 내 ShowDescTooltip() 이라고 하면 그 캔버스가 보이게 하기
    public void ShowItemInfo()
    {
        // 마우스/선택이 해당 아이템에 올라갔나 안 올라갔나 확인하고 위의 행동을 하기.
    }

    // 선택한 무기 성장 시킬 수 있는 메소드 구현할 수도 있음.
}
