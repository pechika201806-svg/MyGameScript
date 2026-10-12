using UnityEngine;
using System.Collections;

public class TitleAppear : MonoBehaviour
{
    [SerializeField] private CanvasGroup cg;
    [SerializeField] private RectTransform rt;

    private void Start()
    {
        StartCoroutine(Appear());
    }

    private IEnumerator Appear()
    {
        float t = 0f;

        cg.alpha = 0f;
        rt.localScale = Vector3.one * 0.6f;

        while (t < 0.8f)
        {
            t += Time.deltaTime;
            float rate = t / 0.8f;

            cg.alpha = rate;
            rt.localScale = Vector3.one * (0.6f + 0.4f * rate);

            yield return null;
        }

        cg.alpha = 1f;
        rt.localScale = Vector3.one;
    }
}
