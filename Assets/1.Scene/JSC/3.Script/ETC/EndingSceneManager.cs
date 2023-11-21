using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingSceneManager : MonoBehaviour
{

    [SerializeField] private string introSceneName;
    private void Awake()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void Restart()
    {
        SceneManager.LoadScene(introSceneName);
    }
    public void GameExit()
    {
        Application.Quit();
    }
}
