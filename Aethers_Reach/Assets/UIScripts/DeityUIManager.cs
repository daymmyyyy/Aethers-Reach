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

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ShowWelcomeDialogue();

        yesButton.onClick.AddListener(UnlockEntry);
        noButton.onClick.AddListener(DeclineUnlock);
    }

    public void ShowWelcomeDialogue()
    {
        deityDialogueText.text = "Mortal… do you seek the knowledge of the diary?";
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);

        viewDiaryButton.gameObject.SetActive(true);
    }

    public void ReturnFromDiary()
    {
        int nextEntry = DiaryUnlockManager.Instance.GetUnlockedEntries();
        int totalEntries = DiaryUnlockManager.Instance.totalEntries;
        int cost = DiaryUnlockManager.Instance.GetEntryCost(nextEntry);

        int playerCurrency = RelicCurrency.GetTotalCurrency();

        if (nextEntry >= totalEntries)
        {
            deityDialogueText.text = "You have unlocked all the knowledge I hold.";
            yesButton.gameObject.SetActive(false);
            noButton.gameObject.SetActive(false);
        }
        else if (playerCurrency >= cost)
        {
            deityDialogueText.text =
                $"You have returned… Do you wish to unlock Entry {nextEntry + 1} for {cost} relics?";
            yesButton.gameObject.SetActive(true);
            noButton.gameObject.SetActive(true);
        }
        else
        {
            deityDialogueText.text =
                $"Go away… you are too poor. Return with {cost} relics.";
            yesButton.gameObject.SetActive(false);
            noButton.gameObject.SetActive(false);
        }
    }

    private void UnlockEntry()
    {
        int nextEntry = DiaryUnlockManager.Instance.GetUnlockedEntries();

        bool unlocked = DiaryUnlockManager.Instance.TryUnlockDiary(nextEntry);

        if (unlocked)
        {
            deityDialogueText.text = $"It is done. Diary Entry {nextEntry + 1} is now yours.";
        }
        else
        {
            deityDialogueText.text = "You dare mock me? You lack the relics!";
        }

        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
    }

    private void DeclineUnlock()
    {
        deityDialogueText.text = "Very well. Return when you are ready.";
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
    }

    public void OnBackToMMPressed()
    {
        // hide Yes/No while on main menu
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
    }

    public void OnDeityButtonPressed()
    {
        ReturnFromDiary();
    }

}
