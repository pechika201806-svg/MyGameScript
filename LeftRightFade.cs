using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftRightFade : MonoBehaviour
{
    [System.Serializable]
    public class LeftRight
    {
        public Button LeftButton;
        public CanvasGroup RightContent;
    }

    [Header("ステージボタンとScrollViewの対応リスト")]
    [SerializeField] private LeftRight[] LeftRights;

    [Header("フェード設定")]
    [SerializeField] private float fadeDuration = 0.5f;

    private CanvasGroup currentTarget;
    private readonly Dictionary<CanvasGroup, Coroutine> fadeRoutines = new Dictionary<CanvasGroup, Coroutine>();

    private void Awake()
    {
        foreach (var entry in LeftRights)
        {
            var group = entry.RightContent;

            SetGroupVisible(group, false, immediate: true);
            fadeRoutines[group] = null;

            entry.LeftButton.onClick.AddListener(() => OnLeftButtonClicked(group));
        }
    }

    private void OnLeftButtonClicked(CanvasGroup targetGroup)
    {
        if (currentTarget == targetGroup)
            return;

        currentTarget = targetGroup;
        foreach (var entry in LeftRights)
        {
            var group = entry.RightContent;
            bool show = (group == targetGroup);
            StartFade(group, show);
        }
    }

    private void StartFade(CanvasGroup group, bool show)
    {
        if (fadeRoutines.TryGetValue(group, out var existing) && existing != null)
            StopCoroutine(existing);

        if (show)
        {
            group.gameObject.SetActive(true);
        }

        fadeRoutines[group] = StartCoroutine(FadeRoutine(group, show));
    }

    private IEnumerator FadeRoutine(CanvasGroup group, bool show)
    {
        float from = group.alpha;
        float to = show ? 1f : 0f;
        float elapsed = 0f;
        group.interactable = show;
        group.blocksRaycasts = show;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }

        group.alpha = to;

        if (!show)
            group.gameObject.SetActive(false);

        fadeRoutines[group] = null;
    }

    private void SetGroupVisible(CanvasGroup group, bool visible, bool immediate)
    {
        group.gameObject.SetActive(visible || immediate);
        group.interactable = visible;
        group.blocksRaycasts = visible;

        if (immediate)
            group.alpha = visible ? 1f : 0f;
    }
}