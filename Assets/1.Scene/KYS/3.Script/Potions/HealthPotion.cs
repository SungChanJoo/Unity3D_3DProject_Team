using UnityEngine;

class HealthPotion : MonoBehaviour, IItem
{
    public string Name { get => "ü�� ����"; }
    public string Description { get => "ü���� 20��ŭ ȸ�������ش�."; }
    public void Use(PlayerData player)
    {
        player.RestoreHealth(20);
    }
}