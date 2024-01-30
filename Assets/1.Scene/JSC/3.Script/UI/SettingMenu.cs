using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingMenu : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private bool activity;
    public void OnSettingActivate()
    {
        if(activity)
        {
            Time.timeScale = 0; //�Ͻ�����
        }
        else
        {
            Time.timeScale = 1; //�Ͻ�����
        }
        menu.SetActive(activity);
    }
}
