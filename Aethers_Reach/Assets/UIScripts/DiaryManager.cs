using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class BiomeUI
{
    public Button tabButton;         // Tab button for this biome
    public Button[] entryButtons;    // 3 entry buttons
}

public class DiaryManager : MonoBehaviour
{
    public static DiaryManager Instance;

    [Header("Diary Data")]
    public DiaryDatabase diaryDatabase;

    [Header("UI Elements")]
    public BiomeUI[] biomesUI; // 0=Skylands, 1=Beach, 2=Ruins
    public Text contentText;

    [Header("Settings")]
    public float autoUnlockDelay = 2f;

    private bool[,] unlockedEntries; // [biomeIndex, entryIndex]

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeUnlocks();
            LoadProgress();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Hook up tab buttons
        for (int i = 0; i < biomesUI.Length; i++)
        {
            int index = i;
            biomesUI[i].tabButton.onClick.AddListener(() => SwitchBiome(index));
        }

        // Hook up entry buttons
        for (int b = 0; b < biomesUI.Length; b++)
        {
            for (int e = 0; e < biomesUI[b].entryButtons.Length; e++)
            {
                int biomeIndex = b;
                int entryIndex = e;
                biomesUI[b].entryButtons[e].onClick.AddListener(() => OnEntryClicked(biomeIndex, entryIndex));
            }
        }

        UpdateEntryButtons();
    }

    private void InitializeUnlocks()
    {
        int maxEntries = 0;
        foreach (var biome in diaryDatabase.biomes)
            maxEntries = Mathf.Max(maxEntries, biome.entries.Length);

        unlockedEntries = new bool[diaryDatabase.biomes.Length, maxEntries];
    }

    public void OnBiomeEntered(int biomeIndex)
    {
        StartCoroutine(AutoUnlockFirstEntry(biomeIndex));
    }

    private IEnumerator AutoUnlockFirstEntry(int biomeIndex)
    {
        yield return new WaitForSeconds(2f);

        // Unlock first entry if not unlocked
        if (!IsEntryUnlocked(biomeIndex, 0))
        {
            UnlockEntry(biomeIndex, 0);

            // Show notification
            if (PopUpManager.Instance != null)
            {
                string entryTitle = diaryDatabase.biomes[biomeIndex].entries[0].title;
                PopUpManager.Instance.ShowPopUp($"Diary Entry Unlocked: {entryTitle}");
            }
        }
    }

    public bool IsEntryUnlocked(int biomeIndex, int entryIndex)
    {
        if (biomeIndex < 0 || biomeIndex >= diaryDatabase.biomes.Length ||
            entryIndex < 0 || entryIndex >= diaryDatabase.biomes[biomeIndex].entries.Length)
            return false;

        return unlockedEntries[biomeIndex, entryIndex];
    }

    public bool UnlockEntry(int biomeIndex, int entryIndex)
    {
        if (IsEntryUnlocked(biomeIndex, entryIndex))
            return false;

        unlockedEntries[biomeIndex, entryIndex] = true;
        SaveProgress();
        UpdateEntryButtons();

        // Show pop-up
        if (PopUpManager.Instance != null)
            PopUpManager.Instance.ShowPopUp($"New Entry Unlocked!");

        return true;
    }

    public bool TryUnlockDiary(int biomeIndex, int entryIndex)
    {
        var entry = diaryDatabase.biomes[biomeIndex].entries[entryIndex];
        int playerCurrency = RelicCurrency.GetTotalCurrency();

        if (playerCurrency >= entry.cost)
        {
            RelicCurrency.SpendCurrency(entry.cost);
            return UnlockEntry(biomeIndex, entryIndex); // Reuse unlock method for popup
        }

        return false;
    }

    private void OnEntryClicked(int biomeIndex, int entryIndex)
    {
        if (!IsEntryUnlocked(biomeIndex, entryIndex))
        {
            contentText.text = "";
            return;
        }

        var entry = diaryDatabase.biomes[biomeIndex].entries[entryIndex];
        contentText.text = $"{entry.title}\n\n{entry.content}";
    }

    public void UpdateEntryButtons()
    {
        for (int b = 0; b < biomesUI.Length; b++)
        {
            if (biomesUI[b].entryButtons == null) continue;

            for (int e = 0; e < biomesUI[b].entryButtons.Length; e++)
            {
                if (biomesUI[b].entryButtons[e] != null)
                    biomesUI[b].entryButtons[e].interactable = IsEntryUnlocked(b, e);
            }
        }
    }

    public void SwitchBiome(int biomeIndex)
    {
        // Hide entries of other biomes
        for (int b = 0; b < biomesUI.Length; b++)
        {
            bool active = (b == biomeIndex);
            foreach (var btn in biomesUI[b].entryButtons)
                btn.gameObject.SetActive(active);
        }

        contentText.text = ""; // Clear content until player clicks an entry
    }

    private void SaveProgress()
    {
        for (int b = 0; b < diaryDatabase.biomes.Length; b++)
        {
            for (int e = 0; e < diaryDatabase.biomes[b].entries.Length; e++)
            {
                PlayerPrefs.SetInt($"DiaryEntry_{b}_{e}", unlockedEntries[b, e] ? 1 : 0);
            }
        }
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        for (int b = 0; b < diaryDatabase.biomes.Length; b++)
        {
            for (int e = 0; e < diaryDatabase.biomes[b].entries.Length; e++)
            {
                unlockedEntries[b, e] = PlayerPrefs.GetInt($"DiaryEntry_{b}_{e}", 0) == 1;
            }
        }
    }
}
