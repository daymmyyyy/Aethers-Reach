using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleporter : MonoBehaviour
{
    [Header("Target Scene Name")]
    public string targetSceneName;

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

                GameManager.Instance.SaveProgressBeforeSceneChange(
                    Vector3.Distance(position, other.transform.position) * multiplier
                );
            }

            // Load target scene
            if (!string.IsNullOrEmpty(targetSceneName))
                SceneManager.LoadScene(targetSceneName);
        }
    }
}
