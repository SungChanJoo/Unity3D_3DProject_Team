using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class Intro : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private GameObject difficultyUI;

    [SerializeField] Image[] images = null;
    [SerializeField] TextMeshProUGUI[] texts = null;

    private void Start()
    {
        StartCoroutine(FadeTextToFullAlpha(1.5f, images, texts));
    }

    public IEnumerator FadeTextToFullAlpha(float t, Image[] images, TextMeshProUGUI[] texts)
    {
        float elapsedTime = 0f;
        Color[] imageStartColors = new Color[images.Length];
        Color[] textStartColors = new Color[texts.Length];

        for (int i = 0; i < images.Length; i++)
        {
            imageStartColors[i] = images[i].color;
            images[i].color = new Color(imageStartColors[i].r, imageStartColors[i].g, imageStartColors[i].b, 0f);
        }

        for (int i = 0; i < texts.Length; i++)
        {
            textStartColors[i] = texts[i].color;
            texts[i].color = new Color(textStartColors[i].r, textStartColors[i].g, textStartColors[i].b, 0f);
        }

        while (elapsedTime < t)
        {
            for (int i = 0; i < images.Length; i++)
            {
                images[i].color = Color.Lerp(new Color(imageStartColors[i].r, imageStartColors[i].g, imageStartColors[i].b, 0f), imageStartColors[i], elapsedTime / t);
            }

            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].color = Color.Lerp(new Color(textStartColors[i].r, textStartColors[i].g, textStartColors[i].b, 0f), textStartColors[i], elapsedTime / t);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure reaching the exact final color values
        for (int i = 0; i < images.Length; i++)
        {
            images[i].color = imageStartColors[i];
        }

        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].color = textStartColors[i];
        }
    }


    public void OnNewGame()
    {
        difficultyUI.SetActive(true);
    }

    public void OnGameExit()
    {
        Application.Quit();
    }
}
