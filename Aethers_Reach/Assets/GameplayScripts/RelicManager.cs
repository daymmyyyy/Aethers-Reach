using UnityEngine;
using UnityEngine.UI;

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
        if (fullRelicUI != null)
            fullRelicUI.SetActive(true);

        if (playerController != null)
            playerController.TriggerSpeedBoost(); //tell player to boost

        //play animation / achievement UI here
    }
}
