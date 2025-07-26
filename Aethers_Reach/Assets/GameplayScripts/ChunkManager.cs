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
    private int chunksSinceRelicComplete = 0;
    private bool relicWasCompletedLastCheck = false;
    private bool spawnedInitialPortal = false;



    [Header("Portal Settings")]
    public GameObject portalChunkPrefab;
    //private bool portalSpawned = false;


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
        GameObject prefab;
        bool relicComplete = RelicManager.Instance != null && RelicManager.Instance.HasCompletedRelic();

        if (relicComplete)
        {
            if (!relicWasCompletedLastCheck)
            {
                // First detection of full relic collected
                chunksSinceRelicComplete = 0;
                spawnedInitialPortal = false;
                relicWasCompletedLastCheck = true;
            }

            if (!spawnedInitialPortal)
            {
                // Immediately spawn portal chunk after relic is complete
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
            // Normal spawning before relic completion
            prefab = chunkPrefabs[Random.Range(0, chunkPrefabs.Length)];
            relicWasCompletedLastCheck = false;
            spawnedInitialPortal = false;
            chunksSinceRelicComplete = 0;
        }

        GameObject newChunk = Instantiate(prefab);

        Transform newChunkEnd = newChunk.transform.Find("ChunkEnd");

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

        // Disable relic pieces if relic is already complete
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
