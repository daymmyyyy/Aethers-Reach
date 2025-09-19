using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BiomeUI
{
    public Button tabButton;
    public Button[] entryButtons;
}

public class DiaryUIManager : MonoBehaviour
{
    public BiomeUI[] biomesUI;
    public Text titleText;
    public Text contentText;

    private int currentBiomeIndex = 0;

    private void Start()
    {
        // Clear title/content at start
        titleText.text = "";
        contentText.text = "";

        // Hook up tab buttons and entry buttons
        for (int i = 0; i < biomesUI.Length; i++)
        {
            int biomeIndex = i;
            if (biomesUI[i].tabButton != null)
                biomesUI[i].tabButton.onClick.AddListener(() => SwitchBiome(biomeIndex));

            if (biomesUI[i].entryButtons == null) continue;
            for (int e = 0; e < biomesUI[i].entryButtons.Length; e++)
            {
                int entryIndex = e;
                var btn = biomesUI[i].entryButtons[e];
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnEntryClicked(biomeIndex, entryIndex));
            }
        }

        SwitchBiome(0);
    }

    private void SwitchBiome(int biomeIndex)
    {
        currentBiomeIndex = biomeIndex;

        for (int b = 0; b < biomesUI.Length; b++)
        {
            bool active = (b == biomeIndex);
            foreach (var btn in biomesUI[b].entryButtons)
                btn.gameObject.SetActive(active);
        }

        titleText.text = "";
        contentText.text = "";

        UpdateEntryButtons();
    }

    private void UpdateEntryButtons()
    {
        for (int b = 0; b < biomesUI.Length; b++)
        {
            for (int e = 0; e < biomesUI[b].entryButtons.Length; e++)
            {
                var btn = biomesUI[b].entryButtons[e];
                btn.interactable = DiaryManager.Instance.IsEntryUnlocked(b, e);
            }
        }
    }

    private void OnEntryClicked(int biomeIndex, int entryIndex)
    {
        if (!DiaryManager.Instance.IsEntryUnlocked(biomeIndex, entryIndex)) return;

        var entry = DiaryManager.Instance.GetDiaryEntry(biomeIndex, entryIndex);
        if (entry == null) return;

        titleText.text = entry.title;
        contentText.text = entry.content;
    }
}
