using System;
using UnityEngine;

public class ShopPanel : MonoBehaviour
{
    private ShopData shop;

    public ShopData Shop => shop;
    public event Action Refreshed;

    public void Bind(ShopData target)
    {
        if (shop != null) shop.Changed -= Refresh;
        shop = target;
        if (shop != null) shop.Changed += Refresh;
        Refresh();
    }

    public void Refresh()
    {
        Refreshed?.Invoke();
    }

    private void OnDestroy()
    {
        if (shop != null) shop.Changed -= Refresh;
    }
}
