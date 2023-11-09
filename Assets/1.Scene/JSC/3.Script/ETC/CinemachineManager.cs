using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemachineManager : MonoBehaviour
{
    public static CinemachineManager Instance = null;

    [SerializeField] private TimelineController timelineController;

    [SerializeField] private GameObject playerCam;

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

    }

    public void LoadBossCam()
    {
        playerCam.SetActive(false);
        timelineController.Play();
    }

    private void OnEndCam()
    {
        playerCam.SetActive(true);
    }
}
