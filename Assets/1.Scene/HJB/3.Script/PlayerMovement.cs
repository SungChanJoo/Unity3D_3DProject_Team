using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;



public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 10f;
    

    private bool playerWalk = false;
    

    public Transform player;
    public Transform enemy;

    [SerializeField] private Transform vCamera;
    [SerializeField] private Transform cameraPoint;

    Rigidbody rigid;
    Animator animator;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        PlayerMove();
        
    }
    private void RotateCamera()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = cameraPoint.rotation.eulerAngles;

        float x = camAngle.x - mouseDelta.y;

        if (x < 180f)
        {
            x = Mathf.Clamp(x, -1f, 70f);
        }
        else
        {
            x = Mathf.Clamp(x, 335f, 361f);
        }

        cameraPoint.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);

    }
    private void PlayerMove()
    {
        playerWalk = false;
        if (Input.anyKey)
        {
            float horizon = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector3 move = new Vector3(horizon, 0, vertical) * playerSpeed;

            rigid.velocity = move;

            playerWalk = true;

            
                //if (move != Vector3.zero)
                //{
                //    // 방향으로 회전
                //    transform.rotation = Quaternion.LookRotation(move);
                //}
            
           

        }
            animator.SetBool("walk", playerWalk);
    }


}
