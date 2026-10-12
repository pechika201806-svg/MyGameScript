using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Diagnostics.CodeAnalysis;
using Unity.Collections.LowLevel.Unsafe;


public class Enemy : MonoBehaviour
{
    public int hp;
    public int atp;
    public int type;
    public int i,j;
    public string enemyname;

    public int cursepoint;
    public bool downcheck = false;
    public bool shinigami = false;
    
    public float HPmax,HPBarMaxWidth;

    public bool BossCore;




    Color blueHP = new Color(0.2117647f, 0.4980392f, 0.6745098f);
    Color bossHP = new Color(0.6745098f, 0, 0.5209827f);
    Color yellowHP = new Color(0.89f, 0.82f, 0);
    Color redHP = new Color(0.66f, 0, 0.03f);

    TMP_Text messageText;
    TMP_Text HPText;
    Image img,HPBar, hpBarImage;


    void Awake()
    {
        Image[] imgs = GetComponentsInChildren<Image>();
        HPText = GetComponentInChildren<TextMeshProUGUI>();
        img = imgs[0];
        HPBar = imgs[1];
        hpBarImage = HPBar;

        HPmax = hp;
        HPBarMaxWidth = HPBar.rectTransform.sizeDelta.x;
    }

    void Start()
    {
        messageText = GameObject.Find("PlayerMessage").GetComponent<TMP_Text>();
    }

    void Update()
    {
        float ratio = (float)hp/HPmax;
        HPText.text = $"{hp}";
        HPBar.rectTransform.sizeDelta = new Vector2(HPBarMaxWidth * ratio, HPBar.rectTransform.sizeDelta.y);
        if (ratio <= 0.25f)
            hpBarImage.color = redHP;
        else if (ratio <= 0.5f)
            hpBarImage.color = yellowHP;
        else if (BossCore)
            hpBarImage.color = bossHP;
        else
            hpBarImage.color = blueHP;
    }
    public void TakeDamage(int dmg,int atty,int ati,int atj)
    {
        if(ati==i&&atj==j)
        {
            dmg=each(dmg,atty,type);
            hp -= dmg;
            Debug.Log($"{enemyname}({i},{j}) に {dmg} ダメージ！ 残りHP: {hp}");
            messageText.text += $"\n{enemyname}({i},{j}) に {dmg} ダメージ！ 残りHP: {hp}";

            if (hp <= 0)
            {
                Debug.Log($"{enemyname} は倒れた！");
                messageText.text += $"\n{enemyname}({i},{j}) は倒れた！";
                EnemyManager.AllEnemies.Remove(this);
                StartCoroutine(FadeAndDestroy());
            }
        }
    }

    public void TakeCurse(int dmg, int ati, int atj)
    {
        if (ati == i && atj == j)
        {
            cursepoint += dmg;
            Debug.Log($"{enemyname}({i},{j}) は {dmg} ダメージ分の呪いを受けた！\n現状呪いは{cursepoint}！");
            messageText.text += $"\n{enemyname}({i},{j}) に {dmg} ダメージ分の呪いを受けた！ \n累積呪いダメージ：{cursepoint}！";
        }
    }

    public void RunCurse()
    {
        if (cursepoint <= 0) return;

        hp -= cursepoint;
        Debug.Log($"{enemyname}({i},{j}) に {cursepoint} 呪いダメージ！ 残りHP: {hp}");
        messageText.text += $"\n{enemyname}({i},{j}) に {cursepoint} 呪いダメージ！ 残りHP: {hp}";

        if (hp <= 0)
        {
            Debug.Log($"{name} は倒れた！");
            messageText.text += $"\n{enemyname}({i},{j}) は倒れた！";
            EnemyManager.AllEnemies.Remove(this);
            StartCoroutine(FadeAndDestroy());
        }
    }

