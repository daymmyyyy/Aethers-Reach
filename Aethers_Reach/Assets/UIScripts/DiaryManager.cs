using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class BiomeUI
{
    public Button tabButton;         // Tab button for this biome
    public Button[] entryButtons;    // 3 entry buttons
}

[System.Serializable]
public class DiaryEntryData
{
    public string title;
    public string content; // Added content text
    public int cost;
}

[System.Serializable]
public class BiomeData
{
    public DiaryEntryData[] entries;
}

[CreateAssetMenu(fileName = "DiaryDatabase", menuName = "Diary/Database")]
public class DiaryDatabase : ScriptableObject
{
    public BiomeData[] biomes;
}

public class DiaryManager : MonoBehaviour
{
    public static DiaryManager Instance;

    [Header("Diary Data")]
    public DiaryDatabase diaryDatabase;

    [Header("UI Elements")]
    public BiomeUI[] biomesUI; // 0=Skylands, 1=Beach, 2=Ruins
    public Text contentText;   // Display content of selected diary entry

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
        // Hook up tab buttons to switch biomes
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
        unlockedEntries = new bool[diaryDatabase.biomes.Length, 3]; // 3 entries per biome
    }

    public void OnBiomeEntered(int biomeIndex)
    {
        StartCoroutine(AutoUnlockFirstEntry(biomeIndex));
    }

    private IEnumerator AutoUnlockFirstEntry(int biomeIndex)
    {
        yield return new WaitForSeconds(autoUnlockDelay);

        if (!IsEntryUnlocked(biomeIndex, 0))
        {
            unlockedEntries[biomeIndex, 0] = true;
            SaveProgress();
            UpdateEntryButtons();

            if (PopUpManager.Instance != null)
                PopUpManager.Instance.ShowPopUp($"New Entry Unlocked!");
        }
    }

    public bool IsEntryUnlocked(int biomeIndex, int entryIndex)
    {
        if (biomeIndex < 0 || biomeIndex >= diaryDatabase.biomes.Length ||
            entryIndex < 0 || entryIndex >= diaryDatabase.biomes[biomeIndex].entries.Length)
            return false;

        return unlockedEntries[biomeIndex, entryIndex];
    }

    public bool TryUnlockDiary(int biomeIndex, int entryIndex)
    {
        if (IsEntryUnlocked(biomeIndex, entryIndex))
            return false; // Already unlocked

        var entry = diaryDatabase.biomes[biomeIndex].entries[entryIndex];
        int playerCurrency = RelicCurrency.GetTotalCurrency();

        if (playerCurrency >= entry.cost)
        {
            RelicCurrency.SpendCurrency(entry.cost);
            unlockedEntries[biomeIndex, entryIndex] = true;
            SaveProgress();
            UpdateEntryButtons();

            if (PopUpManager.Instance != null)
                PopUpManager.Instance.ShowPopUp($"New Entry Unlocked!");

            return true;
        }

        return false;
    }

    private void OnEntryClicked(int biomeIndex, int entryIndex)
    {
        if (!IsEntryUnlocked(biomeIndex, entryIndex))
        {
            contentText.text = ""; // Locked entry shows nothing
            return;
        }

        var entry = diaryDatabase.biomes[biomeIndex].entries[entryIndex];
        contentText.text = $"{entry.title}\n\n{entry.content}";
    }

    public void UpdateEntryButtons()
    {
        for (int b = 0; b < biomesUI.Length; b++)
        {
            for (int e = 0; e < biomesUI[b].entryButtons.Length; e++)
            {
                biomesUI[b].entryButtons[e].interactable = IsEntryUnlocked(b, e);
            }
        }
    }

    public void SwitchBiome(int biomeIndex)
    {
        // Hide entries from other biomes
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
