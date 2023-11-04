using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTest : MonoBehaviour
{
    private float detectionAngle = 90f; // 예시 각도: 90도
    private float detectionDistance = 10f; // 예시 거리: 10 유닛
    public List<GameObject> targetList = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < 360; i += 20)
        {
            GameObject t = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Vector3 pos = EulerToVector(i) * detectionDistance;
            pos.y = transform.position.y;
            t.transform.position = pos;
        }
    }

    void Update()
    {
        // 움직임 또는 회전 코드...
        transform.Rotate(Vector3.up * 100f, Time.deltaTime);

        Debug.DrawRay(transform.position, EulerToVector(detectionAngle / 2) * detectionDistance, Color.red);
        Debug.DrawRay(transform.position, EulerToVector(-detectionAngle / 2) * detectionDistance, Color.blue);
        TargetDetection();
    }

    Vector3 EulerToVector(float _in)
    {
        _in += transform.eulerAngles.y;
        _in *= Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(_in), 0, Mathf.Cos(_in));
    }

    private void TargetDetection()
    {
        Collider[] objs = Physics.OverlapSphere(transform.position, detectionDistance);
        targetList.Clear();

        float radianRange = Mathf.Cos((detectionAngle / 2) * Mathf.Deg2Rad);

        for (int i = 0; i < objs.Length; i++)
        {
            float targetRadian = Vector3.Dot(transform.forward, (objs[i].transform.position - transform.position).normalized);
            if (targetRadian > radianRange)
            {
                targetList.Add(objs[i].gameObject);
                Debug.DrawLine(transform.position, objs[i].transform.position, Color.black);
            }
        }

        Debug.Log("Detected targets: " + targetList.Count);
    }
}
