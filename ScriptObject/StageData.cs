using UnityEngine;
[System.Serializable]
public class EnemySpawnData
{
    public GameObject prefab;
    public int i;
    public int j;
}



[CreateAssetMenu(fileName = "StageData", menuName = "Scriptable Objects/StageData")]
public class StageData : ScriptableObject
{
    public int stageID;
    public int clearEXP;
    public string stageName;
    public Sprite stageImage;

    public EnemySpawnData[] enemies;

}
