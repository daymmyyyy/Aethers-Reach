using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RelicManager : MonoBehaviour
{
    public static RelicManager Instance;
    private int fullRelicsThisSession = 0;

    [Header("Relic Settings")]
    public int totalPiecesRequired = 10;
    private int currentPieces = 0;

    [Header("UI")]
    public Text relicCounterText;
    public GameObject fullRelicUI;

    private PlayerController playerController; // reference to player

    [Header("Relic Drop")]
    public GameObject relicPrefab;


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

        // Update total
        int totalRelics = PlayerPrefs.GetInt("RelicsCollected", 0);
        PlayerPrefs.SetInt("RelicsCollected", totalRelics + 1);
        PlayerPrefs.Save();

        // Track session-only relics
        fullRelicsThisSession++;
        PlayerPrefs.SetInt("RelicsThisSession", fullRelicsThisSession);
        PlayerPrefs.Save();

      //  if (playerController != null)
          //  playerController.TriggerSpeedBoost();

        // Disable all relic pieces
        GameObject[] relicPieces = GameObject.FindGameObjectsWithTag("RelicPiece");
        foreach (GameObject piece in relicPieces)
        {
            piece.SetActive(false);
        }

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

        if (currentPieces == 0)
        {
            if (relicCounterText != null)
                relicCounterText.gameObject.SetActive(false);

            if (fullRelicUI != null)
                fullRelicUI.SetActive(false);
        }
    }
    public bool HasCompletedRelic()
    {
        return currentPieces >= totalPiecesRequired;
    }

    public int GetRelicsThisSession()
    {
        return fullRelicsThisSession;
    }
    public void ResetSessionRelics()
    {
        fullRelicsThisSession = 0;

    }


}
