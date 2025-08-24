using UnityEngine;

public class DiaryUnlockManager : MonoBehaviour
{
    public static DiaryUnlockManager Instance;

    [Header("Diary Settings")]
    public int totalEntries = 10;      // total number of diary entries in your game
    public float baseCost = 2000000f;  // starting cost for first entry
    public float costMultiplier = 2f;  // how much each entry costs compared to the last

    [Header("Player State")]
    public float playerCurrency = 0f;  // current coins the player has
    private int unlockedEntries = 0;   // how many diary entries are unlocked

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

    public void AddCurrency(float amount)
    {
        playerCurrency += amount;
        SaveProgress();
    }

    //try to unlock the next diary entry
    public bool TryUnlockDiary(int entryIndex)
    {
        float cost = GetEntryCost(entryIndex);

        if (playerCurrency >= cost && entryIndex == unlockedEntries)
        {
            playerCurrency -= cost;
            unlockedEntries++;
            SaveProgress();
            return true; // successfully unlocked
        }

        return false; // failed to unlock
    }

    //how many entries are unlocked
    public int GetUnlockedEntries()
    {
        return unlockedEntries;
    }

    //get the cost for unlocking a specific entry
    public float GetEntryCost(int index)
    {
        return baseCost * Mathf.Pow(costMultiplier, index);
    }

    //save player progress (currency + unlocked entries)
    private void SaveProgress()
    {
        PlayerPrefs.SetFloat("PlayerCurrency", playerCurrency);
        PlayerPrefs.SetInt("UnlockedEntries", unlockedEntries);
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        playerCurrency = PlayerPrefs.GetFloat("PlayerCurrency", 0f);
        unlockedEntries = PlayerPrefs.GetInt("UnlockedEntries", 0);
    }

    //only for testing
    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey("PlayerCurrency");
        PlayerPrefs.DeleteKey("UnlockedEntries");
        playerCurrency = 0;
        unlockedEntries = 0;
    }
}
