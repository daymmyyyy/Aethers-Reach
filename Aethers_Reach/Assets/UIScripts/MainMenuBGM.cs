using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MainMenuBGM : MonoBehaviour
{
    public static MainMenuBGM Instance;

    [Header("BGM Settings")]
    public AudioClip bgmClip;
    [Range(0f, 1f)] public float volume = 0.5f;

    private AudioSource audioSource;

    // scenes where BGM should persist
    private readonly string[] allowedScenes = {
        "MainMenu", "HighScore", "DiaryEntries", "Shop", "Settings"
    };

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = bgmClip;
        audioSource.loop = true;
        audioSource.volume = volume;
        audioSource.playOnAwake = false;

        if (bgmClip != null)
            audioSource.Play();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!IsAllowedScene(scene.name))
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(gameObject);
        }
    }


    private bool IsAllowedScene(string sceneName)
    {
        foreach (var s in allowedScenes)
        {
            if (s == sceneName) return true;
        }
        return false;
    }
}
