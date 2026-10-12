using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;



public class Gobattle : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    public void GoBattle()
    {
        SceneManager.LoadScene("StageSelect");
    }

    public void WaitAndGoBattle()
    {
        PlayerPrefs.SetInt("Fade", 1);
        fadeImage.raycastTarget = true;
        StartCoroutine(GoBattleDelayed());
    }
    private IEnumerator GoBattleDelayed()
    {
        float t = 0f;
        while (t < 0.6f)
        {
            t += Time.deltaTime;
            fadeImage.color = new Color(0.788f, 0.635f, 0.458f, t / 0.6f);
            yield return null;
        }
        fadeImage.raycastTarget = false;
        SceneManager.LoadScene("StageSelect");
    }
}
