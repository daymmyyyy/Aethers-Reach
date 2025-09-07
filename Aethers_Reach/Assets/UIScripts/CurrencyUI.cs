using UnityEngine;
using UnityEngine.UI;

public class CurrencyUI : MonoBehaviour
{
    public static CurrencyUI Instance;
    public Text totalCurrencyText;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateCurrencyDisplay();
    }

    public void UpdateCurrencyDisplay()
    {
        int totalCurrency = RelicCurrency.GetTotalCurrency();
        if (totalCurrencyText != null)
            totalCurrencyText.text = totalCurrency.ToString();
    }
}

