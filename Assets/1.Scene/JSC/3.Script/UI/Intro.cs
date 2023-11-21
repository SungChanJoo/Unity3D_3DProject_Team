using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Intro : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private GameObject difficultyUI;
    public void OnNewGame()
    {
        difficultyUI.SetActive(true);
    }

    public void OnGameExit()
    {
        Application.Quit();
    }
}
