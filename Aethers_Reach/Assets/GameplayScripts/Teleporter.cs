using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleporter : MonoBehaviour
{
    [Header("Target Scene Name")]
    public string targetSceneName;

    [Header("Biome Index for Target Scene")]
    [Tooltip("0 = Skylands, 1 = Beach, 2 = Ruins")]
    public int targetBiomeIndex = 0;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Save progress before scene change
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null && GameManager.Instance != null)
            {
                float multiplier = playerController.distanceMultiplier;
                Vector3 position = playerController.lastPosition;

                GameManager.Instance.cameFromMainMenu = false;

                GameManager.Instance.SaveProgressBeforeSceneChange(
                    Vector3.Distance(position, other.transform.position) * multiplier
                );
            }

            // Set biome index
            if (BiomeManager.Instance != null)
                BiomeManager.Instance.SetCurrentBiome(targetBiomeIndex);

            // Load target scene
            if (!string.IsNullOrEmpty(targetSceneName))
                SceneManager.LoadScene(targetSceneName);
        }
    }
}
