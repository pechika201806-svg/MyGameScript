using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeClear : MonoBehaviour
{
    [SerializeField] private Image fadeBlack;
    private void Start()
    {
        fadeBlack.raycastTarget = false;
        StartCoroutine(FadeIn());
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
