using UnityEngine;
using UnityEngine.UI;

public class ButtonStateUI : MonoBehaviour
{
    [Header("References")]
    public Button targetButton;
    public Text mainText;
    public Image[] icons;
    public Text extraText;

    [Header("Colors")]
    public Color interactableTextColor = Color.black;
    public Color notInteractableTextColor = Color.grey;

    private bool lastState;

    private void Start()
    {
        lastState = targetButton.interactable;
        ApplyState(lastState);
    }

    private void Update()
    {
        if (targetButton.interactable != lastState)
        {
            lastState = targetButton.interactable;
            ApplyState(lastState);
        }
    }

    private void ApplyState(bool isInteractable)
    {
        if (mainText == null) return;

        if (isInteractable)
        {
            mainText.color = interactableTextColor;

            if (extraText != null)
                extraText.gameObject.SetActive(false);

            foreach (var icon in icons)
                if (icon != null) icon.gameObject.SetActive(false);
        }
        else
        {
            mainText.color = notInteractableTextColor;

            if (extraText != null)
            {
                extraText.color = interactableTextColor;
                extraText.gameObject.SetActive(true);
            }

            foreach (var icon in icons)
                if (icon != null) icon.gameObject.SetActive(true);
        }
    }
}
