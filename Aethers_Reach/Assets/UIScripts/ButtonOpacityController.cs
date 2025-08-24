using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonOpacityController : MonoBehaviour
{
    [System.Serializable]
    public class ButtonMapping
    {
        public Button triggerButton;          // The button clicked
        public List<Button> targetButtons;    // The buttons whose opacity will change
    }

    [Header("Button mappings")]
    public List<ButtonMapping> buttonMappings;

    [Range(0f, 1f)] public float fadedOpacity = 0.5f;
    public float normalOpacity = 1f;             

    void Start()
    {
        foreach (var mapping in buttonMappings)
        {
            if (mapping.triggerButton != null)
            {
                mapping.triggerButton.onClick.AddListener(() => OnButtonClicked(mapping));
            }
        }
    }

    void OnButtonClicked(ButtonMapping mapping)
    {
        foreach (var bm in buttonMappings)
        {
            if (bm.triggerButton != null) SetButtonOpacity(bm.triggerButton, normalOpacity);
            foreach (var t in bm.targetButtons) SetButtonOpacity(t, normalOpacity);
        }

        foreach (var target in mapping.targetButtons)
        {
            SetButtonOpacity(target, fadedOpacity);
        }
    }

    void SetButtonOpacity(Button button, float opacity)
    {
        if (button != null && button.image != null)
        {
            Color c = button.image.color;
            c.a = opacity;
            button.image.color = c;
        }
    }
}
