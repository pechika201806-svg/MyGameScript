using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SaveLoadUIController : MonoBehaviour
{
    public CanvasGroup SaveImage;
    public CanvasGroup LoadImage;

    public void OnSaveButton()
    {
        GameManager.Instance.SaveGame();
        StartCoroutine(FadeRoutine(SaveImage));
    }

    public void OnLoadButton()
    {
        GameManager.Instance.LoadGame();
        StartCoroutine(FadeRoutine(LoadImage));
    }

    private IEnumerator FadeRoutine(CanvasGroup cg)
    {
        cg.gameObject.SetActive(true);

        float maxAlpha = 0.83f;

        for (float t = 0; t < 1f; t += Time.deltaTime)
        {
            cg.alpha = Mathf.Lerp(0f, maxAlpha, t);
            yield return null;
        }
        cg.alpha = maxAlpha;

        yield return new WaitForSeconds(1f);

        for (float t = 1f; t > 0f; t -= Time.deltaTime)
        {
            cg.alpha = Mathf.Lerp(0f, maxAlpha, t);
            yield return null;
        }
        cg.alpha = 0f;

        cg.gameObject.SetActive(false);
    }
}



