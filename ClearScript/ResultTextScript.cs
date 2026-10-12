using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResultTextScript : MonoBehaviour
{
    public TMP_Text ResultText;
    public TMP_Text UPText;
    void Start()
    {
        var s = StatusManager.Instance;
        ResultText.text =    
            $"CLEAR STAGE: {ResultManager.Instance.stagename}\n\n"
            +$"獲得EXP: {ResultManager.Instance.clearEXP}\n\n"
            +$"レベル: {ResultManager.Instance.prelv}→{s.level}\n"
            +$"EXP: {s.exp}\n"
            +$"HP: {ResultManager.Instance.prehp}→{s.hp}\n"
            +$"MP: {ResultManager.Instance.premp}→{s.mp}\n"
            +$"Script Time: {ResultManager.Instance.prelimitTime}s→{s.limitTime}s\n";

        UPText.text =
            "\n\n\n\n"
            +$"{s.level-ResultManager.Instance.prelv}UP\n"
            +$"次のレベルまでEXP: {ResultManager.Instance.leftEXP}\n"
            +$"{s.hp-ResultManager.Instance.prehp}UP\n"
            +$"{s.mp-ResultManager.Instance.premp}UP\n"
            +$"{s.limitTime-ResultManager.Instance.prelimitTime}UP\n";
    }
}
