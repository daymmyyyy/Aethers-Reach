using UnityEngine;
using UnityEngine.SceneManagement;  // important!

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Gameplay");
    }
}
