using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HPMPtext : MonoBehaviour
{
    public TMP_Text HPText;
    public TMP_Text MPText;
    public RectTransform HPBar;
    public RectTransform MPBar;

    float HPmax,MPmax;
    float HPBarMaxWidth, MPBarMaxWidth;

    Color blueHP = new Color(0.2117647f, 0.4980392f, 0.6745098f);
    Color yellowHP = new Color(0.89f, 0.82f, 0);
    Color redHP = new Color(0.66f, 0, 0.03f);



    void Start()
    {
        HPBarMaxWidth = HPBar.sizeDelta.x;
        MPBarMaxWidth = MPBar.sizeDelta.x;
    }
    void Update()
    {
        UpdateHP();
        UpdateMP();
    }

    void UpdateHP()
    {
        HPmax = PlayerManager.Instance.hpmax;
        HPText.text = $"HP: {PlayerManager.Instance.hp}/{HPmax}";
        HPBar.sizeDelta = new Vector2(HPBarMaxWidth * (PlayerManager.Instance.hp / HPmax), HPBar.sizeDelta.y);
        if (PlayerManager.Instance.hp / HPmax <= 0.25f)
        HPBar.GetComponent<Image>().color = redHP;
        else if (PlayerManager.Instance.hp / HPmax <= 0.5f)
        HPBar.GetComponent<Image>().color = yellowHP;
        else
        HPBar.GetComponent<Image>().color = blueHP;

    }

    void UpdateMP()
    {

        MPmax = PlayerManager.Instance.mpmax;
        MPText.text = $"MP: {PlayerManager.Instance.mp}/{MPmax}";
        MPBar.sizeDelta = new Vector2(MPBarMaxWidth * (PlayerManager.Instance.mp / MPmax), MPBar.sizeDelta.y);
    }
}
