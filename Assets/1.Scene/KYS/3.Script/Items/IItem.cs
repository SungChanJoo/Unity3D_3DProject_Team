public interface IItem
{
    public string Name { get; }
    public string Description { get; }

    // Inventory에서 PlayerData를 참조하고, 그걸 IItem이 사용될 때마다 넘겨주는 방식으로 하는 것이 Player가 최소한으로 참조되는 방식임.
    public void Use(PlayerData player);
}
