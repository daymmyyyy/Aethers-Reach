using UnityEngine;
using UnityEngine.UI;

public class DiaryUIManager : MonoBehaviour
{
    public static DiaryUIManager Instance;

    [Header("UI Elements")]
    public GameObject diaryScreen;
    public Button backButton;
    public Button[] entryButtons;   // buttons for each entry
    public Text[] entryTexts;       // text display for each entry

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        backButton.onClick.AddListener(ReturnToDeity);

        // Add listeners to each button
        for (int i = 0; i < entryButtons.Length; i++)
        {
            int index = i; // capture index for closure
            entryButtons[i].onClick.AddListener(() => OpenEntry(index));
        }
    }

    public void OpenDiaryScreen()
    {
        diaryScreen.SetActive(true);
        RefreshDiary();
    }

    private void RefreshDiary()
    {
        int unlocked = DiaryUnlockManager.Instance.GetUnlockedEntries();

        for (int i = 0; i < entryTexts.Length; i++)
        {
            if (i < unlocked)
            {
                entryTexts[i].text = $"Entry {i + 1}: [Unlocked story here]";
                entryButtons[i].interactable = true;
                var colors = entryButtons[i].colors;
                colors.normalColor = Color.white;
                colors.disabledColor = Color.white;
                entryButtons[i].colors = colors;
            }
            else
            {
                entryTexts[i].text = $"Entry {i + 1}: [Locked]";
                entryButtons[i].interactable = false;
                var colors = entryButtons[i].colors;
                colors.normalColor = Color.gray;
                colors.disabledColor = Color.gray;
                entryButtons[i].colors = colors;
            }
        }
    }

    private void OpenEntry(int index)
    {
        int unlocked = DiaryUnlockManager.Instance.GetUnlockedEntries();
        if (index < unlocked)
        {
            entryTexts[index].text = $"Entry {index + 1}: [Full unlocked story here]";
        }
        else
        {
            // optional: feedback for locked entry
            Debug.Log("Entry is locked!");
        }
    }

    private void ReturnToDeity()
    {
        diaryScreen.SetActive(false);
        DeityUIManager.Instance.ReturnFromDiary();
    }
}
