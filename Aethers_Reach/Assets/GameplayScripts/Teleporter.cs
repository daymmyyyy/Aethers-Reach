using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleporter : MonoBehaviour
{
    [Header("Target Scene Name")]
    public string targetSceneName;

    private GameObject player;
    private PlayerController playerController;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerController == null)
            {
                playerController = other.GetComponent<PlayerController>();
            }

            if (playerController != null)
            {
                float multiplier = playerController.distanceMultiplier;
                Vector3 position = playerController.startPosition;

                GameManager.Instance.SaveProgressBeforeSceneChange(
                    Vector3.Distance(position, other.transform.position) * multiplier
                );

                SceneManager.LoadScene(targetSceneName);
            }
            else
            {
                Debug.LogError("PlayerController not found on player object.");
            }
        }
    }

}
