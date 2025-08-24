using UnityEngine;
using UnityEngine.UI;

public class DeityUIManager : MonoBehaviour
{
    public static DeityUIManager Instance;

    [Header("UI Elements")]
    public Text deityDialogueText;
    public Button viewDiaryButton;
    public Button leaveButton;
    public Button yesButton;
    public Button noButton;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ShowWelcomeDialogue();

        viewDiaryButton.onClick.AddListener(OpenDiary);
        leaveButton.onClick.AddListener(CloseDeityScreen);
        yesButton.onClick.AddListener(UnlockEntry);
        noButton.onClick.AddListener(DeclineUnlock);

        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
    }

    public void ShowWelcomeDialogue()
    {
        deityDialogueText.text = "Mortal… do you seek the knowledge of the diary?";
        viewDiaryButton.gameObject.SetActive(true);
        leaveButton.gameObject.SetActive(true);
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
    }

    private void OpenDiary()
    {
        DiaryUIManager.Instance.OpenDiaryScreen();
        this.gameObject.SetActive(false);
    }

    public void ReturnFromDiary()
    {
        this.gameObject.SetActive(true);

        int nextEntry = DiaryUnlockManager.Instance.GetUnlockedEntries();
        float cost = DiaryUnlockManager.Instance.GetEntryCost(nextEntry);

        if (nextEntry >= DiaryUnlockManager.Instance.totalEntries)
        {
            deityDialogueText.text = "You have unlocked all knowledge I hold.";
            yesButton.gameObject.SetActive(false);
            noButton.gameObject.SetActive(false);
        }
        else if (DiaryUnlockManager.Instance.playerCurrency >= cost)
        {
            deityDialogueText.text = $"You have returned… Do you wish to unlock Entry {nextEntry + 1} for {cost:F0} coins?";
            yesButton.gameObject.SetActive(true);
            noButton.gameObject.SetActive(true);
        }
        else
        {
            deityDialogueText.text = "Go away… you are too poor.";
            yesButton.gameObject.SetActive(false);
            noButton.gameObject.SetActive(false);
        }

        viewDiaryButton.gameObject.SetActive(false);
        leaveButton.gameObject.SetActive(true);
    }

    private void UnlockEntry()
    {
        int nextEntry = DiaryUnlockManager.Instance.GetUnlockedEntries();
        DiaryUnlockManager.Instance.TryUnlockDiary(nextEntry);

        deityDialogueText.text = "It is done. The knowledge is now yours.";

        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
    }

    private void DeclineUnlock()
    {
        deityDialogueText.text = "Very well. Return when you are ready.";
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
    }

    private void CloseDeityScreen()
    {
        this.gameObject.SetActive(false);
    }
}
