using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] Orcs;
   
    [SerializeField] private int wayPointCount = 3;
    public GameObject wayPoint;
    public float weight = 10f;
    public float height = 10f;
    private Vector3 poolPosition = new Vector3(0, -25f, 0);

    private void Awake()
    {
        for (int i = 0; i < Random.Range(2, 5); i++)
        {
            int rand = Random.Range(0,100);
            GameObject orc;
            if (rand >= 50)
            {
                 orc = Instantiate(Orcs[0], transform.position, Quaternion.identity);
            }
            else if(rand >= 20)
            {
                 orc = Instantiate(Orcs[1], transform.position, Quaternion.identity);
            }
            else
            {
                 orc = Instantiate(Orcs[2], transform.position, Quaternion.identity);
            }

            Vector3 pPosition = RandomPosition();

            for (int j = 0; j < wayPointCount; j++)
            {

                if (j == 0)
                {
                    //pPosition = RandomPosition();
                    orc.GetComponent<AnyMonster>().wayPoint.Add(Instantiate(wayPoint, pPosition, Quaternion.identity));

                }
                else
                {
                    orc.GetComponent<AnyMonster>().wayPoint.Add(Instantiate(wayPoint, RandomPosition(pPosition), Quaternion.identity));
                }
            }
        }
    }
    Vector3 RandomPosition()
    {
        Vector3 originPosition = transform.position;

        float randomWeight = 0;
        float randomHeight = 0;

        randomWeight = Random.Range((weight * 0.5f) * -1, weight * 0.5f);
        randomHeight = Random.Range((height * 0.5f) * -1, height * 0.5f);
        Vector3 randomPostion = new Vector3(randomWeight, 0f, randomHeight);

        Vector3 respawnPosition = originPosition + randomPostion;
        return respawnPosition;
    }
    Vector3 RandomPosition(Vector3 pPosition)
    {

        Vector3 originPosition = transform.position;

        float randomWeight = 0;
        float randomHeight = 0;
        while (Mathf.Abs(randomWeight) < weight * 0.5f-1f && Mathf.Abs(randomHeight) < weight * 0.5f-1f)
        {
            randomWeight = Random.Range((weight * 0.5f) * -1, weight * 0.5f);
            randomHeight = Random.Range((height * 0.5f) * -1, height * 0.5f);

        }
        Vector3 randomPostion = new Vector3(randomWeight, 0f, randomHeight);

        Vector3 respawnPosition = originPosition + randomPostion;
        //Debug.Log(respawnPosition);
        return respawnPosition;
    }
}
