using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuFade : MonoBehaviour
{
    [SerializeField] private Image fadeImage;

    private void Start()
    {
        fadeImage.raycastTarget = false;

        if (PlayerPrefs.GetInt("Fade", 0) == 1)
        {
            PlayerPrefs.SetInt("Fade", 0);
            StartCoroutine(FadeIn());
        }
        else
        {
            fadeImage.color = new Color(0, 0, 0, 0);
        }
    }

    private IEnumerator FadeIn()
    {
        float t = 0f;

        Color brown = new Color(0.788f, 0.635f, 0.458f);

        fadeImage.color = new Color(brown.r, brown.g, brown.b, 1);

        while (t < 0.6f)
        {
            t += Time.deltaTime;

            fadeImage.color = new Color(
                brown.r,
                brown.g,
                brown.b,
                1 - (t / 0.6f)
            );

            yield return null;
        }

        fadeImage.color = new Color(brown.r, brown.g, brown.b, 0);
    }
}
