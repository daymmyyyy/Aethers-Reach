using UnityEngine;
using UnityEngine.UI;

public class DeityUIManager : MonoBehaviour
{
    public static DeityUIManager Instance;

    [Header("Testing")]
    [Tooltip("Check this to simulate first-time player dialogue.")]
    public bool isFirstTimePlayer = true;

    [Header("UI Elements")]
    public Text deityDialogueText;
    public Button viewDiaryButton;
    public Button yesButton;
    public Button noButton;

    private int currentBiomeIndex;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Auto-detect current biome from BiomeManager
        if (BiomeManager.Instance != null)
            currentBiomeIndex = BiomeManager.Instance.currentBiomeIndex;

        // First-time dialogue or normal welcome
        if (isFirstTimePlayer || GetUnlockedCount(currentBiomeIndex) == 0)
        {
            ShowFirstTimeDialogue();
        }
        else
        {
            ShowDeityDialogue();
        }

        yesButton.onClick.AddListener(OnYesPressed);
        noButton.onClick.AddListener(OnNoPressed);
    }

    /// <summary>
    /// Show first-time dialogue
    /// </summary>
    public void ShowFirstTimeDialogue()
    {
        deityDialogueText.text = "Welcome, mortal. You hold the diary for the first time...";
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        viewDiaryButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// Show deity dialogue based on unlocked entries and currency
    /// </summary>
    public void ShowDeityDialogue()
    {
        int unlockedCount = GetUnlockedCount(currentBiomeIndex);
        int totalEntries = DiaryUnlockManager.Instance.diaryDatabase.biomes[currentBiomeIndex].entries.Length;

        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        viewDiaryButton.gameObject.SetActive(true);

        if (unlockedCount >= totalEntries)
        {
            deityDialogueText.text = "You have learned all I know…";
        }
        else if (unlockedCount == 0)
        {
            deityDialogueText.text = "Venture forth and earn the first tale of this land!";
        }
        else
        {
            int cost = DiaryUnlockManager.Instance.diaryDatabase.biomes[currentBiomeIndex].entries[unlockedCount].cost;
            int playerCurrency = RelicCurrency.GetTotalCurrency();

            if (playerCurrency >= cost)
            {
                deityDialogueText.text = $"Ah… for {cost} relics, I shall share my secrets (Entry {unlockedCount + 1})!";
                yesButton.gameObject.SetActive(true);
                noButton.gameObject.SetActive(true);
            }
            else
            {
                deityDialogueText.text = $"Begone, penniless mortal! You need {cost} relics.";
            }
        }
    }

    /// <summary>
    /// Unlocks the next entry for current biome
    /// </summary>
    private void OnYesPressed()
    {
        int unlockedCount = GetUnlockedCount(currentBiomeIndex);
        bool unlocked = DiaryUnlockManager.Instance.TryUnlockDiary(currentBiomeIndex, unlockedCount);

        deityDialogueText.text = unlocked
            ? $"It is done. Diary Entry {unlockedCount + 1} is now yours."
            : "Something went wrong. Please try again.";

        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);

        if (CurrencyUI.Instance != null)
            CurrencyUI.Instance.UpdateCurrencyDisplay();
    }

    /// <summary>
    /// Player declines unlocking
    /// </summary>
    private void OnNoPressed()
    {
        deityDialogueText.text = "Very well. Return when you are ready.";
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// Get number of unlocked entries in the current biome
    /// </summary>
    private int GetUnlockedCount(int biomeIndex)
    {
        int count = 0;
        var biome = DiaryUnlockManager.Instance.diaryDatabase.biomes[biomeIndex];

        for (int i = 0; i < biome.entries.Length; i++)
        {
            if (DiaryUnlockManager.Instance.IsEntryUnlocked(biomeIndex, i))
                count++;
        }

        return count;
    }

    /// <summary>
    /// Called when the deity button on main menu is pressed
    /// </summary>
    public void OnDeityButtonPressed()
    {
        ShowDeityDialogue();
    }

    public void OnBackToMMPressed()
    {
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
    }
}
