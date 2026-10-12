using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class RetryButton : MonoBehaviour
{  
    [SerializeField] private Image fadeBlack;
    public CurrentStage currentStage;
    public StageData stageData;
    public void OnClick()
    {
        stageData = currentStage.stage;
        currentStage.stage = stageData;
        fadeBlack.raycastTarget = true;
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

