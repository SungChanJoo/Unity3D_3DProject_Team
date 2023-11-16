using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum BossName
{
    Knight =0,
    Skeleton
}

public class CinemachineManager : MonoBehaviour
{
    public static CinemachineManager Instance = null;

    [SerializeField] private List<TimelineController> timelineControllers;

    [SerializeField] private GameObject playerCam;

    [SerializeField] private List<GameObject> UI;

    [SerializeField] private List<GameObject> bosses;
    [SerializeField] private Knight knight;
    [SerializeField] private Skeleton skeleton;
    private BossName bossName;
    private int index;
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
        gameObject.SetActive(false);
        index = 0;
        bosses[0].GetComponent<Knight>().TryGetComponent(out knight);
        bosses[1].GetComponent<Skeleton>().TryGetComponent(out skeleton);
        bossName = BossName.Knight;

        /*        if(bosses[0].GetComponent<Knight>().TryGetComponent(out knight))
                {
                    bossName = BossName.Knight;
                }
                if (bosses[1].GetComponent<Skeleton>().TryGetComponent(out skeleton))
                {
                    bossName = BossName.Skeleton;
                }*/
    }

    public void LoadBossCam()
    {
        SetUI(false);
        playerCam.SetActive(false);
        timelineControllers[index].Play();
        if (bossName == BossName.Knight)
        {
            knight.canFight = false;
        }
        if (bossName == BossName.Skeleton)
        {
            skeleton.canFight = false;
        }
    }

    public void OnEndCam()
    {
        SetUI(true);
        playerCam.SetActive(true);
        if (bossName == BossName.Knight)
        {
            knight.canFight = true;
        }
        if (bossName == BossName.Skeleton)
        {
            skeleton.canFight = true;
        }
        index++;
        bossName = (BossName)index;
    }

    private void SetUI(bool state)
    {
        for (int i = 0; i < UI.Count; i++)
        {
            UI[i].SetActive(state);
        }
    }
}
