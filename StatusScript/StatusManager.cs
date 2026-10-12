using System.Collections.Generic;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    public static StatusManager Instance;

    public int level;
    public int hp;
    public int mp;
    public int exp;
    public float limitTime;
    public int reachedStage;
    public int tutorialCount;

    public List<Func> userFunctions = new List<Func>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetDefaultStatus()
    {
        level = 1;
        hp = 10;
        mp = 3;
        exp = 0;
        limitTime = 30f;
        reachedStage = 0;
        tutorialCount = 0;

        userFunctions = new List<Func>();
    }

    public void ApplyData(SaveData data)
    {
        level = data.level;
        hp = data.hp;
        mp = data.mp;
        exp = data.exp;
        limitTime = data.limitTime;
        reachedStage = data.reachedStage;
        tutorialCount = data.tutorialCount;

        userFunctions = new List<Func>(data.userFunctions);
    }

    public SaveData ToSaveData()
    {
        return new SaveData()
        {
            level = level,
            hp = hp,
            mp = mp,
            exp = exp,
            limitTime = limitTime,
            reachedStage = reachedStage,
            tutorialCount = tutorialCount,
            userFunctions = new List<Func>(userFunctions)
        };
    }
}

