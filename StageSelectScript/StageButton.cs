using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class StageButton : MonoBehaviour
{
    public StageData stage;
    public CurrentStage currentStage;
    [SerializeField] private Image fadeBlack;
    public void OnClick()
    {
        fadeBlack.raycastTarget = true;
        currentStage.stage = stage;
        StartCoroutine(GoBattleScene());
    }
    private IEnumerator GoBattleScene()
    {
        float t = 0f;
        while (t < 0.6f)
        {
            t += Time.deltaTime;
            fadeBlack.color = new Color(0, 0, 0, t / 0.6f);
            yield return null;
        }
        fadeBlack.raycastTarget = false;
        SceneManager.LoadScene("BattleScene");
    }
}

