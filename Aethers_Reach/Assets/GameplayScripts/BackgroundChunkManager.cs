using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BackgroundChunkManager : MonoBehaviour
{
    public static BackgroundChunkManager Instance;

    [Header("Chunk Settings")]
    public GameObject[] chunkPrefabs;
    public Transform player;
    public CinemachineVirtualCamera virtualCamera;
    public int maxChunksOnScreen = 5;

    private readonly List<GameObject> spawnedChunks = new();
    private Transform lastChunkEnd;

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
        if (lastChunkEnd == null || virtualCamera == null) return;

        Vector3 viewportPos = Camera.main.WorldToViewportPoint(lastChunkEnd.position);

        if (viewportPos.x < 1f && viewportPos.x > 1f)
        {
            SpawnNextChunk();
        }
    }

    public void SpawnNextChunk()
    {
        GameObject prefab = chunkPrefabs[Random.Range(0, chunkPrefabs.Length)];

        GameObject newChunk = Instantiate(prefab);
        Transform newChunkEnd = newChunk.transform.Find("ChunkEnd");

        Vector3 spawnPosition = lastChunkEnd != null
            ? new Vector3(lastChunkEnd.position.x, 14f, 70f)
            : new Vector3(0, 14f, 70f);

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
