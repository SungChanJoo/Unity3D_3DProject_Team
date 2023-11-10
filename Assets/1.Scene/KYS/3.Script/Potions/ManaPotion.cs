using UnityEngine;
class ManaPotion : MonoBehaviour, IItem
{
    public string Name { get => "마나 포션"; }
    public string Description { get => "마나를 20만큼 회복시켜준다."; }
    public void Use(PlayerData player)
    {
        player.RestoreMana(20);
    }
}