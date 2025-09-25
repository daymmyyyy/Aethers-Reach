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
    public Image contentImage;

    private int currentBiomeIndex = 0;

    private void Start()
    {
        // Clear title/content at start
        contentImage.sprite = null;

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

        contentImage.sprite = null;

        Color newColor = contentImage.color;
        newColor.a = 0f;
        contentImage.color = newColor;

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

        contentImage.sprite = entry.content;

        Color newColor = contentImage.color;
        newColor.a = 1f;
        contentImage.color = newColor;
    }
}
