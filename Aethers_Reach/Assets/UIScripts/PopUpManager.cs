using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PopUpManager : MonoBehaviour
{
    public static PopUpManager Instance;

    [Header("Assign in Inspector")]
    public GameObject popUpPrefab; // The prefab (Text + Image)

    [Header("Settings")]
    public float displayDuration = 1f;

    private GameObject currentPopUp;
    private Text popUpText;
    private Coroutine currentCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetupPopUpForScene();
    }

    private void SetupPopUpForScene()
    {
        // Destroy previous pop-up if it exists
        if (currentPopUp != null)
        {
            Destroy(currentPopUp);
            currentPopUp = null;
        }

        GameObject canvas = GameObject.Find("PopUpCanvas");

        if (canvas == null)
        {
            Debug.LogError("PopUpManager: No PopUpCanvas found in the scene!");
            return;
        }

        currentPopUp = Instantiate(popUpPrefab, canvas.transform);
        popUpText = currentPopUp.GetComponentInChildren<Text>(true);
        currentPopUp.SetActive(false); // hidden initially
    }

    public void ShowPopUp(string message)
    {
        if (currentPopUp == null) return;

        if (popUpText != null)
            popUpText.text = message;

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(ShowNotification());
    }

    private IEnumerator ShowNotification()
    {
        currentPopUp.SetActive(true);

        yield return new WaitForSeconds(displayDuration);

        currentPopUp.SetActive(false);
        currentCoroutine = null;
    }
}
