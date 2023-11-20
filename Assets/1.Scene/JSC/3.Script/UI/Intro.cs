using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Intro : MonoBehaviour
{
    [SerializeField] private string sceneName;
    public void OnGameStart()
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OnGameExit()
    {
        Application.Quit();
    }
}
