using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemachineCallBack : MonoBehaviour
{
    void OnEndCam()
    {
        CinemachineManager.Instance.OnEndCam();
        Destroy(gameObject);
    }
}
