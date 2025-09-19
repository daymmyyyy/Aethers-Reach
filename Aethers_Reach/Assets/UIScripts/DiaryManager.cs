using UnityEngine;

[System.Serializable]
public class DiaryEntry
{
    public string title;
    public string content;
    public int cost; // For paid entries
}

public class DiaryManager : MonoBehaviour
{
    public static DiaryManager Instance;

    [Header("Data")]
    public DiaryDatabase diaryDatabase;

    [Header("Settings")]
    public float autoUnlockDelay = 2f;

    private bool[,] unlockedEntries;
    private bool[] freePopupShown;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeArrays();
            LoadProgress();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeArrays()
    {
        if (diaryDatabase == null || diaryDatabase.biomes == null)
        {
            unlockedEntries = new bool[0, 0];
            freePopupShown = new bool[0];
            Debug.LogError("DiaryManager: DiaryDatabase not assigned or empty.");
            return;
        }

        int biomeCount = diaryDatabase.biomes.Length;
        int maxEntries = 0;
        foreach (var biome in diaryDatabase.biomes)
            maxEntries = Mathf.Max(maxEntries, biome.entries.Length);

        unlockedEntries = new bool[biomeCount, maxEntries];
        freePopupShown = new bool[biomeCount];
    }

    // ------------------ Unlocking ------------------
    public void OnBiomeEntered(int biomeIndex)
    {
        if (!IsValidBiome(biomeIndex)) return;

        if (!freePopupShown[biomeIndex] && !IsEntryUnlocked(biomeIndex, 0))
        {
            StartCoroutine(AutoUnlockFirstEntry(biomeIndex));
        }
    }

    private System.Collections.IEnumerator AutoUnlockFirstEntry(int biomeIndex)
    {
        yield return new WaitForSeconds(autoUnlockDelay);

        if (!IsEntryUnlocked(biomeIndex, 0))
        {
            freePopupShown[biomeIndex] = true;
            PlayerPrefs.SetInt($"Diary_FreePopup_{biomeIndex}", 1);
            PlayerPrefs.Save();

            UnlockEntry(biomeIndex, 0, true);
        }
    }

    public bool IsEntryUnlocked(int biomeIndex, int entryIndex)
    {
        if (!IsValidEntry(biomeIndex, entryIndex)) return false;
        return unlockedEntries[biomeIndex, entryIndex];
    }

    public bool TryUnlockPaidEntry(int biomeIndex, int entryIndex)
    {
        if (!IsValidEntry(biomeIndex, entryIndex)) return false;
        if (entryIndex == 0) return false;

        if (!IsEntryUnlocked(biomeIndex, entryIndex - 1)) return false;

        var entry = diaryDatabase.biomes[biomeIndex].entries[entryIndex];
        int playerCurrency = RelicCurrency.GetTotalCurrency();

        if (playerCurrency >= entry.cost)
        {
            RelicCurrency.SpendCurrency(entry.cost);
            UnlockEntry(biomeIndex, entryIndex, false);
            return true;
        }

        return false;
    }

    private void UnlockEntry(int biomeIndex, int entryIndex, bool isFree)
    {
        if (!IsValidEntry(biomeIndex, entryIndex)) return;
        if (unlockedEntries[biomeIndex, entryIndex]) return;

        unlockedEntries[biomeIndex, entryIndex] = true;
        SaveProgress();

        if (isFree)
            PopUpManager.Instance?.ShowPopUp("New Entry Unlocked!");
        else
            PopUpManager.Instance?.ShowPopUp("Entry Unlocked!");

        Debug.Log($"DiaryManager: Unlocked [{biomeIndex},{entryIndex}] (free={isFree})");
    }

    // ------------------ Persistence ------------------
    public void SaveProgress()
    {
        if (diaryDatabase == null) return;

        for (int b = 0; b < diaryDatabase.biomes.Length; b++)
        {
            for (int e = 0; e < diaryDatabase.biomes[b].entries.Length; e++)
            {
                PlayerPrefs.SetInt($"DiaryEntry_{b}_{e}", IsEntryUnlocked(b, e) ? 1 : 0);
            }
            PlayerPrefs.SetInt($"Diary_FreePopup_{b}", freePopupShown[b] ? 1 : 0);
        }

        PlayerPrefs.Save();
    }

    public void LoadProgress()
    {
        if (diaryDatabase == null) return;

        for (int b = 0; b < diaryDatabase.biomes.Length; b++)
        {
            for (int e = 0; e < diaryDatabase.biomes[b].entries.Length; e++)
                unlockedEntries[b, e] = PlayerPrefs.GetInt($"DiaryEntry_{b}_{e}", 0) == 1;

            freePopupShown[b] = PlayerPrefs.GetInt($"Diary_FreePopup_{b}", 0) == 1;
        }
    }

    // ------------------ Helpers ------------------
    public DiaryEntryData GetDiaryEntry(int biomeIndex, int entryIndex)
    {
        if (!IsValidEntry(biomeIndex, entryIndex)) return null;
        return diaryDatabase.biomes[biomeIndex].entries[entryIndex];
    }

    private bool IsValidBiome(int biomeIndex)
    {
        return diaryDatabase != null && diaryDatabase.biomes != null &&
               biomeIndex >= 0 && biomeIndex < diaryDatabase.biomes.Length;
    }

    private bool IsValidEntry(int biomeIndex, int entryIndex)
    {
        return IsValidBiome(biomeIndex) &&
               entryIndex >= 0 &&
               entryIndex < diaryDatabase.biomes[biomeIndex].entries.Length;
    }
}
