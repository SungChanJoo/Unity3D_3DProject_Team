using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadGameUI : MonoBehaviour
{
    public void LoadSaveData()
    {
        GameManager.Instance.LoadPlayerScene();
    }
}
