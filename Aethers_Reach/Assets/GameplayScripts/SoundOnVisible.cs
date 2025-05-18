using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundOnVisible : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip soundToPlay;
    [Range(0f, 1f)] public float volume = 1f;
    public bool playOnce = true;

    private bool hasPlayed = false;
    private AudioSource audioSource;
    private Renderer objectRenderer;
    private Camera mainCamera;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        objectRenderer = GetComponent<Renderer>();
        mainCamera = Camera.main; // Cinemachine
    }

    void Update()
    {
        if (mainCamera == null || objectRenderer == null) return;

        if (IsVisibleToCamera(mainCamera))
        {
            if (!hasPlayed || !playOnce)
            {
                PlaySound();
                hasPlayed = true;
            }
        }
        else
        {
            if (!playOnce)
                hasPlayed = false;
        }
    }

    private bool IsVisibleToCamera(Camera cam)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        return GeometryUtility.TestPlanesAABB(planes, objectRenderer.bounds);
    }

    private void PlaySound()
    {
        if (soundToPlay != null)
        {
            audioSource.clip = soundToPlay;
            audioSource.volume = volume;
            audioSource.Play();
        }
    }
}