    public void Down(int ratepoint,int atty, int ati, int atj)
    {
        if (ati == i && atj == j)
        {
            if (hp <= 0)
            {
                downcheck = true;
                if (atty == 7)
                {
                    Debug.Log("目がくらんで");
                    messageText.text += "\n目がくらんで";
                }
                Debug.Log($"{enemyname}({i},{j}) は麻痺した！");
                messageText.text += $"\n{enemyname}({i},{j}) は麻痺した！";

                return;
            }
        
            float probability = (float)ratepoint / hp;

            if(downcheck)
            {
                Debug.Log($"{enemyname}({i},{j}) はすでに麻痺している");
                messageText.text += $"{enemyname}({i},{j}) はすでに麻痺している";
            }
            else if (Random.value < probability)
            {
                downcheck = true;
                if(atty == 7)
                {
                    Debug.Log("目がくらんで");
                    messageText.text += "\n目がくらんで";
                }
                if (atty == 8)
                {
                    Debug.Log("感電して");
                    messageText.text += "\n感電して";
                }
                Debug.Log($"{enemyname}({i},{j}) は麻痺した！");
                messageText.text += $"{enemyname}({i},{j}) は麻痺した！";
            }
            else
            {
                if (atty == 7)
                {
                    Debug.Log("目がくらまず");
                    messageText.text += "\n目がくらまず";
                }
                if (atty == 8)
                {
                    Debug.Log("感電せず");
                    messageText.text += "\n感電せず";
                }
                downcheck = false;
                Debug.Log($"{enemyname}({i},{j}) は麻痺しなかった！");
                messageText.text += $"{enemyname}({i},{j}) は麻痺しなかった！";
            }
        }
    }




    public IEnumerator FadeAndDestroy()
    {
        float t = 0f;
        float duration = 1.0f;

        Color c = img.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / duration);
            img.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
        if (BossCore && !EnemyManager.Instance.clear)
        {
            EnemyManager.Instance.clear = true;
            EnemyManager.Instance.StartCoroutine(EnemyManager.Instance.ClearSequence());
        }
        else
        {
            EnemyManager.Instance.OnEnemyDestroyed();
        }
    }


    int each(int dmg, int atty, int type)
    {
        if((atty==2&&type==3)||(atty==3&&type==5)||(atty==5&&type==2))
        {
            dmg=dmg*2;
            Debug.Log($"{enemyname}({i},{j})に効果抜群!!");
            messageText.text += $"\n{enemyname}({i},{j})に効果抜群!!";
        }
        if((atty==2&&type==5)||(atty==3&&type==2)||(atty==5&&type==3)) 
        {
            dmg=dmg/2;
            Debug.Log($"{enemyname}({i},{j})には相性が悪かったようだ...");
            messageText.text += $"\n{enemyname}({i},{j})には相性が悪かったようだ...";
        }
        return dmg;
    }

    public void AttackPlayer()
    {
        if (downcheck)
        {
            Debug.Log($"{enemyname}({i},{j})の敵は麻痺して攻撃ができない!!");
            messageText.text += $"\n{enemyname}({i},{j})の敵は麻痺して攻撃ができない!!";
            downcheck = false;
            return;
        }
        else
        {
            Debug.Log($"{enemyname}({i},{j})からの攻撃で{atp}のダメージ!!");
            messageText.text += $"\n{enemyname}({i},{j})からの攻撃で{atp}のダメージ!!";
            PlayerManager.Instance.ApplyDamage(atp);
            return;
        }
    }
    public IEnumerator Puun()
    {
        RectTransform rt = img.rectTransform;

        Vector3 baseScale = rt.localScale;
        Vector3 bigScale = baseScale * 1.18f;
        Vector3 overshoot = baseScale * 1.07f;

        float t = 0f;
        float upTime = 0.14f;
        while (t < upTime)
        {
            t += Time.deltaTime;
            rt.localScale = Vector3.Lerp(baseScale, bigScale, t / upTime);
            yield return null;
        }

        t = 0f;
        float downTime = 0.12f;
        while (t < downTime)
        {
            t += Time.deltaTime;
            rt.localScale = Vector3.Lerp(bigScale, overshoot, t / downTime);
            yield return null;
        }

        rt.localScale = baseScale;
    }

    public void EnemyHeal(int amount)
    {
        hp += amount;
        if (hp > HPmax) hp = (int)HPmax;
    }


    public IEnumerator MoveTo(int newI, int newJ, float duration = 0.3f)
    {
        i = newI;
        j = newJ;

        RectTransform rt = GetComponent<RectTransform>();
        Vector2 startPos = rt.anchoredPosition;
        Vector2 endPos = new Vector2(250f * j, -250f * i);

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            rt.anchoredPosition = Vector2.Lerp(startPos, endPos, t / duration);
            yield return null;
        }
        rt.anchoredPosition = endPos;
    }

    public void Death()
    {
        EnemyManager.AllEnemies.Remove(this);
        StartCoroutine(FadeAndDestroy());
    }

}

