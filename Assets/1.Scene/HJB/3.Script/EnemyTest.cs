using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTest : MonoBehaviour
{

    private float lerpTime = 0;
    [SerializeField] private float speed = 0.2f;
    [SerializeField] private float distance = 2f;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        lerpTime += Time.deltaTime * speed;
        float x = Mathf.Sin(lerpTime);
        

        transform.position = new Vector3( x*distance, 3, transform.position.z); ;
    }
}
