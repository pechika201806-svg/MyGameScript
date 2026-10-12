using NUnit.Framework.Internal.Commands;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.UI;


public class EnemyManager : MonoBehaviour
{
    public Transform enemiesRoot;
    public static List<Enemy> AllEnemies = new List<Enemy>();
    public static EnemyManager Instance;
    public TMP_Text MessageText;
    public Button turnButton;

    public bool clear = false;
    public Image curseOverlayImg;
    public Image SuperFireImg;
    public Image ARASHIImg;
    public Image ExplosionImg;
    public Image TyphoonImg;
    public Image LaserImg;
    public Image FellowImg;
    public Image CosmicImg;
    public Image SunImg;
    public int BossI, BossJ;



    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        if (curseOverlayImg != null)
        {
            Color c = curseOverlayImg.color;
            curseOverlayImg.color = new Color(c.r, c.g, c.b, 0f);
        }
        if (SuperFireImg != null)
        {
            Color c = SuperFireImg.color;
            SuperFireImg.color = new Color(c.r, c.g, c.b, 0f);
        }
        if (ARASHIImg != null)
        {
            Color c = ARASHIImg.color;
            ARASHIImg.color = new Color(c.r, c.g, c.b, 0f);
        }
        if (ExplosionImg != null)
        {
            Color c = ExplosionImg.color;
            ExplosionImg.color = new Color(c.r, c.g, c.b, 0f);
        }
        if (TyphoonImg != null)
        {
            Color c = TyphoonImg.color;
            TyphoonImg.color = new Color(c.r, c.g, c.b, 0f);
        }
        if (LaserImg != null)
        {
            Color c = LaserImg.color;
            LaserImg.color = new Color(c.r, c.g, c.b, 0f);
        }
        if (FellowImg != null)
        {
            Color c = FellowImg.color;
            FellowImg.color = new Color(c.r, c.g, c.b, 0f);
        }
        if (CosmicImg != null)
        {
            Color c = CosmicImg.color;
            CosmicImg.color = new Color(c.r, c.g, c.b, 0f);
        }
        if (SunImg != null)
        {
            Color c = SunImg.color;
            SunImg.color = new Color(c.r, c.g, c.b, 0f);
        }
    }


    public void SpawnEnemies()
    {
        AllEnemies.Clear();

        if (enemiesRoot == null)
        {
            Debug.LogError("EnemyManager: enemiesRoot が設定されていません！");
            return;
        }

        foreach (Transform child in enemiesRoot)
        {
            Enemy e = child.GetComponent<Enemy>();
            if (e != null)
            {
                AllEnemies.Add(e);
                Debug.Log("Enemy 登録: " + child.name);

                RectTransform rt = e.GetComponent<RectTransform>();
                float x = 250f * e.j;
                float y = -250f * e.i;
                rt.anchoredPosition = new Vector2(x, y);
            }
        }

        Debug.Log($"EnemyManager: 敵を {AllEnemies.Count} 体登録しました");
    }

    public void DoEnemyActions()
    {
        StartCoroutine(EnemyActionsWithCurse());
    }

    IEnumerator EnemyActionsWithCurse()
    {
        yield return StartCoroutine(EnemyActionSequence());

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(EnemyMovementPhase());

        yield return new WaitForSeconds(0.5f);

        bool anyCursed = AllEnemies.Any(e => e.cursepoint > 0);
        if (anyCursed && curseOverlayImg != null)
            yield return StartCoroutine(OverlayFlash(curseOverlayImg));

        foreach (var e in AllEnemies.ToList())
        {
            e.RunCurse();
        }

        OnEnemyDestroyed();
        yield return new WaitForSeconds(0.2f);
        if (PlayerManager.Instance.dopinged)
        {
            PlayerManager.Instance.Backlash();
        }


        turnButton.interactable = true;

    }

    IEnumerator EnemyMovementPhase()
    {
        bool Boss = false;
        foreach (var e in AllEnemies.ToList())
        {
            if (e.BossCore == true) Boss = true; 
        }

        if (Boss)
            yield return StartCoroutine(BossMovementPhase());
        else
            yield return StartCoroutine(NormalMovementPhase());
    }

    IEnumerator BossMovementPhase()
    {
        BossI = Random.Range(-1, 4);
        BossJ = Random.Range(-1, 6);
        List<Coroutine> moves = new List<Coroutine>();
        foreach (var e in AllEnemies.ToList())
        {
            if (e.hp <= 0) continue;
            var mover = e.GetComponent<EnemyMover>();
            if (mover != null)
                moves.Add(StartCoroutine(mover.Move()));
        }
        foreach (var c in moves)
            yield return c;
    }

    IEnumerator NormalMovementPhase()
    {
        foreach (var e in AllEnemies.ToList())
        {
            if (e.hp <= 0) continue;
            var mover = e.GetComponent<EnemyMover>();
            if (mover != null)
                yield return StartCoroutine(mover.Move());
        }
    }

    IEnumerator OverlayFlash(Image img)
    {
        Color c = img.color;
        float t = 0f;
        float inTime = 0.15f;
        float holdTime = 0.2f;
        float outTime = 0.35f;

        while (t < inTime)
        {
            t += Time.deltaTime;
            img.color = new Color(c.r, c.g, c.b, Mathf.Lerp(0f, 1f, t / inTime));
            yield return null;
        }
        img.color = new Color(c.r, c.g, c.b, 1f);

        yield return new WaitForSeconds(holdTime);

        t = 0f;
        while (t < outTime)
        {
            t += Time.deltaTime;
            img.color = new Color(c.r, c.g, c.b, Mathf.Lerp(1f, 0f, t / outTime));
            yield return null;
        }
        img.color = new Color(c.r, c.g, c.b, 0f);
    }

    IEnumerator OverlayLaserFlash(int i, Image img)
    {
        RectTransform rt = img.rectTransform;
        Vector2 pos = rt.anchoredPosition;
        pos.y = i * (-250);
        rt.anchoredPosition = pos;

        Color c = img.color;
        float t = 0f;
        float inTime = 0.15f;
        float holdTime = 0.2f;
        float outTime = 0.35f;

        while (t < inTime)
        {
            t += Time.deltaTime;
            img.color = new Color(c.r, c.g, c.b, Mathf.Lerp(0f, 1f, t / inTime));
            yield return null;
        }
        img.color = new Color(c.r, c.g, c.b, 1f);

        yield return new WaitForSeconds(holdTime);

        t = 0f;
        while (t < outTime)
        {
            t += Time.deltaTime;
            img.color = new Color(c.r, c.g, c.b, Mathf.Lerp(1f, 0f, t / outTime));
            yield return null;
        }
        img.color = new Color(c.r, c.g, c.b, 0f);
    }
    IEnumerator EnemyActionSequence()
    {
        foreach (var e in AllEnemies.ToList())
        {
            if (e.hp > 0)
            {
                yield return StartCoroutine(e.Puun());

                e.AttackPlayer();

                yield return new WaitForSeconds(0.1f);
            }

        }
    }

    

    public IEnumerator SuperFire()
    {
        yield return StartCoroutine(OverlayFlash(SuperFireImg));
        foreach (var e in AllEnemies.ToList())
        {
            if (e.type == 2)
            {
                Debug.Log($"{e.enemyname}({e.i},{e.j}) は煉獄で蒸発した");
                MessageText.text += $"\n{e.enemyname}({e.i},{e.j}) は煉獄で蒸発した";
                e.Death();
            }
            if (e.type == 3)
            {
                Debug.Log($"{e.enemyname}({e.i},{e.j}) は煉獄で全回復した");
                MessageText.text += $"\n{e.enemyname}({e.i},{e.j}) は煉獄で全回復した";
                e.hp = (int)e.HPmax;
            }
        }
        OnEnemyDestroyed();
    }

    public IEnumerator ARASHI()
    {
        yield return StartCoroutine(OverlayFlash(ARASHIImg));
        foreach (var e in AllEnemies.ToList())
        {
            int dmg = Random.Range(0,31);
            e.TakeDamage(dmg, 5, e.i, e.j);
        }
        OnEnemyDestroyed();
    }

    public IEnumerator Explosion()
    {
        yield return StartCoroutine(OverlayFlash(ExplosionImg));
        foreach (var e in AllEnemies.ToList())
        {
            int distance = Mathf.Abs(e.j - 2);
            if(Mathf.Abs(e.i - 1) > distance)
            {
                distance = Mathf.Abs(e.i - 1);
            }

            int dmg = 30 - distance * 10;
            if (dmg < 0) dmg = 0;
            e.TakeDamage(dmg, 3, e.i, e.j);
        }
        OnEnemyDestroyed();
    }

    public IEnumerator Typhoon()
    {
        yield return StartCoroutine(OverlayFlash(TyphoonImg));
        foreach (var e in AllEnemies.ToList())
        {
            int distance = Mathf.Abs(e.j - 2);
            if (Mathf.Abs(e.i - 1) > distance)
            {
                distance = Mathf.Abs(e.i - 1);
            }

            int dmg = 10 + distance * 10;
            if (dmg < 0) dmg = 0;
            e.TakeDamage(dmg, 5, e.i, e.j);
        }
        OnEnemyDestroyed();
    }

    public void AllEnemyHeal(int uppoint)
    {
        foreach (var e in AllEnemies.ToList())
        {
            e.EnemyHeal(uppoint);
        }
    }

    public IEnumerator CursedCold()
    {
        foreach (var e in AllEnemies.ToList())
        {
            if (e.cursepoint > 0)
            {
                e.cursepoint = (int)(1.2f * e.cursepoint);
                yield return MagicSystem.Instance.ShowMagicImage(9, e.i, e.j);
            }
        }
    }

    public void TakeShinigami()
    {
        foreach (var e in AllEnemies.ToList())
        {
            e.shinigami = true;
        }
    }

    public void RemoveShinigami()
    {
        foreach (var e in AllEnemies.ToList())
        {
            e.shinigami = false;
        }
    }

    public void ShinigamiRun()
    {
        var target = GetRandomEnemy();
        if (target == null||target.shinigami == false) return;

        Debug.Log("ふっ");
        MessageText.text += "\nふっ";
        target.Death();
    }

    public IEnumerator Laser()
    {
        int targetColumn = Random.Range(0, 3);

        var targets = EnemyManager.AllEnemies.Where(e => e.i == targetColumn).ToList();
        yield return StartCoroutine(OverlayLaserFlash(targetColumn,LaserImg));
        foreach (var e in targets)
        {
            e.TakeDamage(100, 1, e.i, e.j);
        }
        OnEnemyDestroyed();
    }

    public IEnumerator AllTakeDamage(int dmg)
    {
        yield return StartCoroutine(OverlayFlash(FellowImg));
        foreach (var e in AllEnemies.ToList())
        {
            e.TakeDamage(dmg, 1, e.i, e.j);
        }
        OnEnemyDestroyed();
    }

    public IEnumerator CosmicForces()
    {
        yield return StartCoroutine(OverlayFlash(CosmicImg));
        foreach (var e in AllEnemies.ToList())
        {
            e.TakeDamage(e.hp/2, 1, e.i, e.j);
        }
        OnEnemyDestroyed();
    }

    public IEnumerator TheSun()
    {
        yield return StartCoroutine(OverlayFlash(SunImg));
        foreach (var e in AllEnemies.ToList())
        {
            e.TakeDamage(33333, 1, e.i, e.j);
        }
        OnEnemyDestroyed();
    }


    public static Enemy GetRandomEnemy()
    {
        if (AllEnemies.Count == 0)
        {
            Debug.LogWarning("EnemyManager: 敵がいません！");
            return null;
        }

        return AllEnemies[Random.Range(0, AllEnemies.Count)];
    }

    public static Enemy GetEnemyAt(int i, int j)
    {
        foreach (var e in AllEnemies)
        {
            if (e.i == i && e.j == j)
                return e;
        }
        return null;
    }



    public void OnEnemyDestroyed()
    {
        if (clear) return;

        if (AllEnemies.Count == 0)
        {
            StartCoroutine(ClearSequence());
        }
    }
    public IEnumerator ClearSequence()
    {
        yield return new WaitForSeconds(0.5f);
        Debug.Log("=== CLEAR ===");
        MessageText.text = "\n\n=========================== CLEAR ===========================";
        clear = true;
        FadeBlack.Instance.FadeOut();
        yield return null;
    }
}

