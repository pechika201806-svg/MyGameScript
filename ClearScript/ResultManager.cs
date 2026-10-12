using Unity.VisualScripting;
using UnityEngine;
using System;

public class ResultManager : MonoBehaviour
{
    public static ResultManager Instance;
    public CurrentStage currentStage;
    public StageData stageData;
    public string stagename;
    public int clearEXP;
    public bool levelup = false;
    public int prelv,prehp,premp;
    public float prelimitTime;
    public int leftEXP;

    void Awake()
    {
        Instance = this;
        stageData = currentStage.stage;
        stagename = stageData.stageName;
        clearEXP = stageData.clearEXP;
        var s = StatusManager.Instance;
        prelv = s.level;
        s.exp += stageData.clearEXP;
        prehp = s.hp;
        premp = s.mp;
        prelimitTime = s.limitTime;

        while(s.exp >= 100 * s.level)
        {              
            levelup =true;
            s.exp -= 100 * s.level;
            s.level += 1;
            s.hp = CalcHP(s.level);
            s.mp = CalcMP(s.level);
            s.limitTime = CalcTime(s.level); ;
        }
        leftEXP = 100 * s.level - s.exp;
        if(s.reachedStage < stageData.stageID) s.reachedStage = stageData.stageID;
    }

    int CalcHP(int lv)
    {
        return Mathf.RoundToInt(0.2990299f * lv * lv + 9.7009701f);
    }
    int CalcMP(int lv)
    {
        return Mathf.RoundToInt(0.00970097f * lv * lv + 2.99029903f);
    }

    int CalcTime(int lv)
    {
        return Mathf.RoundToInt(20+(10*Mathf.Sqrt(lv)));
    }
}
