using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ChunkManager : MonoBehaviour
{
    public static ChunkManager Instance;

    [Header("Chunk Settings")]
    public GameObject[] chunkPrefabs;
    public Transform player;
    public CinemachineVirtualCamera virtualCamera;
    public int maxChunksOnScreen = 5;

    private List<GameObject> spawnedChunks = new List<GameObject>();
    private Transform lastChunkEnd;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

  void Start()
  {
        SpawnNextChunk();
  }

    void Update()
    {
        if (lastChunkEnd == null || virtualCamera == null) return;

        // Get the viewport position of the ChunkEnd (0 = left edge of screen, 1 = right edge)
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(lastChunkEnd.position);

        if (viewportPos.x < 1.1f && viewportPos.x > 1f)
        {
            SpawnNextChunk();
        }
    }

    public void SpawnNextChunk()
    {
        GameObject prefab = chunkPrefabs[Random.Range(0, chunkPrefabs.Length)];
        GameObject newChunk = Instantiate(prefab);

        Transform newChunkStart = newChunk.transform.Find("ChunkStart");
        Transform newChunkEnd = newChunk.transform.Find("ChunkEnd");

        if (newChunkStart == null || newChunkEnd == null)
        {
            Debug.LogError("Chunk missing 'ChunkStart' or 'ChunkEnd'");
            return;
        }

        if (lastChunkEnd != null)
        {
            Vector3 spawnPos = new Vector3(
                lastChunkEnd.position.x,
                14f,
                lastChunkEnd.position.z
            );
            newChunk.transform.position = spawnPos;
        }
        else
        {
            newChunk.transform.position = new Vector3(0, 14f, 0);
        }

        // Check if relic is completed and disable relics in the new chunk
        if (RelicManager.Instance != null && RelicManager.Instance.HasCompletedRelic())
        {
            // Disable all relics (by tag or child name)
            foreach (Transform child in newChunk.GetComponentsInChildren<Transform>())
            {
                if (child.CompareTag("RelicPiece"))
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        lastChunkEnd = newChunkEnd;
        spawnedChunks.Add(newChunk);
        CleanupOldChunks();
    }




    void CleanupOldChunks()
    {
        if (spawnedChunks.Count > maxChunksOnScreen)
        {
            GameObject oldChunk = spawnedChunks[0];
            spawnedChunks.RemoveAt(0);
            Destroy(oldChunk);
        }
    }

    private Transform GetChildWithTag(Transform parent, string tag)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>())
        {
            if (child.CompareTag(tag))
                return child;
        }
        return null;
    }

}
