using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpLookPlayer : MonoBehaviour
{
    private Transform Cam;

    private void Awake()
    {
        Cam = GameObject.FindGameObjectWithTag("Cam").GetComponent<Transform>();
    }

    private void LateUpdate()
    {
        transform.LookAt(Cam.position);
/*        Vector3 target = (transform.position - Cam.position).normalized;

        transform.forward = target;
        Debug.Log(transform.position);*/
    }
}
