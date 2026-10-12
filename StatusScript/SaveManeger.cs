using UnityEngine;

public static class SaveManager
{
    private const string SaveKey = "save_data";

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();

        Debug.Log("Saved");
    }

    public static SaveData Load()
    {
        if (!PlayerPrefs.HasKey(SaveKey))
        {
            Debug.Log("Save data not found.");
            return null;
        }

        try
        {
            string json = PlayerPrefs.GetString(SaveKey);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            Debug.Log("Loaded");
            return data;
        }
        catch
        {
            Debug.LogWarning("Save data is broken.");
            return null;
        }
    }
}
