using UnityEngine;

public class DiaryUnlockManager : MonoBehaviour
{
    public static DiaryUnlockManager Instance;

    [Header("Diary Settings")]
    public int totalEntries = 10;          // total diary entries
    public int baseCost = 2000000;         // starting relic cost
    public int costIncreasePerEntry = 500000; // how much more each new unlock costs

    private int unlockedEntries = 0;       // how many diary entries are unlocked

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadProgress();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //returns how many entries the player has unlocked
    public int GetUnlockedEntries()
    {
        return unlockedEntries;
    }

    //returns cost for unlocking the next diary entry
    public int GetEntryCost(int entryIndex)
    {
        return baseCost + (entryIndex * costIncreasePerEntry);
    }

    //attempts to unlock a diary entry if player has enough relics
    public bool TryUnlockDiary(int entryIndex)
    {
        int cost = GetEntryCost(entryIndex);
        int playerCurrency = RelicCurrency.GetTotalCurrency();

        if (playerCurrency >= cost)
        {
            RelicCurrency.SpendCurrency(cost);
            unlockedEntries++;
            SaveProgress();
            return true;
        }
        return false;
    }

    //Save progress to PlayerPrefs
    private void SaveProgress()
    {
        PlayerPrefs.SetInt("UnlockedEntries", unlockedEntries);
        PlayerPrefs.Save();
    }

    //load progress from PlayerPrefs
    private void LoadProgress()
    {
        unlockedEntries = PlayerPrefs.GetInt("UnlockedEntries", 0);
    }
}
