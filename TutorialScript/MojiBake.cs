using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;
using TMPro;

public class MojiBake : MonoBehaviour
{
    [SerializeField] private TMP_Text targetText;            
    [SerializeField] private string fileName = "japanese";
    [SerializeField] private int charCount = 210;
    [SerializeField, Min(0.1f)] private float interval = 0.3f;

    private List<string> chars = new List<string>();
    private float timer;
    private StringBuilder sb = new StringBuilder(128);

    void Awake()
    {
        TextAsset txt = Resources.Load<TextAsset>(fileName);
        if (txt == null)
        {
            Debug.LogError($"Resources/{fileName}.txt が見つかりません");
            return;
        }

        string all = txt.text.Replace("\r", "").Replace("\n", "").Replace(" ", "").Replace("　", "");
        var e = StringInfo.GetTextElementEnumerator(all);
        while (e.MoveNext()) chars.Add((string)e.Current);
    }

    void Start()
    {
        Refresh();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer -= interval;
            Refresh();
        }
    }

    private void Refresh()
    {
        targetText.text = "void combo (int ID1,int ID2)\n{\n" + GetRandomChars() + "\n}";
    }

    public string GetRandomChars()
    {
        if (chars.Count == 0) return "";

        sb.Clear();
        for (int i = 0; i < charCount; i++)
            sb.Append(chars[Random.Range(0, chars.Count)]);
        return sb.ToString();
    }
}
