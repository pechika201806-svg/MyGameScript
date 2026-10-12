using UnityEngine;
using System.Collections;

public class BookClose : MonoBehaviour
{
    public CanvasGroup cg;
    private bool closing;

    public void OnClickClose()
    {
        if (closing) return;
        closing = true;
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
        closing = false;
    }
}

