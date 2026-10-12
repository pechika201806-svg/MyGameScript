using UnityEngine;

public class GetBook : MonoBehaviour
{
    [System.Serializable]
    public class GetBookSet
    {
        public int tutorialID;
        public CanvasGroup ButtonCG;
    }
    public GetBookSet[] GetBookSets;

    private void Start()
    {
        int OpenBook = StatusManager.Instance.tutorialCount;
        foreach (var bk in GetBookSets)
        {
            if (bk.tutorialID < OpenBook)
            {
                bk.ButtonCG.alpha = 1;
                bk.ButtonCG.blocksRaycasts = true;
                bk.ButtonCG.interactable = true;
            }
            else
            {
                bk.ButtonCG.alpha = 0;
                bk.ButtonCG.blocksRaycasts = false;
                bk.ButtonCG.interactable = false;
            }
        }
    }
}
