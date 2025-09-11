using UnityEngine;

public class DiaryUnlockManager : MonoBehaviour
{
    public static DiaryUnlockManager Instance;

    [Header("Diary Data")]
    public DiaryDatabase diaryDatabase;

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

    private void InitializeUnlocks()
    {
        if (diaryDatabase == null)
        {
            Debug.LogError("DiaryDatabase is not assigned in DiaryUnlockManager!");
            return;
        }

        int biomeCount = diaryDatabase.biomes.Length;
        unlockedEntries = new bool[biomeCount, 3]; // Assuming 3 entries per biome
    }

    /// <summary>
    /// Check if an entry is unlocked
    /// </summary>
    public bool IsEntryUnlocked(int biomeIndex, int entryIndex)
    {
        if (!IsValidEntry(biomeIndex, entryIndex))
            return false;

        return unlockedEntries[biomeIndex, entryIndex];
    }

    /// <summary>
    /// Try unlocking an entry if player has enough relics
    /// </summary>
    public bool TryUnlockDiary(int biomeIndex, int entryIndex)
    {
        if (!IsValidEntry(biomeIndex, entryIndex))
            return false;

        DiaryEntry entry = diaryDatabase.biomes[biomeIndex].entries[entryIndex];
        int cost = entry.cost;
        int playerCurrency = RelicCurrency.GetTotalCurrency();

        if (playerCurrency >= cost)
        {
            RelicCurrency.SpendCurrency(cost);
            unlockedEntries[biomeIndex, entryIndex] = true;
            SaveProgress();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Unlock an entry for free (first biome entry reward)
    /// </summary>
    public void UnlockFreeEntry(int biomeIndex, int entryIndex)
    {
        if (IsValidEntry(biomeIndex, entryIndex))
        {
            unlockedEntries[biomeIndex, entryIndex] = true;
            SaveProgress();
        }
    }

    private bool IsValidEntry(int biomeIndex, int entryIndex)
    {
        return biomeIndex >= 0 && biomeIndex < diaryDatabase.biomes.Length &&
               entryIndex >= 0 && entryIndex < diaryDatabase.biomes[biomeIndex].entries.Length;
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
