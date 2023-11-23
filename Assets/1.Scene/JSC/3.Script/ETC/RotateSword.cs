using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSword : MonoBehaviour
{
    [SerializeField] private Vector3 rotateVector;
    // Start is called before the first frame update
    private void Update()
    {
        transform.Rotate(rotateVector * Time.deltaTime);
    }
}
