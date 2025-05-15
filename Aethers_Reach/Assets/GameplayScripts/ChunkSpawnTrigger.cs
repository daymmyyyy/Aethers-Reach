using UnityEngine;

public class ChunkSpawnTrigger : MonoBehaviour
{
    private bool hasSpawned = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasSpawned) return;

        if (other.CompareTag("Player"))
        {
            ChunkManager.Instance.SpawnNextChunk();
            hasSpawned = true;
        }
    }
}
