using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    [SerializeField] private PlayerData player;
    [SerializeField] private string sceneName;
    [SerializeField] private List<Image> images;

    private List<float> colorsA;

    private void Awake()
    {
        colorsA = new List<float>();
        for (int i =0; i < images.Count; i++)
        {
            colorsA.Add(images[i].color.a);

            images[i].color = new Color(images[i].color.r, images[i].color.g, images[i].color.b, 0f);
        }

    }
    public void LoadGameOver()
    {
        gameObject.SetActive(true);
        StartCoroutine(LoadImage_co());
    }
    public void OnRestart()
    {
        Time.timeScale = 1;

        SceneManager.LoadScene(sceneName);
        gameObject.SetActive(false);

    }
    IEnumerator LoadImage_co()
    {
        float startNum = 0;
        float endNum = 1;
        while(endNum - startNum >= 0.3f)
        {
            startNum = Mathf.Lerp(startNum, endNum, Time.deltaTime * 0.5f);
            for (int i = 0; i < images.Count; i++)
            {
                if (images[i].color.a - colorsA[i] >= 0.3f)
                {
                    //Debug.Log("color : "+colorsA[i]);
                    continue;
                }

                images[i].color = new Color(images[i].color.r, images[i].color.g, images[i].color.b, startNum);
            }
            //Debug.Log(endNum - startNum);
            yield return null;
        }
        Time.timeScale = 0;

    }
}
