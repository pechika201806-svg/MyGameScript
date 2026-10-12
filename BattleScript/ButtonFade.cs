using System.Collections;
using UnityEngine;

public class ButtonFade : MonoBehaviour
{

    public CanvasGroup cg;


    public void OnClickOpen()
    {
        StartCoroutine(FadeIn(0.8f));
    }

    private IEnumerator FadeIn(float duration)
    {
        float t = 0f;

        cg.alpha = 0f;
        cg.blocksRaycasts = false;

        yield return new WaitForSeconds(0.2f);

        cg.blocksRaycasts = true;

        while (t < duration)
        {
            t += Time.deltaTime;
            cg.alpha = t / duration;
            yield return null;
        }

        cg.alpha = 1f;
    }

    public void OnClickClose()
    {
        StartCoroutine(FadeOut(0.8f));
    }

    private IEnumerator FadeOut(float duration)
    {
        float t = 0f;

        cg.alpha = 1f;
        cg.blocksRaycasts = true;

        yield return new WaitForSeconds(0.2f);

        cg.blocksRaycasts = false;

        while (t < duration)
        {
            t += Time.deltaTime;
            cg.alpha = 1f - t / duration;
            yield return null;
        }

        cg.alpha = 0f;
    }
}

