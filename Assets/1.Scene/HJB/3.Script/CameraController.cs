using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [Header("플레이어 속도")]
    [SerializeField] private float Speed = 5f;
    [SerializeField] private float runSpeed = 10f;
     private float finalSpeed;

    [Header("카메라 우선순위변경")]
    [SerializeField] private CinemachineVirtualCamera vCamera;

    [Header("참조할 카메라")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [Header("카메라 방향")]
    [SerializeField] private Transform cameraPoint;

    [Header("lockOn 카메라")]
    [SerializeField] private Transform lockOnCamera;

    [Header("플레이어 방향 수정 카메라")]
    [SerializeField] private Transform moveCamera;

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
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            haveTarget = !haveTarget;
            Debug.Log(haveTarget);
        }
        //적체크카메라
        CheckEnemy();

        if (haveTarget)
        {
            //락온이 활성화 상태이면 적을 찾기
            
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
        Debug.Log(isRun);
        finalSpeed = isRun ? runSpeed : Speed;
        
        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                    
        bool move = moveInput.magnitude != 0;        
        
        //플레이어의 forward값을 카메라의 foward와 일치
        Vector3 lookForward = new Vector3(cameraPoint.forward.x, 0f, cameraPoint.forward.z).normalized;
        Vector3 lookRight = new Vector3(cameraPoint.right.x, 0f, cameraPoint.right.z).normalized;
        Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;
        if (move&&!haveTarget)
        {            
            player.forward = lookForward;
        }
        //락온이 아닐 때 이동
        if (!haveTarget&&!isRolling)
        {
            animator.SetBool("lockOn", haveTarget);

            Vector3 cameraForward = moveCamera.forward; // 카메라의 forward 방향
            Vector3 moveDirection = Vector3.zero; // 이동할 방향

            if (Input.GetKey(KeyCode.W))
            {
                moveDirection += cameraForward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                moveDirection -= cameraForward;
            }
            if (Input.GetKey(KeyCode.A))
            {
                moveDirection -= moveCamera.right; // 좌측 방향
            }
            if (Input.GetKey(KeyCode.D))
            {
                moveDirection += moveCamera.right; // 우측 방향
            }
            // 정규화된 이동 방향 벡터로 설정
            moveDirection = moveDirection.normalized;
            moveDirection.y = 0;
            
            transform.position += moveDirection * finalSpeed * Time.deltaTime;
            // 이동 방향을 시각적으로 표시
            Debug.DrawRay(player.position, moveDirection * 4f, Color.black);

            //_percent = Mathf.Clamp01(Speed);
            //animator.SetFloat("a", _percent, 0.1f, Time.deltaTime);
            
            //var playerGroundPos = new Vector3(player.position.x, 0, transform.position.z);
            //var cameraGroundPos = new Vector3(Camera.main.transform.position.x, 0, Camera.main.transform.position.z);
            //
            //var cameraToPlayer = (playerGroundPos - cameraGroundPos);
            //
            //var forward = cameraToPlayer.normalized;
            //var right = Vector3.Cross(Vector3.up, forward);
            //
            //Debug.DrawLine(transform.position, transform.position + forward * moveInput.y, Color.red);
            //Debug.DrawLine(transform.position, transform.position + right * moveInput.x, Color.blue);
            //
            //transform.Translate(forward * moveInput.y*Time.deltaTime*Speed);
            //transform.Translate(right * moveInput.x * Time.deltaTime * Speed);

            percent = moveInput.x;
            animator.SetFloat("x", percent,0.01f, Time.deltaTime); 
            percent = moveInput.y;
            animator.SetFloat("y", percent, 0.01f, Time.deltaTime);            
        }

        //락온에 따른 이동
        if (move&&!isRolling&&haveTarget)
        {
            animator.SetBool("lockOn", haveTarget);
            //플레이어 이동
            transform.Translate(moveDir * Time.deltaTime * finalSpeed);
                //+= moveDir * Time.deltaTime * finalSpeed;
            
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
        animator.SetBool("runing", isRun);
        //구르기        
        if (!isRolling &&Input.GetKeyDown(KeyCode.Space))
        {            
            StartCoroutine(Rolling());
        }
        //cameraPoint.position = player.position + new Vector3(0, 1f, 0);
    }
    

    private void RotateCamera()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = cameraPoint.rotation.eulerAngles;

        float x = camAngle.x - mouseDelta.y;
        //카메라 회전각 제어
        if (x<180f)
        {
            x = Mathf.Clamp(x, -1f, 20f);
        }
        else
        {
            x = Mathf.Clamp(x, 335f, 361f);
        }
        cameraPoint.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, 0f);        
    }
    private void CheckEnemy()
    {
        float distance = 5f;
        //나중에 적의 position 받아올 것.
        //플레이어와 적의 위치 계산 후 위치에 따른 카메라 위치 조정
        Vector3 directionEnemy = (enemy.position - transform.position).normalized;
        directionEnemy.y = 0;
        Vector3 cameraPosition = player.position + (directionEnemy * -distance) + Vector3.up *3f;
        Debug.DrawRay(player.position, directionEnemy * 5f, Color.red);
        lockOnCamera.position = cameraPosition;
        lockOnCamera.forward = directionEnemy;               
        
        //적방향으로 플레이어의 foward고정
        if (haveTarget)
        {
            transform.forward = directionEnemy;
            player.forward = directionEnemy;
        }
        
    }

    private IEnumerator Rolling()
    {
        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        isRolling = true;

        float rollSpeed = 5f; 


        // 구르는 방향 설정 빡시네
        Vector3 rollDirection= Vector3.zero;
        //if (Mathf.Abs(_percent) > Mathf.Abs(percent))
        //{
        //    if (_percent > 0)
        //    {                
        //        Debug.Log("위");
        //        rollDirection = cameraPoint.right.normalized;
        //    }
        //    else
        //    {
        //        Debug.Log("아래");                
        //        rollDirection = -cameraPoint.right.normalized;
        //    }
        //}
        //else
        //{
        //    if (percent > 0)
        //    {
        //        Debug.Log("앞");                
        //        rollDirection = cameraPoint.forward.normalized;
        //
        //    }
        //    else
        //    {
        //        Debug.Log("뒤");                
        //        rollDirection = -cameraPoint.forward.normalized;
        //    }
        //}
        //Vector3 cameraForward = moveCamera.forward; // 카메라의 forward 방향

        Vector3 cameraForward = moveCamera.forward;
        if (Input.GetKey(KeyCode.W))
        {
            rollDirection += cameraForward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            rollDirection -= cameraForward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            rollDirection -= moveCamera.right; // 좌측 방향
        }
        if (Input.GetKey(KeyCode.D))
        {
            rollDirection += moveCamera.right; // 우측 방향
        }
        // 정규화된 이동 방향 벡터로 설정
        rollDirection = rollDirection.normalized;
        rollDirection.y = 0;
        float timer = 0f;
        animator.SetFloat("x", _percent, 0.1f, Time.deltaTime);
        animator.SetFloat("y", percent, 0.1f, Time.deltaTime);
        animator.SetTrigger("rolling");        

        // 구르기 지속 시간
        while (timer < 1f) 
        {
            timer += Time.deltaTime;
            if (timer>0.2f && timer<0.9f)
            {                       
                // 구르는 방향으로 순간적으로 이동
                transform.position +=  rollDirection * rollSpeed * Time.deltaTime;
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);             
            }            
            yield return null;        
        }
        isRolling = false;                
    }   
}