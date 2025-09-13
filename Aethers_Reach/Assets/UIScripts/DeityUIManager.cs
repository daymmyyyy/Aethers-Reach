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
    public Button nextButton;

    private int currentBiomeIndex;
    private string[] currentDialogue;
    private int dialogueIndex = 0;
    private bool pendingYesNo; // Track if Yes/No button should appear

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (BiomeManager.Instance != null)
            currentBiomeIndex = BiomeManager.Instance.currentBiomeIndex;

        if (isFirstTimePlayer)
        {
            Debug.Log("Forced first time dialogue for testing.");
            ShowFirstTimeDialogue();
        }
        else
        {
            // Natural flow
            ShowDeityDialogue();
        }

        yesButton.onClick.AddListener(OnYesPressed);
        noButton.onClick.AddListener(OnNoPressed);
        nextButton.onClick.AddListener(OnNextPressed);
    }



    /// First-time dialogue

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

    /// Deity dialogue based on progress

    public void ShowDeityDialogue()
    {
        var diary = DiaryManager.Instance.diaryDatabase;
        int firstEntriesUnlocked = 0;

        // Count how many first entries are unlocked across all biomes
        for (int i = 0; i < diary.biomes.Length; i++)
        {
            if (DiaryManager.Instance.IsEntryUnlocked(i, 0))
                firstEntriesUnlocked++;
        }

        int unlockedCount = GetUnlockedCount(currentBiomeIndex);
        var biome = diary.biomes[currentBiomeIndex];
        int totalEntries = biome.entries.Length;

        viewDiaryButton.gameObject.SetActive(true);

        // Handle 0 or 1–2 unlocked first entries
        if (firstEntriesUnlocked == 0)
        {
            currentDialogue = new string[]
            {
            "Hmph... You wander aimlessly, yet you bring me nothing of value.",
            "Continue venturing forth, mortal. The first tale of each land shall be free.",
            "Return when you have proven your curiosity."
            };
            StartDialogueSequence(false);
            return;
        }
        else if (firstEntriesUnlocked < diary.biomes.Length)
        {
            currentDialogue = new string[]
            {
            "I see you have begun your journey, mortal...",
            "Some lands have yielded their first secrets to you.",
            "Venture further, and bring me crystals for deeper knowledge."
            };
            StartDialogueSequence(false);
            return;
        }

        // All 3 first entries unlocked → normal flow
        if (unlockedCount >= totalEntries)
        {
            currentDialogue = new string[]
            {
            "You have learned all I know... for now.",
            "Even I, the great one, hold no further secrets for you."
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
                StartDialogueSequence(true);
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



    /// Starts dialogue sequence

    private void StartDialogueSequence(bool showYesNoAtEnd)
    {
        dialogueIndex = 0;
        deityDialogueText.text = currentDialogue[dialogueIndex];

        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(currentDialogue.Length > 1);

        pendingYesNo = showYesNoAtEnd; // Correctly track Yes/No
    }


    /// Next button

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
        bool unlocked = DiaryManager.Instance.TryUnlockDiary(currentBiomeIndex, unlockedCount);

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
        var biome = DiaryManager.Instance.diaryDatabase.biomes[biomeIndex];
        for (int i = 0; i < biome.entries.Length; i++)
        {
            if (DiaryManager.Instance.IsEntryUnlocked(biomeIndex, i))
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
