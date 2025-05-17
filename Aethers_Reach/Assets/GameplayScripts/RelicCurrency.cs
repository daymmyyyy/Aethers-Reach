using UnityEngine;
using UnityEngine.UI;

public class RelicCurrency : MonoBehaviour
{
    public static int totalRelics = 0;
    public int relicValue = 10;
    public Text relicText;

    void Start()
    {
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
        Destroy(gameObject);  // Destroy this relic object after collecting
    }

    public static void ResetRelics()
    {
        totalRelics = 0;
    }
}
