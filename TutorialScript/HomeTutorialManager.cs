using UnityEngine;

public class HomeTutorialManager : MonoBehaviour
{

    [System.Serializable]
    public class TutorialSet
    {
        public int stageID;
        public int tutorialID;
        public CanvasGroup tutorialCG;
    }

    public TutorialSet[] tutorialSets;

    void Start()
    {
        var s = StatusManager.Instance;
        bool shown = false;
        foreach (var t in tutorialSets)
        {
            if (t.stageID == s.reachedStage && t.tutorialID == s.tutorialCount)
            {
                t.tutorialCG.alpha = 1;
                t.tutorialCG.blocksRaycasts = true;
                shown = true;
            }
        }
        if (shown)
        {
            s.tutorialCount++;
        }
    }
}
