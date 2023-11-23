using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingDiffculty : MonoBehaviour
{
    [SerializeField] string SceneName;

    public void OnSetDifficulty()
    {

        GameManager.Instance.DeleteSaveData();

        GameManager.Instance.InitSetting();
        //SceneManager.LoadScene(SceneName);
        LoadingSceneManager.LoadScene(SceneName);
    }

}
