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
    private bool pendingYesNo; // Track if Yes/No button should appear

    private void Awake() => Instance = this;

    private void Start()
    {
        if (BiomeManager.Instance != null)
            currentBiomeIndex = BiomeManager.Instance.currentBiomeIndex;

        yesButton.onClick.AddListener(OnYesPressed);
        noButton.onClick.AddListener(OnNoPressed);
        nextButton.onClick.AddListener(OnNextPressed);

        ShowDeityDialogue();
    }

    public void ShowDeityDialogue()
    {
        var diary = DiaryManager.Instance.diaryDatabase;
        viewDiaryButton.gameObject.SetActive(true);

        //count how many entries are unlocked across all biomes
        int totalUnlocked = 0;
        bool[] freeUnlocked = new bool[diary.biomes.Length];
        for (int i = 0; i < diary.biomes.Length; i++)
        {
            freeUnlocked[i] = DiaryManager.Instance.IsEntryUnlocked(i, 0);
            if (DiaryManager.Instance.IsEntryUnlocked(i, 0)) totalUnlocked++;
        }

        //wlcome dialogue if nothing unlocked
        if (totalUnlocked == 0)
        {
            currentDialogue = new string[]
            {
                "Welcome, mortal... You hold the diary for the first time.",
                "I have heard of your curiosity.",
                "Complete your tasks: traverse every land to earn the first tale.",
                //"Bring me the crystals you collect on your journey, and I shall share my knowledge."
            };
            StartDialogueSequence(false);
            return;
        }

        //free entries still missing
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

        //all free entries unlocked → offer paid entries
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

        //All entries unlocked
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

        //unlock  first unpaid entry the player can afford
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

            if (CurrencyUI.Instance != null)
                CurrencyUI.Instance.UpdateCurrencyDisplay();

            // check if another entry is affordable
            if (HasNextAffordableEntry(currentBiomeIndex))
            {
                nextButton.gameObject.SetActive(true);
                pendingYesNo = true; // Keep Yesno active for next entry
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


    private void ContinueOfferingNextEntry(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= ContinueOfferingNextEntry;

        //rerun deity dialogue for current biome
        ShowDeityDialogue();
    }


    private void OnNoPressed()
    {
        deityDialogueText.text = "Very well. Return when you are ready.";
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
    }

    public void OnDeityButtonPressed() => ShowDeityDialogue();
    public void OnBackToMMPressed()
    {
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
    }
}
