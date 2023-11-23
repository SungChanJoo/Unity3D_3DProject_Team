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
    [SerializeField] string endingSceneName;
    public string currentSceneName;

    [SerializeField] private PlayerData player;
    [SerializeField] public PlayerDataJson playerData;

    public GameObject NonExistSaveDataUI;

    public int Seed;

    public Difficulty difficulty;

    private void Awake()
    {


        if (Instance == null)
        {
            Instance = this;
          //  
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }
    public void InitSetting()
    {
        playerData.maxMana = 100f;
        playerData.currentMana = 100f;
        playerData.maxStamina = 100f;
        playerData.currentStamina = 100f;
        playerData.maxHealth = 100f;
        playerData.currentHealth = 100f;


        playerData.PlayerPosition_x = 20f;
        playerData.PlayerPosition_y = 0;
        playerData.PlayerPosition_z = -164f;
        playerData.Difficulty = (float)Difficulty.Easy;
        playerData.SceneName = "";
        playerData.Seed = UnityEngine.Random.Range(0, 10000);
    }
    private void Update()
    {
        try
        {
            if (SceneManager.GetActiveScene().name != introSceneName && player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerData>();
            }

        }
        catch (NullReferenceException e)
        {
            player = null;
        }
    }

    //스카이박스 변경
    public void SkyBoxNight()
    {
        RenderSettings.skybox = nightMat;
    }

    public void PotalToBossRoom()
    {
        player.transform.position = Vector3.zero;
        Save();
        SceneManager.LoadScene(bossRoomSceneName);

    }

    public void LoadEnding()
    {
        SceneManager.LoadScene(endingSceneName);
    }

    public void Restart()
    {
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

    //플레이어 데이터 저장
    public void Save()
    {
        playerData.maxMana = player.MaxMana;
        playerData.currentMana = player.CurrentMana;
        playerData.maxStamina = player.MaxStamina;
        playerData.currentStamina = player.CurrentStamina;
        playerData.maxHealth = player.MaxHealth;
        playerData.currentHealth = player.CurrentHealth;

        playerData.PlayerPosition_x = player.transform.position.x;
        playerData.PlayerPosition_y = player.transform.position.y;
        playerData.PlayerPosition_z = player.transform.position.z;

        playerData.Difficulty = (float)difficulty;
        playerData.SceneName = currentSceneName;
        playerData.Seed = Seed;
        string fileName;

        fileName = Application.dataPath+"/PlayerData.json";// Path.Combine(Application.dataPath, "/PlayerData.json");
        if(!File.Exists(fileName))
        {
            File.Create(fileName);
        }
        string toJson = JsonConvert.SerializeObject(playerData, Formatting.Indented);
        Debug.Log(playerData.maxMana);

        File.WriteAllText(fileName, toJson);
    }

    public void DeleteSaveData()
    {
        string fileName;
        fileName = Application.dataPath + "/PlayerData.json";
        File.Delete(fileName);
    }

    //플레이어 데이터 불러오기
    public PlayerDataJson Load()
    {
        string fileName;
        try
        {
            fileName = Application.dataPath + "/PlayerData.json"; //Path.Combine(Application.dataPath, "/PlayerData.json");
            string ReadData = File.ReadAllText(fileName);
            PlayerDataJson playerData = new PlayerDataJson();
            playerData = JsonConvert.DeserializeObject<PlayerDataJson>(ReadData);
            this.playerData = playerData;
            return playerData;
        }
        catch (DirectoryNotFoundException e)
        {
            Debug.Log(e);
            return null;
        }

    }


    //플레이어가 마지막에 종료한 시점의 씬을 불러옴
    public void LoadPlayerScene()
    {
        try
        {
            PlayerDataJson playerData = Load();

            if (playerData != null)
            {
                SceneManager.LoadScene(playerData.SceneName);
            }
            Debug.Log("플레이어 데이터가 없는데 왜 안뜨죠");
        }

        catch (FileNotFoundException e)
        {
            NonExistSaveDataUI.SetActive(true);
            //todo 1120 나중에 버튼을 비활성화하든 텍스트를 띄워주든 해라
            Debug.Log("플레이어 데이터가 없습니다." + e);
        }

    }
}
