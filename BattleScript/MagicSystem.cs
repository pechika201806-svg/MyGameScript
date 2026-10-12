using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class MagicSystem : MonoBehaviour
{
    public static MagicSystem Instance;
    public TMP_Text MessageText;
    public Queue<IEnumerator> magicQueue = new Queue<IEnumerator>();
    public RectTransform canvasTransform;
    public GameObject MagicEffectPrefab_UI;
    public Sprite[] MagicSpritesArray;
    Dictionary<int, Sprite> MagicSprites;

    bool isRunning = false;
    public bool IsRunning => isRunning;
    public float waitTime = 0.5f;

    public int shinigami = 0;


    void Awake()
    {
        Instance = this;

        MagicSprites = new Dictionary<int, Sprite>();
        for (int i = 0; i < MagicSpritesArray.Length; i++)
        {
            MagicSprites[i + 1] = MagicSpritesArray[i];
        }
    }

    public void TryRunQueue()
    {
        if (!isRunning && magicQueue.Count > 0)
        {
            StartCoroutine(RunMagicQueue());
        }
    }

    public IEnumerator RunMagicQueue()
    {
        isRunning = true;

        while (magicQueue.Count > 0)
        {
            if (shinigami == 1) shinigami++;
            var routine = magicQueue.Dequeue();
            yield return StartCoroutine(routine);
            if (shinigami != 1)
            {
                shinigami = 0;
                EnemyManager.Instance.RemoveShinigami();
            }
        }

        isRunning = false;
    }

    

    public IEnumerator MagicMessage(string msg, int magicId , int i, int j)
    {
        yield return new WaitForSeconds(0.3f);
        MessageText.text += "\n" + msg;
        if (magicId != 0)
        {
            yield return ShowMagicImage(magicId, i, j);
        }
        else
        {
            yield return new WaitForSeconds(waitTime);
        }
    }
    
    public IEnumerator ShowMagicImage(int magicId, int i, int j)
    {
        if (i < 0 || i > 2 || j < 0 || j > 4)
        {
            yield return new WaitForSeconds(waitTime);
            yield break;
        }
        
   
        var fx = Instantiate(MagicEffectPrefab_UI, canvasTransform);

        fx.GetComponent<Image>().sprite = MagicSprites[magicId];

        RectTransform rt = fx.GetComponent<RectTransform>();

        float x = 250 * j;
        float y = -250 * i;

        rt.anchoredPosition = new Vector2(x, y);

        yield return new WaitForSeconds(waitTime);

        Destroy(fx);
    }



    

    public bool MPjudge(int cost)
    {
        if(PlayerManager.Instance.mp < cost)
        {
            Debug.Log("MP不足");
            MessageText.text += "\nMP不足";
            return false;
        }
        
        PlayerManager.Instance.mp -= cost;
        return true;
    }

    public void Magic(int id)
    {
        magicQueue.Enqueue(MagicRoutine(id));
        TryRunQueue();
    }

    IEnumerator MagicRoutine(int id)
    {
        if(!MPjudge(1)) yield break;

        switch (id)
        {
            case 1:
                yield return Attack(1,2);
                break;

            case 2:
                yield return Water(1,2);
                break;

            case 3:
                yield return Fire(1,2);
                break;

            case 4:
                yield return HealPlayer(10);
                break;

            case 5:
                yield return Wind(1,2);
                break;

            default:
                Debug.Log("Unknown magic id: " + id);
                MessageText.text += "\nUnknown magic id: " + id;
                break;
        }
    }

    public void Shot(int id, int i, int j)
    {
        magicQueue.Enqueue(shotRoutine(id, i, j));
        TryRunQueue();
    }

    IEnumerator shotRoutine(int id,int i,int j)
    {
        if(!MPjudge(1)) yield break;
        
        switch (id)
        {
            case 1:
                yield return Attack(i,j);
                break;

            case 2:
                yield return Water(i,j);
                break;

            case 3:
                yield return Fire(i,j);
                break;

            case 4:
                yield return HealPlayer(10);
                break;

            case 5:
                yield return Wind(i,j);
                break;

            default:
                Debug.Log("Unknown magic id: " + id);
                MessageText.text += "\nUnknown magic id: " + id;
                break;
        }
    }

    public void Exmagic(int id, int i, int j)
    {
        magicQueue.Enqueue(exmagicRoutine(id, i, j));
        TryRunQueue();
    }

    IEnumerator exmagicRoutine(int id, int i, int j)
    {
        if (!MPjudge(2)) yield break;

        switch (id)
        {
            case 1:
                yield return ExAttack(i, j);
                break;

            case 2:
                yield return ExWater(i, j);
                break;

            case 3:
                yield return ExFire(i, j);
                break;

            case 4:
                yield return ExHealPlayer(50);
                break;

            case 5:
                yield return ExWind(i, j);
                break;

            case 6:
                yield return Dark(i, j);
                break;

            case 7:
                yield return Shine(i, j);
                break;


            default:
                Debug.Log("Unknown magic id: " + id);
                MessageText.text += "\nUnknown magic id: " + id;
                break;
        }
    }

    public int Scope(int i, int j)
    {
        var target = EnemyManager.GetEnemyAt(i, j);
        if (target == null)
        {
            Debug.Log($"ミス({i},{j})");
            magicQueue.Enqueue(MagicMessage($"ミス({i},{j})", 4, i, j));
            TryRunQueue();
            return 0;
        }

        Debug.Log($"({i},{j}) : 属性{target.type}");
        magicQueue.Enqueue(MagicMessage($"({i},{j}) : 属性{target.type}", 4, i, j));
        TryRunQueue();
        return target.type;
    }



    IEnumerator Attack(int i, int j)
    {
        var target = EnemyManager.GetEnemyAt(i, j);
        if (target == null)
        {
            Debug.Log($"ミス({i},{j})");
            yield return MagicMessage($"ミス({i},{j})",1,i,j);
            yield break;
        }
        Debug.Log("無属性攻撃!!");
        yield return MagicMessage("無属性攻撃!!",1,i,j);        
        target.TakeDamage(10, 1, i, j);
    }


    IEnumerator Water(int i,int j)
    {
        var target = EnemyManager.GetEnemyAt(i, j);
        if (target == null)
        {
            Debug.Log($"ミス({i},{j})");
            yield return MagicMessage($"ミス({i},{j})",2,i,j);
            yield break;
        }
        Debug.Log("水属性攻撃!!");
        yield return MagicMessage("水属性攻撃!!",2,i,j);
        target.TakeDamage(10, 2, i, j);
    }
    IEnumerator Fire(int i,int j)
    {
        var target = EnemyManager.GetEnemyAt(i, j);
        if (target == null)
        {
            Debug.Log($"ミス({i},{j})");
            yield return MagicMessage($"ミス({i},{j})",3,i,j);
            yield break;
        }
        Debug.Log("火属性攻撃!!");
        yield return MagicMessage("火属性攻撃!!",3,i,j);
        target.TakeDamage(10, 3, i, j);
    }

    IEnumerator HealPlayer(int uppoint)
    {
        PlayerManager.Instance.Heal(uppoint);
        Debug.Log($"HealPlayer 発動!! プレイヤーを{uppoint}回復");
        yield return MagicMessage($"回復魔法発動!! プレイヤーを{uppoint}回復",0,100,100);
    }

    IEnumerator Wind(int i,int j)
    {
        var target = EnemyManager.GetEnemyAt(i, j);
        if (target == null)
        {
            Debug.Log($"ミス({i},{j})");
            yield return MagicMessage($"ミス({i},{j})",5,i,j);
            yield break;
        }
        Debug.Log("風属性攻撃!!");
        yield return MagicMessage("風属性攻撃!!",5,i,j);
        target.TakeDamage(10, 5, i, j);
    }


    public IEnumerator ExAttack(int i, int j)
    {
        var target = EnemyManager.GetEnemyAt(i, j);
        if (target == null)
        {
            Debug.Log($"ミス({i},{j})");
            yield return MagicMessage($"ミス({i},{j})", 1, i, j);
            yield break;
        }
        Debug.Log("無属性強攻撃!!");
        yield return MagicMessage("無属性強攻撃!!", 1, i, j);
        target.TakeDamage(30, 1, i, j);
    }


    public IEnumerator ExWater(int i, int j)
    {
        var target = EnemyManager.GetEnemyAt(i, j);
        if (target == null)
        {
            Debug.Log($"ミス({i},{j})");
            yield return MagicMessage($"ミス({i},{j})", 2, i, j);
            yield break;
        }
        Debug.Log("水属性強攻撃!!");
        yield return MagicMessage("水属性強攻撃!!", 2, i, j);
        target.TakeDamage(30, 2, i, j);
    }
    public IEnumerator ExFire(int i, int j)
    {
        var target = EnemyManager.GetEnemyAt(i, j);
        if (target == null)
        {
            Debug.Log($"ミス({i},{j})");
            yield return MagicMessage($"ミス({i},{j})", 3, i, j);
            yield break;
        }
        Debug.Log("火属性強攻撃!!");
        yield return MagicMessage("火属性強攻撃!!", 3, i, j);
        target.TakeDamage(30, 3, i, j);
    }

    public IEnumerator ExHealPlayer(int uppoint)
    {
        PlayerManager.Instance.Heal(uppoint);
        Debug.Log($"中回復魔法発動!! プレイヤーを{uppoint}回復");
        yield return MagicMessage($"中回復魔法発動!! プレイヤーを{uppoint}回復", 0, 100, 100);
    }

    public IEnumerator ExWind(int i, int j)
    {
        var target = EnemyManager.GetEnemyAt(i, j);
        if (target == null)
        {
            Debug.Log($"ミス({i},{j})");
            yield return MagicMessage($"ミス({i},{j})", 5, i, j);
            yield break;
        }
        Debug.Log("風属性強攻撃!!");
        yield return MagicMessage("風属性強攻撃!!", 5, i, j);
        target.TakeDamage(30, 5, i, j);
    }

    public IEnumerator Dark(int i, int j)
    {
        var target = EnemyManager.GetEnemyAt(i, j);
        if (target == null)
        {
            Debug.Log($"ミス({i},{j})");
            yield return MagicMessage($"ミス({i},{j})", 6, i, j);
            yield break;
        }
        Debug.Log("闇属性攻撃");
        yield return MagicMessage("闇属性攻撃!!", 6, i, j);
        target.TakeCurse(20, i, j);
    }

    public IEnumerator Shine(int i, int j)
    {
        var target = EnemyManager.GetEnemyAt(i, j);
        if (target == null)
        {
            Debug.Log($"ミス({i},{j})");
            yield return MagicMessage($"ミス({i},{j})", 7, i, j);
            yield break;
        }
        Debug.Log("光属性攻撃");
        yield return MagicMessage("光属性攻撃!!", 7, i, j);
        target.Down(77, 7, i, j);
    }

}
