using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadGameUI : MonoBehaviour
{
    [SerializeField] private GameObject nonExistSaveDataUI;
    public void LoadSaveData()
    {
        GameManager.Instance.NonExistSaveDataUI = nonExistSaveDataUI;

        GameManager.Instance.LoadPlayerScene();
    }
}
