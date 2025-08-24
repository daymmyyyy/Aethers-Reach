using UnityEngine;
using UnityEngine.UI;

public class DiaryUIManager : MonoBehaviour
{
    public static DiaryUIManager Instance;

    public GameObject diaryScreen;
    public Button backButton;
    public Text[] entryTexts; // assign diary entry text slots in Inspector

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        diaryScreen.SetActive(false);
        backButton.onClick.AddListener(ReturnToDeity);
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
                entryTexts[i].text = $"Entry {i + 1}: [Unlocked story here]";
            else
                entryTexts[i].text = $"Entry {i + 1}: [Locked]";
        }
    }

    private void ReturnToDeity()
    {
        diaryScreen.SetActive(false);
        DeityUIManager.Instance.ReturnFromDiary();
    }
}
