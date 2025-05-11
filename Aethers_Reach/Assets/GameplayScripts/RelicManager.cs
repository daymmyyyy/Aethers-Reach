using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RelicManager : MonoBehaviour
{
    public static RelicManager Instance;

    [Header("Relic Settings")]
    public int totalPiecesRequired = 10;
    private int currentPieces = 0;

    [Header("UI")]
    public Text relicCounterText;
    public GameObject fullRelicUI;

    private PlayerController playerController; // reference to player

    [Header("Relic Drop")]
    public GameObject relicPrefab;
    public float dropOffset = 2f; // how far in front of the player to drop them


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        UpdateRelicUI();

        if (fullRelicUI != null)
            fullRelicUI.SetActive(false);

        if (relicCounterText != null)
            relicCounterText.gameObject.SetActive(false);

        playerController = FindObjectOfType<PlayerController>();
    }

    public void CollectPiece()
    {
        currentPieces++;

        if (relicCounterText != null && !relicCounterText.gameObject.activeSelf)
            relicCounterText.gameObject.SetActive(true);

        UpdateRelicUI();

        if (currentPieces >= totalPiecesRequired)
        {
            CompleteRelic();
        }
    }

    void UpdateRelicUI()
    {
        if (relicCounterText != null)
            relicCounterText.text = $"{currentPieces}/{totalPiecesRequired} Relic Pieces";
    }

    void CompleteRelic()
    {
        Debug.Log("Relic fully assembled!");

        // Store number of full relics
        int relicsCollected = PlayerPrefs.GetInt("RelicsCollected", 0);
        PlayerPrefs.SetInt("RelicsCollected", relicsCollected + 1);
        PlayerPrefs.Save();

        if (playerController != null)
            playerController.TriggerSpeedBoost();

        StartCoroutine(ShowFullRelicUI());
    }


    private IEnumerator ShowFullRelicUI()
    {
        if (fullRelicUI != null)
            fullRelicUI.SetActive(true);

        yield return new WaitForSeconds(2f);

        if (fullRelicUI != null)
            fullRelicUI.SetActive(false);

        if (relicCounterText != null)
            relicCounterText.gameObject.SetActive(false);
    }

    public void LoseRelics(int amount)
    {
        currentPieces = Mathf.Max(0, currentPieces - amount);
        UpdateRelicUI();
    }

    public void DropRelics(int amount, Transform playerTransform)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 dropPosition = playerTransform.position + new Vector3(dropOffset + i * 10f, 0, 0);
            Instantiate(relicPrefab, dropPosition, Quaternion.identity);
        }
    }


}
