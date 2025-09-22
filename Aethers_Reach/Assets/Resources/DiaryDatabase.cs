using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class DiaryEntryData
{
    public string title;
    public Sprite content;
    public int cost;
}

[System.Serializable]
public class BiomeData
{
    public DiaryEntryData[] entries;
}

[CreateAssetMenu(fileName = "DiaryDatabase", menuName = "Database/DiaryDatabase")]
public class DiaryDatabase : ScriptableObject
{
    public BiomeData[] biomes;
}
