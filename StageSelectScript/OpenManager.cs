using UnityEngine;
using UnityEngine.UI;

public class OpenManager : MonoBehaviour
{
    public Button[] StageButton;

    private void Start()
    {
        int OpenStage = StatusManager.Instance.reachedStage + 1;
        for (int i = 0; i < OpenStage; i++)
        {
            StageButton[i].interactable = true;
        }
    }    
}
