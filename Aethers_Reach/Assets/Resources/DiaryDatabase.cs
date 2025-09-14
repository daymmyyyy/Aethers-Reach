using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DiaryEntryData
{
    public string title;
    [TextArea(3, 10)]
    public string content;
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
