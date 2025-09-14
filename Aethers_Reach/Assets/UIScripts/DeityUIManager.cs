using UnityEngine;
using UnityEngine.UI;

public class DeityUIManager : MonoBehaviour
{
    public static DeityUIManager Instance;

    [Header("UI Elements")]
    public Text deityDialogueText;
    public Button viewDiaryButton;
    public Button yesButton;
    public Button noButton;
    public Button nextButton;

    private int currentBiomeIndex;
    private string[] currentDialogue;
    private int dialogueIndex = 0;
    private bool pendingYesNo;
    private bool introShown = false; // track if intro has been shown

    private void Awake()
    {
        Instance = this;

        if (deityDialogueText == null)
            deityDialogueText = GetComponentInChildren<Text>(true);

        if (yesButton == null || noButton == null || nextButton == null)
        {
            var buttons = GetComponentsInChildren<Button>(true);
            foreach (var b in buttons)
            {
                var name = b.name.ToLower();
                if (yesButton == null && name.Contains("yes")) yesButton = b;
                else if (noButton == null && name.Contains("no")) noButton = b;
                else if (nextButton == null && name.Contains("next")) nextButton = b;
            }
        }
    }

    private void Start()
    {
        if (BiomeManager.Instance != null)
            currentBiomeIndex = BiomeManager.Instance.currentBiomeIndex;

        yesButton.onClick.AddListener(OnYesPressed);
        noButton.onClick.AddListener(OnNoPressed);
        nextButton.onClick.AddListener(OnNextPressed);

        // Hide all at start
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);

        ShowDeityDialogue();
    }

    public void ShowDeityDialogue()
    {
        // Always show intro first if not yet shown
        if (!introShown)
        {
            currentDialogue = new string[]
            {
                "Welcome, mortal, to my presence.",
                "I have heard of your curiosity.",
                "Complete your tasks: traverse every land to earn the first tale."
            };
            introShown = true;
            StartDialogueSequence(false);
            return;
        }

        // After intro, follow normal logic
        var diary = DiaryManager.Instance.diaryDatabase;
        int totalUnlocked = 0;
        for (int i = 0; i < diary.biomes.Length; i++)
        {
            if (DiaryManager.Instance.IsEntryUnlocked(i, 0))
                totalUnlocked++;
        }

        if (totalUnlocked == 0)
        {
            currentDialogue = new string[]
            {
                "You still have no tales unlocked. Venture forth to earn your first entry."
            };
            StartDialogueSequence(false);
            return;
        }

        if (totalUnlocked < diary.biomes.Length)
        {
            currentDialogue = new string[]
            {
                "Mortal, you have unlocked some tales, but there are still free secrets waiting in other lands.",
                "Venture forth to earn the first entry before seeking my paid knowledge."
            };
            StartDialogueSequence(false);
            return;
        }

        // Paid entries
        for (int b = 0; b < diary.biomes.Length; b++)
        {
            var entries = diary.biomes[b].entries;
            for (int e = 1; e < entries.Length; e++)
            {
                if (!DiaryManager.Instance.IsEntryUnlocked(b, e))
                {
                    int cost = entries[e].cost;
                    int playerCurrency = RelicCurrency.GetTotalCurrency();
                    currentBiomeIndex = b;

                    if (playerCurrency >= cost)
                    {
                        currentDialogue = new string[]
                        {
                            $"Ah, mortal, you seek the knowledge of {entries[e].title}.",
                            $"It shall cost you {cost} crystals. Do you dare pay the price?"
                        };
                        StartDialogueSequence(true);
                        return;
                    }
                    else
                    {
                        currentDialogue = new string[]
                        {
                            $"You desire {entries[e].title}, but you lack the {cost} crystals required.",
                            "Return when you have enough crystals to claim my wisdom."
                        };
                        StartDialogueSequence(false);
                        return;
                    }
                }
            }
        }

        // All entries unlocked
        currentDialogue = new string[]
        {
            "You have learned all I know... for now.",
            "Even I, the great one, hold no further secrets for you."
        };
        StartDialogueSequence(false);
    }

    private void StartDialogueSequence(bool showYesNoAtEnd)
    {
        dialogueIndex = 0;
        deityDialogueText.text = currentDialogue[dialogueIndex];

        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);

        nextButton.gameObject.SetActive(currentDialogue.Length > 1);
        pendingYesNo = showYesNoAtEnd;

        if (currentDialogue.Length == 1 && showYesNoAtEnd)
        {
            yesButton.gameObject.SetActive(true);
            noButton.gameObject.SetActive(true);
        }
    }

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
            else
            {
                // Allow dialogue to restart if player pressed next after No
                ShowDeityDialogue();
            }
        }
    }

    private void OnYesPressed()
    {
        bool unlocked = false;
        var entries = DiaryManager.Instance.diaryDatabase.biomes[currentBiomeIndex].entries;

        for (int e = 1; e < entries.Length; e++)
        {
            if (!DiaryManager.Instance.IsEntryUnlocked(currentBiomeIndex, e))
            {
                unlocked = DiaryManager.Instance.TryUnlockDiary(currentBiomeIndex, e);
                break;
            }
        }

        if (unlocked)
        {
            deityDialogueText.text = "It is done. The entry is now yours.";
            CurrencyUI.Instance?.UpdateCurrencyDisplay();

            if (HasNextAffordableEntry(currentBiomeIndex))
            {
                nextButton.gameObject.SetActive(true);
                pendingYesNo = true;
            }
            else
            {
                yesButton.gameObject.SetActive(false);
                noButton.gameObject.SetActive(false);
            }
        }
        else
        {
            deityDialogueText.text = "Return when you have enough crystals to claim more wisdom.";
            yesButton.gameObject.SetActive(false);
            noButton.gameObject.SetActive(false);

            // Let next button cycle dialogue again
            nextButton.gameObject.SetActive(true);
        }
    }

    private bool HasNextAffordableEntry(int biomeIndex)
    {
        var entries = DiaryManager.Instance.diaryDatabase.biomes[biomeIndex].entries;
        int playerCurrency = RelicCurrency.GetTotalCurrency();

        for (int e = 1; e < entries.Length; e++)
        {
            if (!DiaryManager.Instance.IsEntryUnlocked(biomeIndex, e) && playerCurrency >= entries[e].cost)
                return true;
        }
        return false;
    }

    private void OnNoPressed()
    {
        deityDialogueText.text = "Very well. Return when you are ready.";
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);

        // Let next button continue dialogue cycle
        nextButton.gameObject.SetActive(true);
    }
}
