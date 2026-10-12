using UnityEngine;

public class BattleTutorialManager : MonoBehaviour
{

    [System.Serializable]
    public class TutorialSet
    {
        public int stageID;
        public int tutorialID;
        public CanvasGroup tutorialCG;
    }

    [SerializeField] GetTutorial getTutorial;

    public TutorialSet[] tutorialSets;
    public CurrentStage currentStage;

    void Start()
    {
        var s = StatusManager.Instance;
        var stageData = currentStage.stage;
        bool shown = false;
        foreach (var t in tutorialSets)
        {
            if (t.stageID == stageData.stageID && t.tutorialID == s.tutorialCount)
            {
                t.tutorialCG.alpha = 1;
                t.tutorialCG.blocksRaycasts = true;
                shown = true;
            }
        }
        if (shown)
        {
            s.tutorialCount++;
            getTutorial.Refresh();
        }
    }
}
