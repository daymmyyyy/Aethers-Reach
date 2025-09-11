using UnityEngine;
using UnityEngine.UI;

public class DiaryPageUI : MonoBehaviour
{
    public Text[] entryTexts;
    public int biomeIndex;

    public void Refresh()
    {
        var biome = DiaryUnlockManager.Instance.diaryDatabase.biomes[biomeIndex];
        for (int i = 0; i < biome.entries.Length; i++)
        {
            entryTexts[i].text = biome.entries[i].unlocked
                ? biome.entries[i].title
                : "Locked Entry";
        }
    }
}
