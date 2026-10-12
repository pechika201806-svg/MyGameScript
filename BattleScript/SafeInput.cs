using UnityEngine;
using TMPro;

public class SafeInput : MonoBehaviour
{
    public TMP_InputField input;
    public TMP_Text message;

    private bool inside = false;
    private string lastInternalCopy = "";

    private string previousText = "";
    private int previousCaretPosition = 0;

    private bool checkPasteThisFrame = false;

    void Start()
    {
        input.onSelect.AddListener(_ => inside = true);
        input.onDeselect.AddListener(_ => inside = false);

        previousText = input.text;
        previousCaretPosition = input.stringPosition;
    }

    void Update()
    {
        if (inside && Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.C))
        {
            int start = Mathf.Min(input.stringPosition, input.selectionStringAnchorPosition);
            int end = Mathf.Max(input.stringPosition, input.selectionStringAnchorPosition);

            lastInternalCopy = input.text.Substring(start, end - start);
            GUIUtility.systemCopyBuffer = lastInternalCopy;
        }

        if (inside && Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.V))
        {
            checkPasteThisFrame = true;
        }
    }

    void LateUpdate()
    {
        if (checkPasteThisFrame)
        {
            checkPasteThisFrame = false;

            bool isExternal = GUIUtility.systemCopyBuffer != lastInternalCopy;

            if (isExternal && input.text != previousText)
            {
                input.SetTextWithoutNotify(previousText);
                input.caretPosition = previousCaretPosition;
                input.selectionStringAnchorPosition = previousCaretPosition;
                input.selectionStringFocusPosition = previousCaretPosition;

                message.text += "\n外部からのペーストは許可していません";
                return;
            }
        }

        if (input.text != previousText)
        {
            previousText = input.text;
            previousCaretPosition = input.stringPosition;
        }
    }
}

