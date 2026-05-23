using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlManager : MonoBehaviour
{
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Back() {
        if (!string.IsNullOrEmpty(SceneHistory.LastSceneName))
            {
                SceneManager.LoadScene(SceneHistory.LastSceneName);
            }
    }
}
