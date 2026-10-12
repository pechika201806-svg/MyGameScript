using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageCell : MonoBehaviour
{
    [SerializeField] private StageData targetStage;
    [SerializeField] private GameObject enemyCellPrefab;
    private Transform enemyCellParent;
    public TMP_Text stagename;

    void Awake()
    {
        enemyCellParent = transform.Find("EnemyButton");
        stagename.text = targetStage.stageName;
    }

    void Start()
    {
        var sm = StatusManager.Instance;
        bool isUnreached = targetStage.stageID > sm.reachedStage + 1;

        List<GameObject> enemyPrefabs = EnemyImageUtility.GetEnemyPrefabsInStage(targetStage);

        foreach (var prefab in enemyPrefabs)
        {
            Sprite sprite = EnemyImageUtility.GetEnemySprite(prefab);
            if (sprite == null) continue;

            GameObject cellObj = Instantiate(enemyCellPrefab, enemyCellParent);
            EnemyCell cell = cellObj.GetComponent<EnemyCell>();
            cell.Setup(sprite, prefab);

            if (isUnreached)
            {
                Image image = cellObj.GetComponent<Image>();
                if (image != null) image.color = new Color(0, 0, 0);

                Button button = cellObj.GetComponent<Button>();
                if (button != null) button.interactable = false;
            }
        }
    }
}
