using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void Controls() {
        SceneHistory.LastSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("ControlScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}