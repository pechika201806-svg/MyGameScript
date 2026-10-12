using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public static class EnemyImageUtility
{
    public static List<Sprite> GetEnemySpritesInStage(StageData stage)
    {
        List<Sprite> sprites = new List<Sprite>();

        if (stage == null || stage.enemies == null) return sprites;

        HashSet<GameObject> seenPrefabs = new HashSet<GameObject>();

        foreach (var enemyData in stage.enemies)
        {
            if (enemyData.prefab == null) continue;
            if (!seenPrefabs.Add(enemyData.prefab)) continue;

            Sprite sprite = GetEnemySprite(enemyData.prefab);
            if (sprite != null) sprites.Add(sprite);
        }
        Debug.Log($"最終的なsprites.Count = {sprites.Count}");

        return sprites;
    }

    public static Dictionary<string, Sprite> GetEnemySpriteDictInStage(StageData stage)
    {
        Dictionary<string, Sprite> result = new Dictionary<string, Sprite>();

        if (stage == null || stage.enemies == null) return result;

        foreach (var enemyData in stage.enemies)
        {
            if (enemyData.prefab == null) continue;

            Sprite sprite = GetEnemySprite(enemyData.prefab);
            if (sprite == null) continue;

            Enemy enemyScript = enemyData.prefab.GetComponent<Enemy>();
            string key = enemyScript != null ? enemyScript.enemyname : enemyData.prefab.name;
            result[key] = sprite;
        }

        return result;
    }

    public static Sprite GetEnemySprite(GameObject enemyPrefab)
    {
        if (enemyPrefab == null) return null;

        Image[] imgs = enemyPrefab.GetComponentsInChildren<Image>(true);
        if (imgs.Length == 0)
        {
            Debug.LogWarning($"{enemyPrefab.name} にImageコンポーネントが見つかりませんでした");
            return null;
        }

        return imgs[0].sprite;
    }

    public static List<GameObject> GetEnemyPrefabsInStage(StageData stage)
    {
        List<GameObject> prefabs = new List<GameObject>();

        if (stage == null || stage.enemies == null) return prefabs;

        HashSet<GameObject> seenPrefabs = new HashSet<GameObject>();

        foreach (var enemyData in stage.enemies)
        {
            if (enemyData.prefab == null) continue;
            if (!seenPrefabs.Add(enemyData.prefab)) continue;

            prefabs.Add(enemyData.prefab);
        }

        return prefabs;
    }
}
