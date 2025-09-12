using UnityEngine;

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
        }
    }

    public void SetCurrentBiome(int index)
    {
        currentBiomeIndex = index;
    }
}
