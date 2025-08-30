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

    [Header("Portal Settings")]
    public GameObject portalChunkPrefab;
    public string portalTag = "PortalChunk";
    public int chunksBetweenPortals = 3;

    private readonly List<GameObject> spawnedChunks = new();
    private Transform lastChunkEnd;

    private GameObject currentPortal;
    private int fillerChunksSincePortal = 0;
    private List<int> usedFillerIndices = new List<int>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        SpawnNextChunk();
    }

    private void Update()
    {
        if (lastChunkEnd == null || virtualCamera == null || player == null) return;

        Vector3 viewportPos = Camera.main.WorldToViewportPoint(lastChunkEnd.position);
        if (viewportPos.x < 1.1f && viewportPos.x > 1f)
        {
            SpawnNextChunk();
        }
    }

    public void SpawnNextChunk()
    {
        GameObject prefab;

        // Only spawn portal if player has collected at least 5 relic pieces
        bool canSpawnPortal = RelicManager.Instance != null &&
                              RelicManager.Instance.HasCompletedRelic() &&
                              currentPortal == null &&
                              fillerChunksSincePortal >= chunksBetweenPortals;

        if (canSpawnPortal)
        {
            prefab = portalChunkPrefab;
            fillerChunksSincePortal = 0;
            Debug.Log("Spawning portal chunk!");
        }
        else
        {
            // Spawn a normal filler chunk
            if (usedFillerIndices.Count >= chunkPrefabs.Length)
                usedFillerIndices.Clear();

            int index;
            do
            {
                index = Random.Range(0, chunkPrefabs.Length);
            } while (usedFillerIndices.Contains(index));

            usedFillerIndices.Add(index);
            prefab = chunkPrefabs[index];
            fillerChunksSincePortal++;
        }

        // Instantiate chunk
        GameObject newChunk = Instantiate(prefab);

        // Position chunk
        Transform newChunkEnd = newChunk.transform.Find("ChunkEnd");
        if (newChunkEnd == null) newChunkEnd = newChunk.transform;

        Vector3 spawnPosition = lastChunkEnd != null
            ? new Vector3(lastChunkEnd.position.x, 14f, 50f)
            : new Vector3(0, 14f, 50f);

        newChunk.transform.position = spawnPosition;

        lastChunkEnd = newChunkEnd;
        spawnedChunks.Add(newChunk);

        CleanupOldChunks();
    }

    private void CleanupOldChunks()
    {
        while (spawnedChunks.Count > maxChunksOnScreen)
        {
            Destroy(spawnedChunks[0]);
            spawnedChunks.RemoveAt(0);
        }
    }
}
