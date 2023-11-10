using UnityEngine;
class MaxHealthPotion : MonoBehaviour, IItem
{
    public string Name { get => "최대 체력 포션"; }
    public string Description { get => "최대 체력을 20만큼 늘려준다."; }
    public void Use(PlayerData player)
    {
        player.IncreaseMaxHealth(20);
    }
}