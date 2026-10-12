using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;



public class GoTitle : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    public void GoBoot()
    {
        SceneManager.LoadScene("BootScene");
    }

    public void WaitAndGoBoot()
    {
        PlayerPrefs.SetInt("Fade", 1);
        fadeImage.raycastTarget = true;
        StartCoroutine(GoBootDelayed());
    }
    private IEnumerator GoBootDelayed()
    {
        float t = 0f;
        while (t < 0.6f)
        {
            t += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, t / 0.6f);
            yield return null;
        }
        fadeImage.raycastTarget = false;
        SceneManager.LoadScene("BootScene");
    }
}
