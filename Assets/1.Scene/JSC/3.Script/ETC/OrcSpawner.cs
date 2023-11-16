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
        for (int i = 0; i < Random.Range(1, 5); i++)
        {
            GameObject orc = Instantiate(Orcs[0], transform.position, Quaternion.identity);

            Vector3 pPosition = transform.position;
            orc.GetComponent<AnyMonster>().wayPoint.Add(Instantiate(wayPoint, pPosition, Quaternion.identity));

            /*            for (int j = 0; j < wayPointCount; j++)
                        {
                            float temp = Time.time * 100f;
                            Random.InitState((int)temp);
                            if (j == 0)
                            {
                                //pPosition = RandomPosition();
                                orc.GetComponent<AnyMonster>().wayPoint.Add(Instantiate(wayPoint, pPosition, Quaternion.identity));

                            }
                            else
                            {
                                orc.GetComponent<AnyMonster>().wayPoint.Add(Instantiate(wayPoint, RandomPosition(pPosition), Quaternion.identity));
                            }
                        }*/
        }
    }
/*    Vector3 RandomPosition()
    {


        Vector3 originPosition = transform.position;

        rangeX = Random.Range((rangeX / 2) * -1, rangeX / 2);
        rangeZ = Random.Range((rangeZ / 2) * -1, rangeZ / 2);
        Vector3 randomPostion = new Vector3(rangeX, 0f, rangeZ);

        Vector3 respawnPosition = originPosition + randomPostion;
        return respawnPosition;
    }
    Vector3 RandomPosition(Vector3 pPosition)
    {

        Vector3 originPosition = transform.position;
        Vector3 randomPostion = pPosition;
        while (Vector3.SqrMagnitude(pPosition - randomPostion)>70f)
        {
            rangeZ = Random.Range((rangeZ / 2) * -1, rangeZ / 2);
            rangeX = Random.Range((rangeX / 2) * -1, rangeX / 2);
            randomPostion = new Vector3(rangeX, 0f, rangeZ);
        }


        Vector3 respawnPosition = originPosition + randomPostion;
        return respawnPosition;
    }*/
}
