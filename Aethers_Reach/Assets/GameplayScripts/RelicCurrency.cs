using UnityEngine;
using UnityEngine.UI;

public class RelicCurrency : MonoBehaviour
{
    public static int totalRelics = 0;
    public int relicValue = 10;
    private Text relicText;

    void Start()
    {
        GameObject relicTextObj = GameObject.Find("RelicCurrencyCounter");
        if (relicTextObj != null)
        {
            relicText = relicTextObj.GetComponent<Text>();
        }

        UpdateRelicText();
    }

    void UpdateRelicText()
    {
        if (relicText != null)
            relicText.text = totalRelics.ToString();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CollectRelic();
        }
    }

    private void CollectRelic()
    {
        totalRelics += relicValue;
        UpdateRelicText();
        Destroy(gameObject);
    }

    public static void ResetRelics()
    {
        totalRelics = 0;
    }
}
