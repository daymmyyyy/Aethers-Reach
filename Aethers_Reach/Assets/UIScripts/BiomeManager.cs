using UnityEngine;
using UnityEngine.SceneManagement;

public class BiomeManager : MonoBehaviour
{
    public static BiomeManager Instance;

    [Header("Biome Info")]
    [Tooltip("0 = Skylands, 1 = Beach, 2 = Ruins")]
    public int currentBiomeIndex = 0;


private void Awake()
{
    if (Instance == null)
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    else
    {
        Destroy(gameObject);
        return;
    }

    string sceneName = SceneManager.GetActiveScene().name;

    switch (sceneName)
    {
        case "Biome1": currentBiomeIndex = 0; break;
        case "Biome2": currentBiomeIndex = 1; break;
        case "Biome3": currentBiomeIndex = 2; break;
        default: currentBiomeIndex = -1; break; // For Main Menu or misc scenes
    }
}

    public void SetCurrentBiome(int index)
    {
        currentBiomeIndex = index;
    }
}
