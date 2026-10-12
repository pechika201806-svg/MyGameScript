using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public BattleManager battleManager;
    public enum Turn { Start, Player, Enemy }
    public Turn currentTurn;
    public TMP_InputField codeInput;
    public Button runButton;
    public Button turnButton;
    public RectTransform timeBar;
    public TMP_Text messageText;
    public TMP_Text turnbuttonText;
    private Coroutine timerCoroutine;

    private float timeLeft;
    public float playerTimeLimit;
    public int mpmax;
    public int AutoHealCount=0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        playerTimeLimit = PlayerManager.Instance.time;
        currentTurn=Turn.Start;
        StartTurn();
        
    }

    


    public void NextTurn()
    {
        if (currentTurn == Turn.Player)
        {
            currentTurn = Turn.Enemy;
            messageText.text = "";
            EnemyTurn();
        }
        else
        {
            currentTurn = Turn.Player;
            messageText.text = "";
            PlayerTurn();
        }
    }

    void StartTurn()
    {
        codeInput.interactable = false;
        runButton.interactable = false;
        turnButton.interactable = true;
        turnbuttonText.text = "START";
    }


    void PlayerTurn()
    {
        timeBar.localScale = new Vector3(1f, 1f, 1f);
        codeInput.interactable = true;
        runButton.interactable = true;
        turnButton.interactable = false;
        codeInput.text = "";
        turnbuttonText.text ="ENEMY";
        PlayerManager.Instance.mp = PlayerManager.Instance.mpmax;
        ComboSystem.Instance.MPCount = false;
        Debug.Log("プレイヤーターン開始");
        messageText.text = "プレイヤーターン開始";
        Timer();
    }

    void EnemyTurn()
    {
        turnButton.interactable=false;
        turnbuttonText.text ="START";
        Debug.Log("敵ターン開始");
        messageText.text = "敵ターン開始";
        if (AutoHealCount > 0)
        {
            for (int i = 0; i < AutoHealCount; i++)
            {
                AutoHeal();
            }
            messageText.text += $"\n自動回復が{AutoHealCount}回実行されました";
        }
        EnemyManager.Instance.DoEnemyActions();
    }

    void Timer()
    {
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        timerCoroutine = StartCoroutine(TimerRoutine());
    }
    public void StopTimer()
    {
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
    }


    IEnumerator TimerRoutine()
    {
        timeLeft = playerTimeLimit;

        while (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;

            float ratio = Mathf.Clamp01(timeLeft / playerTimeLimit);
            timeBar.localScale = new Vector3(ratio, 1f, 1f);

            yield return null;
        }

        Debug.Log("時間切れでコードが実行されました");
        messageText.text += "\n時間切れでコードが実行されました";

        battleManager.OnRunButtonClicked();

    }

    public void AutoHeal()
    {
        if(MagicSystem.Instance.MPjudge(1))
        PlayerManager.Instance.Heal(30);
    }
}
