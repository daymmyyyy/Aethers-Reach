using UnityEngine;
using UnityEngine.UI;

public class RelicCurrency : MonoBehaviour
{
    public static int totalCurrency = 0;
    public static int currencyThisSession = 0;

    public int currencyValue = 10;
    private static Text currencyText;
    private static Animator currencyAnimator;

    [Header("Audio")]
    public AudioClip collectSFX; // assign in inspector

    void Start()
    {
        GameObject currencyTextObj = GameObject.Find("RelicCurrencyCounter");
        if (currencyTextObj != null)
        {
            currencyText = currencyTextObj.GetComponent<Text>();
            currencyAnimator = currencyTextObj.GetComponent<Animator>();
        }

        // Load total relics from PlayerPrefs
        totalCurrency = PlayerPrefs.GetInt("TotalCurrencyCollected", 0);
        UpdateCurrencyText();
    }

    public static void UpdateCurrencyText()
    {
        if (currencyText != null)
        {
            currencyText.text = currencyThisSession.ToString();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CollectCurrency();
        }
    }

    private void CollectCurrency()
    {
        totalCurrency += currencyValue;
        currencyThisSession += currencyValue;

        PlayerPrefs.SetInt("TotalCurrencyCollected", totalCurrency);
        PlayerPrefs.Save();

        UpdateCurrencyText();

        // Play collect SFX
        if (AudioManager.Instance != null && collectSFX != null)
        {
            AudioManager.Instance.PlaySFX(collectSFX);
        }

        Destroy(gameObject);
    }

    public static void ResetCurrency()
    {
        currencyThisSession = 0;
    }

    public static int GetSessionCurrency()
    {
        return currencyThisSession;
    }

    public static int GetTotalCurrency()
    {
        return PlayerPrefs.GetInt("TotalCurrencyCollected", 0);
    }

    public static bool SpendCurrency(int amount)
    {
        int currentCurrency = PlayerPrefs.GetInt("TotalCurrencyCollected", 0);

        if (currentCurrency >= amount)
        {
            currentCurrency = Mathf.Max(0, currentCurrency - amount);
            PlayerPrefs.SetInt("TotalCurrencyCollected", currentCurrency);
            PlayerPrefs.Save();
            totalCurrency = currentCurrency;
            return true;
        }
        return false; // not enough currency
    }

    public static void LoseCurrency(int amount)
    {
        // Decrease session relics
        currencyThisSession = Mathf.Max(0, currencyThisSession - amount);

        UpdateCurrencyText();

        if (currencyAnimator != null)
        {
            currencyAnimator.SetBool("isShaking", false);
            currencyAnimator.Update(0f);
            currencyAnimator.SetBool("isShaking", true);
        }
    }

    // To allow static methods to access instance SFX
    private static RelicCurrency _instance;
    public static RelicCurrency Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<RelicCurrency>();
            return _instance;
        }
    }
}
