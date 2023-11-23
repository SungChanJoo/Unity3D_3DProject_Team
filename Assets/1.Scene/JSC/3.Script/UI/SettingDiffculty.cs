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

        GameManager.Instance.DeleteSaveData();

        GameManager.Instance.InitSetting();
        SceneManager.LoadScene(SceneName);
        //LoadingSceneManager.LoadScene(SceneName);
    }

}
