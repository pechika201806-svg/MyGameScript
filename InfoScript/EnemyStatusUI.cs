using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyStatusUI : MonoBehaviour
{
    public static EnemyStatusUI Instance;

    private TMP_Text enemyNameText;
    private Image enemyImage;
    private TMP_Text hpText;
    private TMP_Text atpText;
    private TMP_Text typeText;
    private CanvasGroup canvasGroup;

    [SerializeField] private float fadeDuration = 0.2f;

    private Coroutine fadeCoroutine;

    void Awake()
    {
        Instance = this;

        enemyNameText = transform.Find("EnemyName").GetComponent<TMP_Text>();
        enemyImage = transform.Find("EnemyImage").GetComponent<Image>();
        hpText = transform.Find("HP").GetComponent<TMP_Text>();
        atpText = transform.Find("Atp").GetComponent<TMP_Text>();
        typeText = transform.Find("Type").GetComponent<TMP_Text>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Show(GameObject enemyPrefab)
    {
        if (enemyPrefab == null) return;

        Enemy enemyScript = enemyPrefab.GetComponent<Enemy>();
        if (enemyScript == null) return;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeAndShow(enemyScript, enemyPrefab));
    }

    private IEnumerator FadeAndShow(Enemy enemyScript, GameObject enemyPrefab)
    {
        canvasGroup.blocksRaycasts = false;

        yield return Fade(canvasGroup.alpha, 0f);

        enemyNameText.text = enemyScript.enemyname;
        enemyImage.sprite = EnemyImageUtility.GetEnemySprite(enemyPrefab);
        hpText.text = $"HP: {enemyScript.hp}";
        atpText.text = $"Atp: {enemyScript.atp}";
        typeText.text = $"Type: {enemyScript.type}";
        yield return Fade(canvasGroup.alpha, 1f);

        canvasGroup.blocksRaycasts = true;
    }

    private IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = to;
    }

    public void Close()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        canvasGroup.blocksRaycasts = false;
        yield return Fade(canvasGroup.alpha, 0f);
    }
}
