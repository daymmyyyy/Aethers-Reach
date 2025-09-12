using UnityEngine;
using UnityEngine.UI;

public class DiaryPageUI : MonoBehaviour
{
    public Button[] entryButtons;       // 9 buttons total (3 per biome)
    public Text[] entryTitles;          // 9 Texts for the button titles
    public Text contentText;            // Right page content area

    private int currentBiomeIndex = 0;  // 0=Skylands, 1=Beach, 2=Ruins

    private void Start()
    {
        // Add listeners for all buttons
        for (int i = 0; i < entryButtons.Length; i++)
        {
            int index = i; // closure
            entryButtons[i].onClick.AddListener(() => OnEntryButtonClicked(index));
        }

        RefreshDiary();
    }

    /// <summary>
    /// Switch biome tab
    /// </summary>
    public void SwitchBiome(int biomeIndex)
    {
        currentBiomeIndex = biomeIndex;
        contentText.text = ""; // Clear right page
        RefreshDiary();
    }

    /// <summary>
    /// Update button titles and interactable state for current biome
    /// </summary>
    private void RefreshDiary()
    {
        var biome = DiaryUnlockManager.Instance.diaryDatabase.biomes[currentBiomeIndex];

        // Hide all buttons first
        for (int i = 0; i < entryButtons.Length; i++)
        {
            entryButtons[i].gameObject.SetActive(false);
        }

        // Show only the 3 buttons for the current biome
        int startIndex = currentBiomeIndex * 3;
        for (int i = 0; i < biome.entries.Length; i++)
        {
            int btnIndex = startIndex + i;
            bool unlocked = DiaryUnlockManager.Instance.IsEntryUnlocked(currentBiomeIndex, i);

            entryButtons[btnIndex].gameObject.SetActive(true);
            entryButtons[btnIndex].interactable = unlocked;
            entryTitles[btnIndex].text = unlocked ? biome.entries[i].title : "Locked Entry";
        }
    }

    /// <summary>
    /// Show entry content on right page when a button is pressed
    /// </summary>
    private void OnEntryButtonClicked(int buttonIndex)
    {
        int biomeIndex = buttonIndex / 3;
        int entryIndex = buttonIndex % 3;

        if (!DiaryUnlockManager.Instance.IsEntryUnlocked(biomeIndex, entryIndex))
            return;

        var entry = DiaryUnlockManager.Instance.diaryDatabase.biomes[biomeIndex].entries[entryIndex];
        contentText.text = entry.content;
    }
}
