using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip clickSound;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(PlayClickSound);
    }

    private void PlayClickSound()
    {
        // If this button is already the selected one, skip playing sound
        if (EventSystem.current != null &&
            EventSystem.current.currentSelectedGameObject == button.gameObject)
        {
            return;
        }

        if (clickSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(clickSound);

            // Mark this button as the new selected one
            EventSystem.current.SetSelectedGameObject(button.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(PlayClickSound);
    }
}
