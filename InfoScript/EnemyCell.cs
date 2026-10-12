using UnityEngine;
using UnityEngine.UI;

public class EnemyCell : MonoBehaviour
{
    [SerializeField] private Image image;

    private GameObject enemyPrefab;

    public void Setup(Sprite sprite, GameObject enemyPrefab)
    {
        image.sprite = sprite;
        this.enemyPrefab = enemyPrefab;

        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        EnemyStatusUI.Instance.Show(enemyPrefab);
    }
}
