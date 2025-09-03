using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonOpacityController : MonoBehaviour
{
    [System.Serializable]
    public class ButtonMapping
    {
        public Button triggerButton;          //button clicked
        public List<Image> targetImages;      //images whose opacity will change
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
        // Reset all images to normal opacity
        foreach (var bm in buttonMappings)
        {
            foreach (var img in bm.targetImages) SetImageOpacity(img, normalOpacity);
            if (bm.triggerButton != null) SetImageOpacity(bm.triggerButton.image, normalOpacity);
        }

        // Fade the target images
        foreach (var img in mapping.targetImages)
        {
            SetImageOpacity(img, fadedOpacity);
        }
    }

    void SetImageOpacity(Image img, float opacity)
    {
        if (img != null)
        {
            Color c = img.color;
            c.a = opacity;
            img.color = c;
        }
    }
}
