using UnityEngine;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public TMP_Text MessageText;

    public int hp,hpmax;
    public int mp,mpmax;
    public float time;
    public bool gameover = false;

    public bool mirage = false;

    public bool dopinged = false;

    void Awake()
    {
        Instance = this;
        var s = StatusManager.Instance;
        hp = s.hp;
        mp = s.mp;
        hpmax = s.hp;
        mpmax = s.mp;
        time = s.limitTime;
    }


    public void Heal(int amount)
    {
        hp += amount;
        if(hp > hpmax) hp = hpmax;
    }

    public void MPHeal(int amount)
    {
        mp += amount;
        if (mp > mpmax) mp = mpmax;
    }

    public void OverDose()
    {
        mp = 0;
    }

    public void Mirage()
    {
        mirage = true;
    }

    public　void Berserk()
    {
        hp -= 30;
        if (hp <= 0)
        {
            hp = 0;
            GameOver();
        }
        mp += 1;
        if (mpmax < mp) mpmax = mp;
    }

    public void Doping()
    {
        hpmax *= 2;
        mpmax *= 2;
        hp *= 2;
        mp *= 2;
        dopinged = true;
    }

    public void OverDoping()
    {
        hp /= 2;
        mp /= 2;
        hpmax /= 2;
        mpmax /= 2;
        if (hp <= 0)
        {
            hp = 0;
            GameOver();
        }
    }

    public void Backlash()
    {
        hp /= 4;
        mp /= 4;
        hpmax /= 4;
        mpmax /= 4;
        dopinged = false;
        Debug.Log("ドーピングの反動によりHP,MP減少!!");
        MessageText.text += "\nドーピングの反動によりHP,MP減少!!";
        if (hp <= 0)
        {
            hp = 0;
            GameOver();
        }
    }


    public void ApplyDamage(int dmg)
    {
        bool hit = true;
        if(mirage)
        {
            hit = Random.value >= 1f / 3f;
        }

        if (hit)
        {
            hp -= dmg;

            if (hp <= 0)
            {
                hp = 0;
                GameOver();
            }
        }
        else
        {
            Debug.Log("蜃気楼によって攻撃が当たらなかった");
            MessageText.text += "\n蜃気楼によって攻撃が当たらなかった";
        }
    }

    public void GameOver()
    {
        if (!gameover)
        {
            StartCoroutine(GameOverSequence());
        }
    }
    IEnumerator GameOverSequence()
    {
        gameover = true;
        yield return new WaitForSeconds(0.5f);
        Debug.Log("=== GAME OVER ===");
        MessageText.text = "\n\n========================= GAME OVER =========================";
        FadeBlack.Instance.FadeOut();
    }
}

