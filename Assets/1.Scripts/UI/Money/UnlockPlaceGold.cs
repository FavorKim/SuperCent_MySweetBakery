

using System;
using System.Collections.Generic;

public class UnlockPlaceGold
{

    private static Dictionary<LockedPlace, UnlockPlaceGold> instances = new Dictionary<LockedPlace, UnlockPlaceGold>();
    private UnlockPlaceGold()
    {
        goldPaid = 0;
    }
    private int goldPaid;
    public int GoldPaid { get { return goldPaid; } }

    public event Action<int> OnSetGold;

    public void SetGold(int goldPaid)
    {
        this.goldPaid = goldPaid;
        OnSetGold?.Invoke(goldPaid);
    }

    public static UnlockPlaceGold GetInstance(LockedPlace owner)
    {
        if(instances.ContainsKey(owner) == false)
        {
            UnlockPlaceGold instance = new UnlockPlaceGold();
            instances.Add(owner, instance);
        }
        return instances[owner];
    }
}
