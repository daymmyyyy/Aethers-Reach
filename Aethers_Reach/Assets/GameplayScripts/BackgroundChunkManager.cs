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
    [SerializeField] private Transform firstManualChunk;
    private Transform lastChunkEnd;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (firstManualChunk != null)
        {
            Transform manualChunkEnd = firstManualChunk.Find("ChunkEnd");
            if (manualChunkEnd != null)
            {
                lastChunkEnd = manualChunkEnd;
                spawnedChunks.Add(firstManualChunk.gameObject);
            }
        }
        else
        {
            // if no manual chunk, just spawn the first one
            SpawnNextChunk();
        }
    }

    private void Update()
    {
        if (lastChunkEnd == null || virtualCamera == null) return;

        Vector3 viewportPos = Camera.main.WorldToViewportPoint(lastChunkEnd.position);

        if (viewportPos.x < 1f)
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

        Vector3 spawnPosition;

        if (lastChunkEnd != null && newChunkStart != null)
        {
            Vector3 offset = newChunk.transform.position - newChunkStart.position;
            spawnPosition = lastChunkEnd.position + offset;
        }
        else
        {
            spawnPosition = Vector3.zero; // fallback
        }

        spawnPosition.y = 10f;
        spawnPosition.z = 100f;

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
