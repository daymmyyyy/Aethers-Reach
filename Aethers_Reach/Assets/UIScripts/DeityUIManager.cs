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
    public Button nextButton; // Next button to progress dialogue

    private int currentBiomeIndex;
    private string[] currentDialogue;
    private int dialogueIndex = 0;
    private bool pendingYesNo; // Track if Yes/No should appear

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Auto-detect biome
        if (BiomeManager.Instance != null)
            currentBiomeIndex = BiomeManager.Instance.currentBiomeIndex;

        // Decide which dialogue to show
        if (isFirstTimePlayer)
        {
            ShowFirstTimeDialogue();
        }
        else
        {
            ShowDeityDialogue();
        }

        yesButton.onClick.AddListener(OnYesPressed);
        noButton.onClick.AddListener(OnNoPressed);
        nextButton.onClick.AddListener(OnNextPressed);
    }

    /// <summary>
    /// First-time dialogue
    /// </summary>
    public void ShowFirstTimeDialogue()
    {
        currentDialogue = new string[]
        {
            "Welcome, mortal... You hold the diary for the first time.",
            "I have heard of your curiosity.",
            "Complete your tasks: traverse every land to earn the first tale of each biome.",
            "Bring me the crystals you collect on your journey, and I shall share my knowledge."
        };

        StartDialogueSequence(false); // No Yes/No here
    }

    /// <summary>
    /// Deity dialogue based on progress
    /// </summary>
    public void ShowDeityDialogue()
    {
        int unlockedCount = GetUnlockedCount(currentBiomeIndex);
        var biome = DiaryUnlockManager.Instance.diaryDatabase.biomes[currentBiomeIndex];
        int totalEntries = biome.entries.Length;

        viewDiaryButton.gameObject.SetActive(true);

        if (unlockedCount >= totalEntries)
        {
            currentDialogue = new string[]
            {
                "You have learned all I know... for now.",
                "Even I, the great one, hold no further secrets for you."
            };
            StartDialogueSequence(false);
        }
        else if (unlockedCount == 0)
        {
            currentDialogue = new string[]
            {
                "Venture forth, and prove your worth.",
                "Return to me with crystals, and I shall enlighten you."
            };
            StartDialogueSequence(false);
        }
        else
        {
            int cost = biome.entries[unlockedCount].cost;
            int playerCurrency = RelicCurrency.GetTotalCurrency();

            if (playerCurrency >= cost)
            {
                currentDialogue = new string[]
                {
                    $"Ah... so you desire knowledge of Entry {unlockedCount + 1}.",
                    $"It shall cost you {cost} relics, mortal.",
                    "Do you dare pay the price?"
                };
                StartDialogueSequence(true); // Yes/No will show at end
            }
            else
            {
                currentDialogue = new string[]
                {
                    $"Begone, penniless mortal! You lack the {cost} relics required.",
                    "Return when you are worthy of my wisdom."
                };
                StartDialogueSequence(false);
            }
        }
    }

    /// <summary>
    /// Starts dialogue sequence
    /// </summary>
    private void StartDialogueSequence(bool showYesNoAtEnd)
    {
        dialogueIndex = 0;
        deityDialogueText.text = currentDialogue[dialogueIndex];

        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(currentDialogue.Length > 1);

        pendingYesNo = showYesNoAtEnd; // Correctly track Yes/No
    }

    /// <summary>
    /// Next button
    /// </summary>
    private void OnNextPressed()
    {
        dialogueIndex++;
        if (dialogueIndex < currentDialogue.Length)
        {
            deityDialogueText.text = currentDialogue[dialogueIndex];
        }
        else
        {
            nextButton.gameObject.SetActive(false);
            if (pendingYesNo)
            {
                yesButton.gameObject.SetActive(true);
                noButton.gameObject.SetActive(true);
            }
        }
    }

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

    private void OnNoPressed()
    {
        deityDialogueText.text = "Very well. Return when you are ready.";
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
    }

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

    public void OnDeityButtonPressed() => ShowDeityDialogue();
    public void OnBackToMMPressed()
    {
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
    }
}
