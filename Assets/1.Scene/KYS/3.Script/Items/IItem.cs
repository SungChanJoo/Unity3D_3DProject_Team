using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 무기, 방어구, 소모품가 상속하는 인터페이스
// 추상 클래스나 ScriptableObject로 구현하는 편이 좋을지도 모르겠다.
public interface IItem
{
    // Inventory에서 PlayerData를 참조하고, 그걸 IItem이 사용될 때마다 넘겨주는 방식으로 하는 것이 Player가 최소한으로 참조되는 방식임.
    public void Use(PlayerData player);

    public void Store();

    public void Drop();
}
