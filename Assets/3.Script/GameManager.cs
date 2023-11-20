using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;
    public Material nightMat;

    [SerializeField] string introSceneName;
    [SerializeField] string bossRoomSceneName;

    [SerializeField] GameObject endUI;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        if(endUI != null)
        {
            endUI.SetActive(false);
        }
    }
    public void SkyBoxNight()
    {
        RenderSettings.skybox = nightMat;
    }

    public void PotalToBossRoom()
    {
        SceneManager.LoadScene(bossRoomSceneName);
    }

    public void ViewEndingUI()
    {
        endUI.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
    public void HideEndingUI()
    {
        endUI.SetActive(false);
    }

    public void Restart()
    {
        HideEndingUI();
        SceneManager.LoadScene(introSceneName);
    }
    public void GameExit()
    {
        Application.Quit();
    }
}
