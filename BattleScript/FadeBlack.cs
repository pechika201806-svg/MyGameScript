using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class FadeBlack : MonoBehaviour
{
    [SerializeField] private Image fadeBlack;
    public static FadeBlack Instance;
    private bool isFading = false;
    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        fadeBlack.raycastTarget = false;
        StartCoroutine(FadeIn());
    }

    public void FadeOut()
    {
        if (isFading) return;
        isFading = true;
        fadeBlack.raycastTarget = true;
        StartCoroutine(GoScene());
    }
    private IEnumerator GoScene()
    {
        float t = 0f;
        yield return new WaitForSeconds(1.5f);
        
        while (t < 0.6f)
        {
            t += Time.deltaTime;
            fadeBlack.color = new Color(0, 0, 0, t / 0.6f);
            yield return null;
        }
        fadeBlack.raycastTarget = false;
        if(PlayerManager.Instance.gameover) SceneManager.LoadScene("GameOver");
        else if(EnemyManager.Instance.clear) SceneManager.LoadScene("Clear");

    }

    private IEnumerator FadeIn()
    {
        float t = 0f;

        fadeBlack.color = new Color(0, 0, 0, 1);
        yield return new WaitForSeconds(0.3f);
        while (t < 0.6f)
        {
            t += Time.deltaTime;

            fadeBlack.color = new Color(0, 0, 0,1 - (t / 0.6f));

            yield return null;
        }

        fadeBlack.color = new Color(0, 0, 0, 0);
    }
}
