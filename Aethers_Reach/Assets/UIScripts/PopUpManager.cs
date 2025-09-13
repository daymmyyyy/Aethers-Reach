using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopUpManager : MonoBehaviour
{
    public static PopUpManager Instance;

    [Header("Pop-up UI")]
    public GameObject popUpPrefab; // Prefab with Image + Text
    public Transform popUpParent;  // Canvas transform

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // persists across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Show a temporary pop-up with text
    /// </summary>
    public void ShowPopUp(string message, float duration = 2f)
    {
        if (popUpPrefab == null || popUpParent == null) return;

        GameObject popUp = Instantiate(popUpPrefab, popUpParent);
        Text txt = popUp.GetComponentInChildren<Text>();
        if (txt != null) txt.text = message;

        // Destroy after duration
        Destroy(popUp, duration);
    }
}
