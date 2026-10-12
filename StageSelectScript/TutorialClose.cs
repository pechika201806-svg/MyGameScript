using UnityEngine;
using System.Collections;

public class TutorialClose : MonoBehaviour
{
    public CanvasGroup brocg,pcg;
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

        pcg.alpha = 1f;
        pcg.blocksRaycasts = true;

        yield return new WaitForSeconds(0.2f);

        pcg.blocksRaycasts = false;

        while (t < duration)
        {
            t += Time.deltaTime;
            pcg.alpha = 1f - t / duration;
            yield return null;
        }

        pcg.alpha = 0f;
        brocg.alpha = 1f;
        brocg.blocksRaycasts = true;
        closing = false;
    }
}

