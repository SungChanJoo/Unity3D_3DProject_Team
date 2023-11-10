using UnityEngine;

class HealthPotion : MonoBehaviour, IItem
{
    public string Name { get => "체력 포션"; }
    public string Description { get => "체력을 20만큼 회복시켜준다."; }

    public void Use(PlayerData player)
    {
        player.RestoreHealth(20);
    }
}
