using UnityEngine;
using TMPro;

public class StatusText : MonoBehaviour
{
    public TMP_Text statusText;

    public void ShowStatus()
    {
        var s = StatusManager.Instance;

        int nextExp = GetNextLevelExp(s.level);

        statusText.text =
            $"Lv:{s.level}\n\n" +
            $"HP:{s.hp}\n\n" +
            $"MP:{s.mp}\n\n" +
            $"Script Time:{s.limitTime}s\n\n" +
            $"EXP:{s.exp}\n" +
            $"次のレベルまでEXP:{nextExp}";
    }

    int GetNextLevelExp(int level)
    {
        var s = StatusManager.Instance;
        return level * 100 - s.exp;
    }
}

