using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;
    public Material nightMat;

    [SerializeField] string introSceneName;
    [SerializeField] string bossRoomSceneName;
    [SerializeField] string currentSceneName;
    [SerializeField] string nextSceneName;
    [SerializeField] GameObject endUI;

    [SerializeField] private PlayerData player;
    [SerializeField] public PlayerDataJson playerData;

    public Difficulty difficulty;

    private void Awake()
    {


        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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



    private void Update()
    {
        try
        {
            if (SceneManager.GetActiveScene().name != introSceneName && player == null)
            {
                Debug.Log("�� �������");
                player = GameObject.Find("Player").GetComponent<PlayerData>();
            }
        }catch(NullReferenceException e)
        {
            player = null;
        }



    }

    //��ī�̹ڽ� ����
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
        //Cursor.lockState = CursorLockMode.Confined;
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

    private void OnApplicationQuit()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        Save();
    }

    //�÷��̾� ������ ����
    public void Save()
    {
        playerData.maxMana = player.MaxMana;
        playerData.currentMana = player.CurrentHealth;
        playerData.maxStamina = player.MaxStamina;
        playerData.currentStamina = player.CurrentStamina;
        playerData.maxHealth = player.MaxHealth;
        playerData.currentHealth = player.CurrentHealth;

        playerData.PlayerPosition_x = player.transform.position.x;
        playerData.PlayerPosition_y = player.transform.position.y;
        playerData.PlayerPosition_z = player.transform.position.z;

        playerData.Difficulty = difficulty;
        playerData.SceneName = currentSceneName;
        string fileName;

        fileName = Path.Combine("Player/", "PlayerData.json");
        string toJson = JsonConvert.SerializeObject(playerData, Formatting.Indented);
        Debug.Log(playerData.maxMana);

        File.WriteAllText(fileName, toJson);
    }

    //�÷��̾� ������ �ҷ�����
    public PlayerDataJson Load()
    {
        string fileName;
        try
        {
            fileName = Path.Combine("Player/", "PlayerData.json");
            string ReadData = File.ReadAllText(fileName);
            PlayerDataJson playerData = new PlayerDataJson();
            playerData = JsonConvert.DeserializeObject<PlayerDataJson>(ReadData);

            Debug.Log(playerData.maxMana);
            return playerData;
        }
        catch (DirectoryNotFoundException e)
        {
            Debug.Log(e);
            return null;
        }




    }

    //�÷��̾ �������� ������ ������ ���� �ҷ���
    public void LoadPlayerScene()
    {
        PlayerDataJson playerData = Load();
        if(playerData != null)
        {
            SceneManager.LoadScene(playerData.SceneName);
        }
        else
        {
            //todo 1120 ���߿� ��ư�� ��Ȱ��ȭ�ϵ� �ؽ�Ʈ�� ����ֵ� �ض�
            Debug.Log("�÷��̾� �����Ͱ� �����ϴ�.");
        }
    }
}
