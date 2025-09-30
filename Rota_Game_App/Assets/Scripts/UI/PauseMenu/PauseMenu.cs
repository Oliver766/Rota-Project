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

    public LevelLoaading levelLoaading;


    private bool isPaused = false;

    private void Awake()
    {
       
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

    public void ResumeGame() => TogglePause();

    public void RestartGame()
    {
        Time.timeScale = 1;
        //add screen transition here if needed
       levelLoaading.LoadScene(2); // Assuming 0 is the main menu scene index

        
    }

    public void OpenSettings()
    {
        // Hook into your settings UI logic
        Debug.Log("Settings opened");
    }

    public void QuitGame()
    {
        Time.timeScale = 1;
        //add screen transition here if needed 

        levelLoaading.LoadScene(0); // Assuming 0 is the main menu scene index

        
        
    }
}