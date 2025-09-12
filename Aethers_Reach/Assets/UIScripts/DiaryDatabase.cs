using UnityEngine;

[CreateAssetMenu(fileName = "DiaryDatabase", menuName = "Game/Diary Database")]
public class DiaryDatabase : ScriptableObject
{
    public BiomeData[] biomes;
}

[System.Serializable]
public class BiomeData
{
    public string biomeName;
    public DiaryEntry[] entries;
}

[System.Serializable]
public class DiaryEntry
{
    public string title;
    [TextArea] public string content;
    public int cost; // Cost to unlock
}

