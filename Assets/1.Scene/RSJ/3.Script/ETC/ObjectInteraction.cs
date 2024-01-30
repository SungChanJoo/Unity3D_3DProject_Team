using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    // 왼쪽문, 오른쪽문
    private GameObject rightDoor;
    private GameObject leftDoor;
    private bool isOpened = false;
    private Vector3 destRightDoorRot = new Vector3(0, -90, 0);
    private Vector3 destLeftDoorRot = new Vector3(0, 90, 0);

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openDoorClip;

    void Start()
    {
        rightDoor = this.transform.GetChild(2).gameObject;
        leftDoor = this.transform.GetChild(1).gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !isOpened)
        {
            audioSource.PlayOneShot(openDoorClip);
            BoxCollider thiscollider = GetComponent<BoxCollider>();
            this.transform.GetComponent<BoxCollider>().enabled = false;
            StartCoroutine(OpenGate());
        }
    }
    

    private IEnumerator OpenGate()
    {
        isOpened = true;
        float accumulateTime = 0f;
        Vector3 offset = this.transform.parent.transform.rotation.eulerAngles;
        while (accumulateTime < 4f)
        {
            float lerpValue = accumulateTime *0.25f;
            rightDoor.transform.rotation = Quaternion.Euler(Vector3.Lerp(Vector3.zero + offset, destRightDoorRot + offset, lerpValue));
            leftDoor.transform.rotation = Quaternion.Euler(Vector3.Lerp(Vector3.zero + offset, destLeftDoorRot + offset, lerpValue));
            accumulateTime += Time.deltaTime;
            //Debug.Log("축적된 시간" + accumulateTime);
            yield return null;
        }
        
    }
}
