using UnityEngine;

class MaxManaPotion : MonoBehaviour, IItem
{
    public string Name { get => "최대 마나 포션"; }
    public string Description { get => "최대 마나를 20만큼 늘려준다."; }
    public void Use(PlayerData player)
    {
        player.IncreaseMaxMana(20);
    }
}