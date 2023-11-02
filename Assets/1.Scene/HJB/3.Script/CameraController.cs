using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{

    [SerializeField] private float Speed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float finalSpeed;
    [SerializeField] private Transform vCamera;
    [SerializeField] private Transform cameraPoint;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    
        
    private bool haveTarget = false;
    private bool run = false;
    

    public Transform player;
    public Transform enemy;

    Animator animator;

    private void Awake()
    {
        animator = player.GetComponent<Animator>();
    }
    private void Update()
    {
        LockOnTarget();

        if (Input.GetKey(KeyCode.LeftShift))
        {
            run = true;
        }
        else
        {
            run = false;
        }

        move();

        if (haveTarget)
        {
            CheckEnemy();
        }
        else
        {
            RotateCamera();
            virtualCamera.LookAt = null;
        }
    }
    private void LockOnTarget()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            haveTarget = !haveTarget;

            virtualCamera.LookAt = enemy;
        }
    }
    private void move()
    {
        finalSpeed = run ? runSpeed : Speed;

        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool isMove = moveInput.magnitude != 0;
        
        if (isMove)
        {
            Vector3 lookForward = new Vector3(cameraPoint.forward.x, 0f, cameraPoint.forward.z).normalized;
            Vector3 lookRight = new Vector3(cameraPoint.right.x, 0f, cameraPoint.right.z).normalized;
            Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

            player.forward = lookForward;
            transform.position += moveDir * Time.deltaTime * finalSpeed;
                        
            //transform.rotation = Quaternion.LookRotation(moveInput);
        }
        //player.forward = moveInput;
        //float percet = ((run) ? 1f : -1f) * moveInput.magnitude;

        

        
            float _percent =  1f * moveInput.x;
            Debug.Log(moveInput.y);
            animator.SetFloat("x", _percent, 0.1f, Time.deltaTime);
        
        
            float percent =  1f * moveInput.y;
            animator.SetFloat("y", percent, 0.1f, Time.deltaTime);
        
        
    }
    private void RotateCamera()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = cameraPoint.rotation.eulerAngles;

        float x = camAngle.x - mouseDelta.y;

        if (x<180f)
        {
            x = Mathf.Clamp(x, -1f, 70f);
        }
        else
        {
            x = Mathf.Clamp(x, 335f, 361f);
        }

        cameraPoint.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);

    }
    private void CheckEnemy()
    {
        float distance = 5f;

        Vector3 directionEneny = (enemy.position - player.position).normalized;

        Vector3 DirectionEnemy = -directionEneny;

        Vector3 cameraPosition = vCamera.position + (DirectionEnemy * distance) + Vector3.up * 3f;

        vCamera.position = cameraPosition;
    }
}