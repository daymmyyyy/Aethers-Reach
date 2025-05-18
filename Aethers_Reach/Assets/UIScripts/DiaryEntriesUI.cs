using UnityEngine;
using UnityEngine.UI;

public class DiaryEntriesUI : MonoBehaviour
{
    public Text relicText;

    void Start()
    {
        int totalRelics = PlayerPrefs.GetInt("RelicsCollected", 0);
        relicText.text = $"Total Relics Found: {totalRelics}";
    }
}
