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

    private const string INTRO_KEY = "DeityIntroRead"; // PlayerPrefs key to track intro

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
        if (deityDialogueText == null)
        {
            Debug.LogError("Cannot show dialogue: deityDialogueText is null!");
            return;
        }

        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);

        viewDiaryButton.gameObject.SetActive(true);

        bool introRead = PlayerPrefs.GetInt(INTRO_KEY, 0) == 1;

        if (!introRead)
        {
            // Show intro only once
            currentDialogue = new string[]
            {
                "Welcome, traveler. What wisdom do you seek?",
                "This diary contains knowledge of all lands. Seek and learn."
            };
            StartDialogueSequence(false);
            PlayerPrefs.SetInt(INTRO_KEY, 1);
            PlayerPrefs.Save();
            return;
        }

        var diary = DiaryManager.Instance.diaryDatabase;
        int totalUnlocked = 0;

        for (int i = 0; i < diary.biomes.Length; i++)
        {
            if (DiaryManager.Instance.IsEntryUnlocked(i, 0))
                totalUnlocked++;
        }

        // If free entries still available
        if (totalUnlocked < diary.biomes.Length)
        {
            currentDialogue = new string[]
            {
                "Mortal, you have unlocked some tales, but free secrets remain in other lands.",
                "Venture forth to earn the first entry before seeking my paid knowledge."
            };
            StartDialogueSequence(false);
            return;
        }

        // All free entries unlocked
        for (int b = 0; b < diary.biomes.Length; b++)
        {
            for (int e = 1; e < diary.biomes[b].entries.Length; e++)
            {
                if (!DiaryManager.Instance.IsEntryUnlocked(b, e))
                {
                    int cost = diary.biomes[b].entries[e].cost;
                    int playerCurrency = RelicCurrency.GetTotalCurrency();
                    currentBiomeIndex = b;

                    if (playerCurrency >= cost)
                    {
                        currentDialogue = new string[]
                        {
                            $"Ah, mortal, you seek the knowledge of {diary.biomes[b].entries[e].title}.",
                            $"It shall cost you {cost} crystals. Do you dare pay the price?"
                        };
                        StartDialogueSequence(true);
                        return;
                    }
                    else
                    {
                        currentDialogue = new string[]
                        {
                            $"You desire {diary.biomes[b].entries[e].title}, but you lack the {cost} crystals required.",
                            "Return when you have enough crystals to claim my wisdom."
                        };
                        StartDialogueSequence(false);
                        return;
                    }
                }
            }
        }

        // Everything unlocked
        currentDialogue = new string[]
        {
            "You have unlocked all free knowledge.",
            "Do you wish to purchase the remaining entries?"
        };
        StartDialogueSequence(true);
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
        nextButton.gameObject.SetActive(true); // Allow cycling dialogue again
    }
}
