using UnityEngine;
using System.Collections;

public class TutorialOpen: MonoBehaviour
{

    public CanvasGroup cg;
    private bool open;

    public void OnClick()
    {
        if (open) return;
        open = true;
        StartCoroutine(FadeIn(0.8f));
    }

    private IEnumerator FadeIn(float duration)
    {
        float t = 0f;

        cg.alpha = 0f;
        cg.blocksRaycasts = true;
        yield return new WaitForSeconds(0.2f);

        while (t < duration)
        {
            t += Time.deltaTime;
            cg.alpha = t / duration;
            yield return null;
        }

        cg.alpha = 1f;
        open = false;
    }
}
