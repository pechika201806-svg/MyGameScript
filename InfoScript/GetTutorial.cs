using NUnit.Framework.Internal.Execution;
using UnityEngine;
using UnityEngine.UI;

public class GetTutorial : MonoBehaviour
{
    [System.Serializable]
    public class TutorialButtonSet
    {
        public Button tutorialButton;
        public int tutorialID;
    }

    public TutorialButtonSet[] tutorialButtonSets;

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        int OpenTutorial = StatusManager.Instance.tutorialCount;
        foreach(var tB in tutorialButtonSets)
        {
            if (tB.tutorialID < OpenTutorial)
                tB.tutorialButton.interactable = true;
            else
                tB.tutorialButton.interactable = false;
        }
    }
}
