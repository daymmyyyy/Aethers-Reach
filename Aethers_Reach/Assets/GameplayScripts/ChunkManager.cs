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
    private int chunksSinceLastPortal = 0;
    private bool portalMissed = false;
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

        // Check if player missed current portal
        if (currentPortal != null && !portalMissed)
        {
            float portalX = currentPortal.transform.position.x;
            if (player.position.x > portalX + 1f) // small buffer
            {
                portalMissed = true;
                chunksSinceLastPortal = 0; // reset counter to spawn next portal after N chunks
            }
        }
    }

    public void SpawnNextChunk()
    {
        GameObject prefab;

        bool spawnPortal = currentPortal == null && fillerChunksSincePortal >= 3;

        if (spawnPortal)
        {
            prefab = portalChunkPrefab;
            fillerChunksSincePortal = 0;

            currentPortal = null; // assign after spawn below
            Debug.Log("Spawning portal chunk!");
        }
        else
        {
            // Random filler chunk without repetition
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

        // Force full opacity
        Renderer[] renderers = newChunk.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in renderers)
        {
            rend.material = new Material(rend.material); // unique material
            Color c = rend.material.color;
            c.a = 1f;
            rend.material.color = c;
        }

        // Position chunk
        Transform newChunkEnd = newChunk.transform.Find("ChunkEnd");
        if (newChunkEnd == null)
            newChunkEnd = newChunk.transform;

        Vector3 spawnPosition = lastChunkEnd != null
            ? new Vector3(lastChunkEnd.position.x, 14f, lastChunkEnd.position.z)
            : new Vector3(0, 14f, 50f);

        newChunk.transform.position = spawnPosition;

        // Assign Teleporter if this is a portal
        if (prefab == portalChunkPrefab)
        {
            currentPortal = newChunk;
            Teleporter teleporter = newChunk.GetComponentInChildren<Teleporter>();
            if (teleporter != null)
            {
                teleporter.targetSceneName = "YourTargetScene";
            }
            else
            {
                Debug.LogWarning("Portal chunk missing Teleporter script!");
            }
        }

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
