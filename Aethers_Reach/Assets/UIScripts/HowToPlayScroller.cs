using UnityEngine;

public class HowToPlayScroller : MonoBehaviour
{
    public GameObject[] howToPlayImages;
    private int currentIndex = 0;

    void Start()
    {
        UpdateImageVisibility();
    }

    public void ShowNext()
    {
        if (currentIndex < howToPlayImages.Length - 1)
        {
            currentIndex++;
            UpdateImageVisibility();
        }
    }

    public void ShowPrevious()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateImageVisibility();
        }
    }

    private void UpdateImageVisibility()
    {
        for (int i = 0; i < howToPlayImages.Length; i++)
        {
            howToPlayImages[i].SetActive(i == currentIndex);
        }
    }
}
