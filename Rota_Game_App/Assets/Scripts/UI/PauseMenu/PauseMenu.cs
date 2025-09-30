using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Canvas pauseMenuCanvas;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    private bool isPaused = false;

    private void Awake()
    {
        resumeButton.onClick.AddListener(ResumeGame);
        restartButton.onClick.AddListener(RestartGame);
        settingsButton.onClick.AddListener(OpenSettings);
        quitButton.onClick.AddListener(QuitGame);
        pauseMenuCanvas.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenuCanvas.enabled = isPaused;
        Time.timeScale = isPaused ? 0 : 1;
    }

    private void ResumeGame() => TogglePause();

    private void RestartGame()
    {
        Time.timeScale = 1;
       //add screen transition here if needed
        
    }

    private void OpenSettings()
    {
        // Hook into your settings UI logic
        Debug.Log("Settings opened");
    }

    private void QuitGame()
    {
        Time.timeScale = 1;
        //add screen transition here if needed
    }
}