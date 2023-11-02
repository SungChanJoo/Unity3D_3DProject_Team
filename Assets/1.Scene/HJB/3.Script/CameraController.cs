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

    [Header("카메라 우선순위변경")]
    [SerializeField] private CinemachineVirtualCamera vCamera;

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
    //애니메이션 Blend값 저장할 변수
    float _percent;
    float percent;
    //수렴 속도
    float percentDamping = 1.5f;

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
        CheckEnemy();
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            haveTarget = !haveTarget;            
            Vector3 directionEneny = (enemy.position - player.position).normalized;
            transform.forward = directionEneny;        
        }

        if (haveTarget)
        {   //락온이 활성화 상태이면 적을 찾기
            vCamera.Priority = 5;
        }
        else
        {   //락온이 비활성화면 카메라 회전이 가능하고 카메라의 고정기능을 null로 반환
            RotateCamera();
            vCamera.Priority = 20;
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
                    
        bool move = moveInput.magnitude != 0;
        
        //플레이어의 forward값을 카메라의 foward와 일치
        Vector3 lookForward = new Vector3(cameraPoint.forward.x, 0f, cameraPoint.forward.z).normalized;
        Vector3 lookRight = new Vector3(cameraPoint.right.x, 0f, cameraPoint.right.z).normalized;
        Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;
        if (move)
        {
            player.forward = lookForward;

        }

        if (move&&!isRolling)
        {                
            //플레이어 이동
            transform.position += moveDir * Time.deltaTime * finalSpeed;
            //애니메이션의 Blend값 처리
            if (!isRolling&& isRun)
            {
                _percent = 1f * moveInput.x;
                animator.SetFloat("RunX", _percent, 0.1f, Time.deltaTime);
                percent = 1f * moveInput.y;
                animator.SetFloat("RunY", percent, 0.1f, Time.deltaTime);
            }
            else if (!isRolling && !isRun)
            {
                _percent = 1f * moveInput.x;
                animator.SetFloat("x", _percent, 0.1f, Time.deltaTime);
                percent = 1f * moveInput.y;
                animator.SetFloat("y", percent, 0.1f, Time.deltaTime);
            }
        }
        else
        {
            _percent = Mathf.Lerp(_percent, 0f, Time.deltaTime * percentDamping);
            percent = Mathf.Lerp(percent, 0f, Time.deltaTime * percentDamping);

            animator.SetFloat("RunX", _percent, 0.1f, Time.deltaTime);
            animator.SetFloat("RunY", percent, 0.1f, Time.deltaTime);
            animator.SetFloat("x", _percent, 0.1f, Time.deltaTime);
            animator.SetFloat("y", percent, 0.1f, Time.deltaTime);
        }


        //구르기
            
        
        if (!isRolling &&Input.GetKeyDown(KeyCode.Space))
        {            
            StartCoroutine(Rolling());
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
        //float distance = 5f;
        //나중에 적의 position 받아올 것.
        //플레이어와 적의 위치 계산 후 위치에 따른 카메라 위치 조정
        //Vector3 directionEneny = (enemy.position - player.position).normalized;
        //Vector3 DirectionEnemy = -directionEneny;
        //Vector3 cameraPosition = vCamera.position + (DirectionEnemy * distance) + Vector3.up *6f;
        //Debug.DrawRay(transform.position, DirectionEnemy * 5f, Color.red);
        //vCamera.position = cameraPosition;
        //적방향으로 플레이어의 foward고정
        //transform.forward = directionEneny * transform.position.y;
    }

    private IEnumerator Rolling()
    {
        isRolling = true;

        float rollSpeed = 10f; 

        animator.SetFloat("x", _percent, 0.1f, Time.deltaTime);
        animator.SetFloat("y", percent, 0.1f, Time.deltaTime);
        animator.SetTrigger("rolling");
        animator.SetTrigger("out");

        // 구르는 방향 설정 빡시네
        Vector3 rollDirection;
        if (Mathf.Abs(_percent) > Mathf.Abs(percent))
        {
            if (_percent > 0)
            {
                
                Debug.Log("위");
                rollDirection = cameraPoint.right.normalized;
            }
            else
            {
                Debug.Log("아래");                
                rollDirection = -cameraPoint.right.normalized;
            }
        }
        else
        {
            if (percent > 0)
            {
                Debug.Log("앞");                
                rollDirection = cameraPoint.forward.normalized;

            }
            else
            {
                Debug.Log("뒤");                
                rollDirection = -cameraPoint.forward.normalized;
            }
        }        
        float timer = 0f;

        // 구르기 지속 시간
        while (timer < 1f) 
        {
            timer += Time.deltaTime;
            if (timer>0.2f)
            {
            
            
                // 구르는 방향으로 순간적으로 이동
                transform.position +=  rollDirection * rollSpeed * Time.deltaTime;
                transform.position = new Vector3(transform.position.x, 0, transform.position.z); 
            
            }
            //timer += Time.deltaTime;
            //transform.position += rollDirection * rollSpeed * Time.deltaTime;
            //transform.position = new Vector3(transform.position.x, 0, transform.position.z);
                yield return null;
        }
        isRolling = false;                
    }
    

}