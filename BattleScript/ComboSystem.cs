using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class ComboSystem : MonoBehaviour
{
    public static ComboSystem Instance;
    public bool MPCount = false;
    public TMP_Text MessageText;
    void Awake()
    {
        Instance = this;
    }


    public void Combo(int id1, int id2)
    {
        MagicSystem.Instance.magicQueue.Enqueue(ComboRoutine(id1,id2));
        MagicSystem.Instance.TryRunQueue();
    }

    IEnumerator ComboRoutine(int id1,int id2)
    {

        int id = id1 * id2;

        switch (id)
        {
            case 1:
                if (!MagicSystem.Instance.MPjudge(2)) yield break;
                yield return MagicSystem.Instance.ExAttack(1,2);
                break;

            case 2:
                if (!MagicSystem.Instance.MPjudge(2)) yield break;
                yield return MagicSystem.Instance.ExWater(1,2);
                break;

            case 3:
                if (!MagicSystem.Instance.MPjudge(2)) yield break;
                yield return MagicSystem.Instance.ExFire(1,2);
                break;

            case 4:
                if (!MagicSystem.Instance.MPjudge(2)) yield break;
                yield return MagicSystem.Instance.ExHealPlayer(50);
                break;

            case 5:
                if (!MagicSystem.Instance.MPjudge(2)) yield break;
                yield return MagicSystem.Instance.ExWind(1,2);
                break;

            case 6:
                if (!MagicSystem.Instance.MPjudge(2)) yield break;
                yield return MagicSystem.Instance.Dark(1,2);
                break;

            case 7:
                if (!MagicSystem.Instance.MPjudge(2)) yield break;
                yield return MagicSystem.Instance.Shine(1,2);
                break;

            case 8:
                if (!MagicSystem.Instance.MPjudge(6)) yield break;
                yield return BigHeal(200);
                break;

            case 9:
                if (!MagicSystem.Instance.MPjudge(30)) yield break;
                yield return SuperFire();
                break;

            case 10:
                if (!MagicSystem.Instance.MPjudge(10)) yield break;
                yield return ARASHI();
                break;

            case 12:
                yield return MPHeal(10);
                break;

            case 14:
                if (!MagicSystem.Instance.MPjudge(10)) yield break;
                yield return Mirage();
                break;

            case 15:
                if (!MagicSystem.Instance.MPjudge(10)) yield break;
                yield return Explosion();
                break;

            case 16:
                if (!MagicSystem.Instance.MPjudge(14)) yield break;
                yield return SuperHeal(500);
                break;

            case 18:
                yield return Berserk();
                break;

            case 20:
                if (!MagicSystem.Instance.MPjudge(1)) yield break;
                yield return AutoHeal();
                break;

            case 21:
                if (!MagicSystem.Instance.MPjudge(10)) yield break;
                yield return Lightning();
                break;

            case 24:
                if (!MagicSystem.Instance.MPjudge(10)) yield break;
                yield return Doping();
                break;

            case 25:
                if (!MagicSystem.Instance.MPjudge(10)) yield break;
                yield return Typhoon();
                break;

            case 28:
                if (!MagicSystem.Instance.MPjudge(2)) yield break;
                yield return Pray(200);
                break;

            case 30:
                if (!MagicSystem.Instance.MPjudge(10)) yield break;
                yield return CursedCold();
                break;

            case 35:
                if (!MagicSystem.Instance.MPjudge(20)) yield break;
                yield return Laser();
                break;

            case 36:
                yield return Fellow();
                break;

            case 42:
                if (!MagicSystem.Instance.MPjudge(99)) yield break;
                yield return CosmicForces();
                break;

            case 49:
                if (!MagicSystem.Instance.MPjudge(333)) yield break;
                yield return TheSun();
                break;


            default:
                Debug.Log("Unknown magic id: " + id);
                MessageText.text += "\nUnknown magic id: " + id;
                break;
        }
    }

    public IEnumerator BigHeal(int uppoint)
    {
        PlayerManager.Instance.Heal(uppoint);
        Debug.Log($"中回復魔法発動!! プレイヤーを{uppoint}回復");
        yield return MagicSystem.Instance.MagicMessage($"中回復魔法発動!! プレイヤーを{uppoint}回復", 0, 100, 100);
    }

    public IEnumerator SuperFire()
    {
        Debug.Log("煉獄発動!!");
        yield return new WaitForSeconds(0.3f);
        MessageText.text += "\n煉獄発動!!";
        yield return EnemyManager.Instance.SuperFire();
        yield return new WaitForSeconds(MagicSystem.Instance.waitTime);
    }

    public IEnumerator ARASHI()
    {
        Debug.Log("嵐魔法発動!!");
        yield return new WaitForSeconds(0.3f);
        MessageText.text += "\n嵐魔法発動!!";
        yield return EnemyManager.Instance.ARASHI();
        yield return new WaitForSeconds(MagicSystem.Instance.waitTime);
    }

    public IEnumerator MPHeal(int uppoint)
    {
        if (!MPCount)
        {
            
            Debug.Log($"魔薬草発動!! プレイヤーを{uppoint}回復");
            yield return MagicSystem.Instance.MagicMessage($"魔薬草発動!! プレイヤーのMPを{uppoint}回復", 0, 100, 100);
            PlayerManager.Instance.MPHeal(uppoint);
            MPCount = true;

        }
        else
        {
            Debug.Log($"オーバードーズ!!MP0!!");
            yield return MagicSystem.Instance.MagicMessage($"オーバードーズ!!MP0", 0, 100, 100);
            PlayerManager.Instance.OverDose();
        }
    }

    public IEnumerator Mirage()
    {
        Debug.Log("蜃気楼発動!!");
        yield return new WaitForSeconds(0.3f);
        MessageText.text += "\n蜃気楼発動!!";
        PlayerManager.Instance.Mirage();
        yield return new WaitForSeconds(MagicSystem.Instance.waitTime);
    }

    public IEnumerator Explosion()
    {
        Debug.Log("爆裂魔法発動!!");
        yield return new WaitForSeconds(0.3f);
        MessageText.text += "\n爆裂魔法発動!!";
        yield return EnemyManager.Instance.Explosion();
        yield return new WaitForSeconds(MagicSystem.Instance.waitTime);
    }

    public IEnumerator SuperHeal(int uppoint)
    {
        PlayerManager.Instance.Heal(uppoint);
        Debug.Log($"大回復魔法発動!! プレイヤーを{uppoint}回復");
        yield return MagicSystem.Instance.MagicMessage($"大回復魔法発動!! プレイヤーを{uppoint}回復", 0, 100, 100);
    }

    public IEnumerator Berserk()
    {
        MagicSystem.Instance.shinigami = 1;
        EnemyManager.Instance.TakeShinigami();
        Debug.Log("バーサーク発動!!");
        yield return MagicSystem.Instance.MagicMessage("バーサーク発動!!", 0, 100, 100);
        PlayerManager.Instance.Berserk();
    }

    public IEnumerator AutoHeal()
    {
        TurnManager.Instance.AutoHealCount++;
        Debug.Log("自動回復魔法発動!!");
        yield return MagicSystem.Instance.MagicMessage("自動回復魔法発動!!", 0, 100, 100);
    }

    public IEnumerator Lightning()
    {
        for (int k = 0; k < 3; k++)
        {
            var target = EnemyManager.GetRandomEnemy();
            if (target == null) yield break;

            Debug.Log("雷属性攻撃!!");
            yield return MagicSystem.Instance.MagicMessage("雷属性攻撃!!", 8, target.i, target.j);
            target.TakeDamage(30, 1, target.i, target.j);
            target.Down(30, 8, target.i, target.j);
        }
    }

    public IEnumerator Doping()
    {
        if (!PlayerManager.Instance.dopinged)
        {
            MagicSystem.Instance.shinigami = 1;
            Debug.Log("ドーピング発動!!");
            yield return MagicSystem.Instance.MagicMessage("ドーピング発動!!", 0, 100, 100);
            PlayerManager.Instance.Doping();
        }
        else
        {
            MagicSystem.Instance.shinigami = 1;
            Debug.Log("過剰ドーピング!!");
            yield return MagicSystem.Instance.MagicMessage("過剰ドーピング!!", 0, 100, 100);
            PlayerManager.Instance.OverDoping();
        }
    }

    public IEnumerator Typhoon()
    {
        Debug.Log("台風魔法発動!!");
        yield return new WaitForSeconds(0.3f);
        MessageText.text += "\n台風魔法発動!!";
        yield return EnemyManager.Instance.Typhoon();
        yield return new WaitForSeconds(MagicSystem.Instance.waitTime);
    }

    public IEnumerator Pray(int uppoint)
    {
        PlayerManager.Instance.Heal(uppoint);
        EnemyManager.Instance.AllEnemyHeal(uppoint);
        Debug.Log($"祈り発動!! プレイヤーと敵どちらも{uppoint}回復");
        yield return MagicSystem.Instance.MagicMessage($"祈り発動!! プレイヤーと敵どちらも{uppoint}回復", 0, 100, 100);
    }

    public IEnumerator CursedCold()
    {
        if(MagicSystem.Instance.shinigami==2)
        {
            EnemyManager.Instance.ShinigamiRun();
        }
        yield return EnemyManager.Instance.CursedCold();
        Debug.Log($"呪い風邪発動!!");
        yield return MagicSystem.Instance.MagicMessage($"呪い風邪発動!!", 0, 100, 100);
    }

    public IEnumerator Laser()
    {
        Debug.Log("レーザー魔法発動!!");
        yield return new WaitForSeconds(0.3f);
        MessageText.text += "\nレーザー魔法発動!!";
        yield return EnemyManager.Instance.Laser();
        yield return new WaitForSeconds(MagicSystem.Instance.waitTime);
    }

    public IEnumerator Fellow()
    {
        PlayerManager.Instance.ApplyDamage(1000);
        Debug.Log($"道ずれ発動!!\nプレイヤーに1000ダメージ");
        yield return MagicSystem.Instance.MagicMessage($"道ずれ発動!!\nプレイヤーに1000ダメージ!!", 0, 100, 100);
        yield return EnemyManager.Instance.AllTakeDamage(1000);
        Debug.Log($"\n敵全体に1000ダメージ");
        yield return MagicSystem.Instance.MagicMessage($"敵全体に1000ダメージ!!", 0, 100, 100);
    }

    public IEnumerator CosmicForces()
    {
        Debug.Log("陰陽魔法発動!!");
        yield return new WaitForSeconds(0.3f);
        MessageText.text += "\n陰陽魔法発動!!";
        yield return EnemyManager.Instance.CosmicForces();
        yield return new WaitForSeconds(MagicSystem.Instance.waitTime);
    }

    public IEnumerator TheSun()
    {
        Debug.Log("THE SUN");
        yield return new WaitForSeconds(0.3f);
        MessageText.text = "\n\nTHE SUN";
        yield return EnemyManager.Instance.TheSun();
        yield return new WaitForSeconds(MagicSystem.Instance.waitTime);
    }
}
