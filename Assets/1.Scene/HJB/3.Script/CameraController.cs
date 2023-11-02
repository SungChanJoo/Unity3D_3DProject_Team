using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [Header("플레이어 속도")]
    [SerializeField] private float Speed = 5f;
    [SerializeField] private float isRunSpeed = 10f;
     private float finalSpeed;

    [Header("Follow Object")]
    [SerializeField] private Transform vCamera;

    [Header("참조할 카메라")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [Header("카메라 방향")]
    [SerializeField] private Transform cameraPoint;
    
    private bool haveTarget = false;
    private bool isRun = false;
    private bool isRolling = false;

    [Header("대상")]
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
        move();
        
    }    

    private void LockOnTarget()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            haveTarget = !haveTarget;

            virtualCamera.LookAt = enemy;
        }
        if (haveTarget)
        {   //락온이 활성화 상태이면 적을 찾기
            CheckEnemy();
        }
        else
        {   //락온이 비활성화면 카메라 회전이 가능하고 카메라의 고정기능을 null로 반환
            RotateCamera();
            virtualCamera.LookAt = null;
        }
    }
    private void move()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isRun = true;
        }
        else
        {
            isRun = false;
        }
        animator.SetBool("runing", isRun);
        finalSpeed = isRun ? isRunSpeed : Speed;
        
        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                    
        bool isMove = moveInput.magnitude != 0;
        if (isMove)
        {                
            Vector3 lookForward = new Vector3(cameraPoint.forward.x, 0f, cameraPoint.forward.z).normalized;
            Vector3 lookRight = new Vector3(cameraPoint.right.x, 0f, cameraPoint.right.z).normalized;
            Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;
            //플레이어의 forward값을 카메라의 foward와 일치
            player.forward = lookForward;
            //플레이어 이동
            transform.position += moveDir * Time.deltaTime * finalSpeed;
        }
        //애니메이션의 Blend값 처리
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isRolling = true;
            animator.SetBool("rolling", isRolling);

            float _percent = 1f * moveInput.x;
            animator.SetFloat("rollingX", _percent, 0.1f, Time.deltaTime);
            float percent = 1f * moveInput.y;
            animator.SetFloat("rollingY", percent, 0.1f, Time.deltaTime);
            
        }

        if (isRun)
        {
            float _percent = 1f * moveInput.x;
            animator.SetFloat("RunX", _percent, 0.1f, Time.deltaTime);
            float percent = 1f * moveInput.y;
            animator.SetFloat("RunY", percent, 0.1f, Time.deltaTime);
        }
        else if (!isRun)
        {
            float _percent = 1f * moveInput.x;
            animator.SetFloat("x", _percent, 0.1f, Time.deltaTime);
            float percent = 1f * moveInput.y;
            animator.SetFloat("y", percent, 0.1f, Time.deltaTime);
        }


        
    }
    private void RotateCamera()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = cameraPoint.rotation.eulerAngles;

        float x = camAngle.x - mouseDelta.y;
        //카메라 회전각 제어
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

        //플레이어와 적의 위치 계산 후 위치에 따른 카메라 위치 조정
        Vector3 directionEneny = (enemy.position - player.position).normalized;
        Vector3 DirectionEnemy = -directionEneny;
        Vector3 cameraPosition = vCamera.position + (DirectionEnemy * distance) + Vector3.up * 3f;

        vCamera.position = cameraPosition;
        //적방향으로 플레이어의 foward고정
        player.forward = directionEneny * player.position.y;
    }

    
    

}