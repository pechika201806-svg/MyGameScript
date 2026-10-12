using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public int level;
    public int hp;
    public int mp;
    public int exp;
    public float limitTime;
    public int reachedStage;
    public int tutorialCount;

    public List<Func> userFunctions;
}


