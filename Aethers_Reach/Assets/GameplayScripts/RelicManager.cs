using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RelicManager : MonoBehaviour
{
    public static RelicManager Instance;

    [Header("Relic Settings")]
    public int totalPiecesRequired = 5;          // total for full relic
    public int disablePiecesThreshold = 5;       // disable all pieces after 5
    private int currentPieces = 0;

    [Header("UI")]
    public Text relicCounterText;
    public GameObject fullRelicUI;
    public Image relicSprite; // relic icon

    [Header("Relic Drop")]
    public GameObject relicPrefab;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateRelicUI();

        if (fullRelicUI != null)
            fullRelicUI.SetActive(false);

        if (relicCounterText != null)
            relicCounterText.gameObject.SetActive(false);

        if (relicSprite != null)
            relicSprite.gameObject.SetActive(false);
    }

    public void CollectPiece()
    {
        currentPieces++;

        if (relicCounterText != null && !relicCounterText.gameObject.activeSelf)
            relicCounterText.gameObject.SetActive(true);
            relicSprite.gameObject.SetActive(true);


        UpdateRelicUI();

        if (currentPieces >= disablePiecesThreshold)
            HideAllRelicPieces();

        if (currentPieces >= totalPiecesRequired)
            CompleteRelic();
    }

    void UpdateRelicUI()
    {
        if (relicCounterText != null)
            relicCounterText.text = $"{currentPieces}/{totalPiecesRequired}";
    }

    void CompleteRelic()
    {
        Debug.Log("Relic fully assembled!");
        StartCoroutine(ShowFullRelicUI());
    }

    private void HideAllRelicPieces()
    {
        GameObject[] relicPieces = GameObject.FindGameObjectsWithTag("RelicPiece");
        foreach (GameObject piece in relicPieces)
        {
            DestroyRelicPiece(piece);
        }
    }

    public void DestroyRelicPiece(GameObject piece)
    {
        if (piece == null) return;
        Destroy(piece);
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
            relicSprite.gameObject.SetActive(false);
    }

    public bool HasCompletedRelic()
    {
        return currentPieces >= totalPiecesRequired;
    }

    public void ResetSessionRelics()
    {
        currentPieces = 0;
    }

    public bool ShouldHideRelics()
    {
        return currentPieces >= disablePiecesThreshold;
    }
}
