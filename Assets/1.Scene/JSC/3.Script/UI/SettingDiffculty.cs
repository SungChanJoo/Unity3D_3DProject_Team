using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingDiffculty : MonoBehaviour
{
    [SerializeField] string SceneName;
    [SerializeField] Difficulty difficulty;

    public void OnSetDifficulty()
    {
        GameManager.Instance.difficulty = difficulty;
        SceneManager.LoadScene(SceneName);
    }

}
