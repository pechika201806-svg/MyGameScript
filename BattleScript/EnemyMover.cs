using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class EnemyMover : MonoBehaviour
{
    public enum MoveType { Linear, Random, Square, BossMove, Summon}
    public MoveType moveType = MoveType.Linear;

    public int deltaI = 0;
    public int deltaJ = 0;

    [Header("召喚設定")]
    public bool canSummon = false;     
    public int summonInterval = 1;
    public int summonCount = 1;
    public int maxEnemies = 15;
    public List<GameObject> summonPrefabs = new List<GameObject>();
    int summonTurn = 0;

    Enemy self;
    int turnCount = 0;

    void Awake()
    {
        self = GetComponent<Enemy>();
    }

    public IEnumerator Move()
    {
        int newI = self.i;
        int newJ = self.j;

        switch (moveType)
        {
            case MoveType.Linear:
                newI += deltaI;
                newJ += deltaJ;
                break;

            case MoveType.Random:
                var empty = new System.Collections.Generic.List<Vector2Int>();
                for (int a = 0; a < 3; a++)
                    for (int b = 0; b < 5; b++)
                        if (!IsOccupied(a, b))
                            empty.Add(new Vector2Int(a, b));

                if (empty.Count > 0)
                {
                    Vector2Int p = empty[Random.Range(0, empty.Count)];
                    newI = p.x;
                    newJ = p.y;
                }
                break;

            case MoveType.Square:
                int direction = turnCount % 4;
                switch (direction)
                {
                    case 0:
                        newI = -1 + deltaI;
                        newJ = 5 + deltaJ;
                        break;
                    case 1:
                        newI = -1 + deltaI;
                        newJ = -1 + deltaJ;
                        break;
                    case 2:
                        newI = 3 + deltaI;
                        newJ = -1 + deltaJ;
                        break;
                    case 3:
                        newI = 3 + deltaI;
                        newJ = 5 + deltaJ;
                        break;
                }
                break;

            case MoveType.BossMove:
                newI = EnemyManager.Instance.BossI + deltaI;
                newJ = EnemyManager.Instance.BossJ + deltaJ;
                break;

            case MoveType.Summon:
                yield return Summon();
                break;
        }

        turnCount++;
        yield return StartCoroutine(self.MoveTo(newI, newJ));
    }

    bool IsOccupied(int checkI, int checkJ)
    {
        foreach (var e in EnemyManager.AllEnemies)
        {
            if (e == self) continue;
            if (e.i == checkI && e.j == checkJ)
                return true;
        }
        return false;
    }

    public IEnumerator Summon()
    {
        if (!canSummon || summonPrefabs == null || summonPrefabs.Count == 0) yield break;

        summonTurn++;
        if (summonTurn % summonInterval != 0) yield break;

        for (int n = 0; n < summonCount; n++)
        {
            if (EnemyManager.AllEnemies.Count >= maxEnemies) yield break;

            var empty = new List<Vector2Int>();
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 5; j++)
                    if (!IsOccupied(i, j)&& !(i == 1 && j == 2))
                        empty.Add(new Vector2Int(i, j));

            if (empty.Count == 0) yield break;

            Vector2Int pos = empty[Random.Range(0, empty.Count)];

            GameObject prefab = summonPrefabs[Random.Range(0, summonPrefabs.Count)];
            if (prefab == null) continue;

            GameObject obj = Instantiate(prefab, transform.parent);
            Enemy e = obj.GetComponent<Enemy>();
            e.i = pos.x;
            e.j = pos.y;

            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(250f * e.j, -250f * e.i);

            EnemyManager.AllEnemies.Add(e);

            Debug.Log($"{self.enemyname} は {e.enemyname}({e.i},{e.j}) を召喚した！");
            EnemyManager.Instance.MessageText.text +=
                $"\n{self.enemyname} は {e.enemyname}({e.i},{e.j}) を召喚した！";

            yield return StartCoroutine(e.Puun());
        }
    }
}


