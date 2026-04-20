using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    [Header("Optional Panels")]
    public GameObject mainMenuCanvas;
    public GameObject settingsCanvas;

    void Start()
    {
        // Make sure main panel is shown first
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(true);
        if (settingsCanvas != null) settingsCanvas.SetActive(false);
    }

    // Start the game
    public void OpenLevel(string gameSceneName)
    {
        SceneManager.LoadScene(gameSceneName);
    }

    // Quit the game
    public void QuitGame()
    {
        Debug.Log("Game is quitting...");
        Application.Quit();
    }

    // Open settings menu
    public void OpenSettings()
    {
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(false);
        if (settingsCanvas != null) settingsCanvas.SetActive(true);
    }

    // Go back to main menu
    public void BackToMenu()
    {
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(true);
        if (settingsCanvas != null) settingsCanvas.SetActive(false);
    }
}