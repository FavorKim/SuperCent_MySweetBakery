

using System;

public class MoneyModel
{
    private static MoneyModel instance;
    public static MoneyModel Instance
    {
        get 
        {
            if(instance == null)
                instance = new MoneyModel();
            return instance; 
        }
    }

    private int goldCount = 0;
    public event Action<int> OnPlusGold;
   
    public void PlusGold(int goldPlus)
    {
        goldCount += goldPlus;
        OnPlusGold?.Invoke(goldCount);
    }

    public void RefreshGold()
    {
        OnPlusGold?.Invoke(goldCount);
    }

    public void ClearOnPlusGold()
    {
        OnPlusGold = null;
    }
}
