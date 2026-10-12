using System.Collections;
using TMPro;
using UnityEngine;


public class BookOpen : MonoBehaviour
{
    public CanvasGroup cg;
    public TMP_InputField codeInput;
    public TMP_Text funcName;
    public TMP_Text PlayerMessage;
    public TMP_Text buttontext;
    public CustomCheck customCheck;
    public int customID;

    private void Update()
    {
        var stm = StatusManager.Instance;

        if (stm.userFunctions.Count <= customID)
        {
            buttontext.text = $"Custom{customID + 1}";
            return;
        }

        var func = stm.userFunctions[customID];

        if (func == null)
        {
            buttontext.text = $"Custom{customID + 1}";
            return;
        }

        buttontext.text = func.Name;
    }


    public void OnClickOpen()
    {
        customCheck.customID = customID;
        var sm = StatusManager.Instance;
        if (sm.userFunctions.Count <= customID)
        {
            funcName.text = $"Custom{customID + 1}";
            PlayerMessage.text =
                        "関数型："
                        + "\n\n引数数："
                        + "\n\n本体トークン数：";
            codeInput.text = "";
        }
        else
        {
            var func = sm.userFunctions[customID];
            if (func == null)
            {
                funcName.text = $"Custom{customID + 1}";
                PlayerMessage.text =
                    "関数型："
                    + "\n\n引数数："
                    + "\n\n本体トークン数：";
                codeInput.text = "";
            }
            else
            {
                funcName.text = $"{func.Name}";
                PlayerMessage.text =
                    $"関数型：{func.ReturnType}"
                    + $"\n\n引数数：{func.ArgNames.Count}"
                    + $"\n\n本体トークン数：{func.BodyTokens.Count}/100";
                codeInput.text = $"{func.Code}";
            }

        }


        StartCoroutine(FadeIn(0.8f));
    }

    private IEnumerator FadeIn(float duration)
    {
        float t = 0f;

        cg.alpha = 0f;
        cg.blocksRaycasts = false;

        yield return new WaitForSeconds(0.2f);

        cg.blocksRaycasts = true;

        while (t < duration)
        {
            t += Time.deltaTime;
            cg.alpha = t / duration;
            yield return null;
        }

        cg.alpha = 1f;
    }
}
