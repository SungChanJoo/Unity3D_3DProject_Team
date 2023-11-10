using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemachineManager : MonoBehaviour
{
    public static CinemachineManager Instance = null;

    [SerializeField] private TimelineController timelineController;

    [SerializeField] private GameObject playerCam;

    [SerializeField] private List<GameObject> UI;
    // Start is called before the first frame update
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        timelineController = GetComponent<TimelineController>();
        gameObject.SetActive(false);
        
    }

    public void LoadBossCam()
    {
        SetUI(false);
        playerCam.SetActive(false);
        gameObject.SetActive(true);
        timelineController.Play();
        FindObjectOfType<Knight>().canFight = false;

    }

    private void OnEndCam()
    {
        SetUI(true);
        gameObject.SetActive(false);
        playerCam.SetActive(true);
        FindObjectOfType<Knight>().canFight = true;
    }

    private void SetUI(bool state)
    {
        for (int i = 0; i < UI.Count; i++)
        {
            UI[i].SetActive(state);
        }
    }
}
