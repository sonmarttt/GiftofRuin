using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuPanel;

    private bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (isPaused) {
                ResumeGame();
            } else {
                PauseGame();
            }
        }
    }

    public void PauseGame() {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame() {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void MainMenu() {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene("Start Screen");
    }

    public void Restart() {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Controls() {
        SceneHistory.LastSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("ControlScene");
    }
}
