using UnityEngine;
using UnityEngine.UI;

public class CustomButton: MonoBehaviour
{
    public Button customButton;

    private void Start()
    {
        int stage = StatusManager.Instance.reachedStage;
        if (stage >= 15)
            customButton.interactable = true;
        else
            customButton.interactable = false;
        
    }
}
