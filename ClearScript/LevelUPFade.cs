using UnityEngine;
using System.Collections;

public class LevelUPfade : MonoBehaviour
{
    [SerializeField] private CanvasGroup cg;

    void Start()
    {
        if(ResultManager.Instance.levelup)
        StartCoroutine(BlinkSimple(0.3f,3));
    }

    private IEnumerator BlinkSimple(float interval, int count)
    {
        cg.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.9f);

        for (int i = 0; i < count; i++)
        {
            cg.alpha = 1f;
            yield return new WaitForSeconds(interval);

            cg.alpha = 0f;
            yield return new WaitForSeconds(interval-0.15f);
        }

        cg.alpha = 0f;
    }

}
