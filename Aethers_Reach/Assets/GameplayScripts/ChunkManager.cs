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

    private readonly List<GameObject> spawnedChunks = new();
    private Transform lastChunkEnd;

    private int chunksSinceRelicComplete = 0;
    private bool relicWasCompletedLastCheck = false;
    private bool spawnedInitialPortal = false;

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

        if (viewportPos.x <1.1f && viewportPos.x >1f)
        {
            SpawnNextChunk();
        }
    }

    public void SpawnNextChunk()
    {
        GameObject prefab;
        bool relicComplete = RelicManager.Instance != null && RelicManager.Instance.HasCompletedRelic();

        if (relicComplete)
        {
            if (!relicWasCompletedLastCheck)
            {
                chunksSinceRelicComplete = 0;
                spawnedInitialPortal = false;
                relicWasCompletedLastCheck = true;
            }

            if (!spawnedInitialPortal)
            {
                prefab = portalChunkPrefab;
                spawnedInitialPortal = true;
                Debug.Log("Spawning initial portal chunk!");
            }
            else
            {
                chunksSinceRelicComplete++;
                if (chunksSinceRelicComplete >= 3)
                {
                    prefab = portalChunkPrefab;
                    chunksSinceRelicComplete = 0;
                    Debug.Log("Spawning recurring portal chunk!");
                }
                else
                {
                    prefab = chunkPrefabs[Random.Range(0, chunkPrefabs.Length)];
                }
            }
        }
        else
        {
            prefab = chunkPrefabs[Random.Range(0, chunkPrefabs.Length)];
            relicWasCompletedLastCheck = false;
            spawnedInitialPortal = false;
            chunksSinceRelicComplete = 0;
        }

        GameObject newChunk = Instantiate(prefab);
        Transform newChunkEnd = newChunk.transform.Find("ChunkEnd");

        Vector3 spawnPosition = lastChunkEnd != null
            ? new Vector3(lastChunkEnd.position.x, 14f, lastChunkEnd.position.z)
            : new Vector3(0, 14f, 0);

        newChunk.transform.position = spawnPosition;

        if (relicComplete)
        {
            foreach (Transform child in newChunk.GetComponentsInChildren<Transform>())
            {
                if (child.CompareTag("RelicPiece"))
                    child.gameObject.SetActive(false);
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
