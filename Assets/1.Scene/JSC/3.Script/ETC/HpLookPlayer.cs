using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpLookPlayer : MonoBehaviour
{
    public Transform Cam;
    private void LateUpdate()
    {
        transform.LookAt(Cam);
    }
}
