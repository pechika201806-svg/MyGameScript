using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    public CurrentStage currentStage;
    public StageData stageData;
    public EnemyManager enemyManager;
    public Image backImage;
    public Transform enemiesRoot;

    void Awake()
    {
        stageData = currentStage.stage;
        
        if (backImage != null && stageData.stageImage != null)
        {
            backImage.sprite = stageData.stageImage;
        }

        CreateEnemies();
        enemyManager.SpawnEnemies();
    }

    void CreateEnemies()
    {
        foreach (var info in stageData.enemies)
        {
            GameObject enemyObj = Instantiate(info.prefab, enemiesRoot);

            Enemy enemy = enemyObj.GetComponent<Enemy>();
            enemy.i = info.i;
            enemy.j = info.j;

            Debug.Log($"敵生成: {enemyObj.name} ({info.i},{info.j})");
        }
    }
    public void SetStage(StageData stage)
    {
        stageData = stage;
    }
}

