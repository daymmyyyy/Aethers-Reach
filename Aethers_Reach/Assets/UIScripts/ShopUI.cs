using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public Text totalCurrencyText;

    void Start()
    {
        int totalCurrency = RelicCurrency.GetTotalCurrency();
        if (totalCurrencyText != null)
            totalCurrencyText.text = "Total : " + totalCurrency;
    }
}
