using UnityEngine;
using UnityEngine.UI;

public class CurrencyUI : MonoBehaviour
{
    public Text totalCurrencyText;

    void Start()
    {
        int totalCurrency = RelicCurrency.GetTotalCurrency();
        if (totalCurrencyText != null)
            totalCurrencyText.text = "" + totalCurrency;
    }
}
