using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("Original");
        }
        else
        {
            Debug.Log("Destroy");
            Destroy(gameObject);
        }
        
    }

    public void InitializeGame()
    {
        SaveData data = SaveManager.Load();

        if (data != null)
        {
            StatusManager.Instance.ApplyData(data);
        }
        else
        {
            StatusManager.Instance.SetDefaultStatus();
        }

        SaveGame();

        SceneManager.LoadScene("TitleScene");
    }

    public void SaveGame()
    {
        SaveData data = StatusManager.Instance.ToSaveData();
        SaveManager.Save(data);
    }

    public void LoadGame()
    {
        SaveData data = SaveManager.Load();
        if (data != null)
            StatusManager.Instance.ApplyData(data);
    }

}

