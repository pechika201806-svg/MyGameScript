using UnityEngine;
using System.Collections;

public class Statusfade : MonoBehaviour
{
    [SerializeField] private CanvasGroup cg;
    [SerializeField] private float duration = 0.5f;

    public void ClickFade()
    {
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float t = 0f;

        cg.alpha = 0f;
        cg.gameObject.SetActive(true);

        while (t < duration)
        {
            t += Time.deltaTime;
            cg.alpha = t / duration;
            yield return null;
        }

        cg.alpha = 1f;
    }
}
