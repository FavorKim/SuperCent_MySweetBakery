

using System.Collections.Generic;

public class GameInfo
{
    private static GameInfo instance;
    public static GameInfo Instance
    {
        get
        {
            if(instance == null)
                instance = new GameInfo();

            return instance;
        }
    }

    public GameInfo()
    {
        havePointedList.Add(false);
        havePointedList.Add(false);
    }

    private List<bool> havePointedList = new List<bool>();

    public bool IsTutorialEnd
    {
        get;
        private set;
    }

    public void EndTutorial()
    {
        IsTutorialEnd = true;
    }

    public void Point(int index)
    {
        havePointedList[index-1] = true;
    }
    public bool HavePointed(int index)
    {
        return havePointedList[index-1];
    }
}
