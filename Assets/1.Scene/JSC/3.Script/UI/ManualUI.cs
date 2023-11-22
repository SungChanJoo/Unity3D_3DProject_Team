using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManualUI : MonoBehaviour
{
    public void ManualExit()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        gameObject.SetActive(false);
    }

    public void GoTitle()
    {
        GameManager.Instance.currentSceneName = SceneManager.GetActiveScene().name;

        GameManager.Instance.Save();
        GameManager.Instance.Restart();
    }
    public void GameExit()
    {
        GameManager.Instance.GameExit();
    }

    
}
