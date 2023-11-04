using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [Header("플레이어 속도")]
    [SerializeField] private float Speed = 5f;
    [SerializeField] private float runSpeed = 8f;
     private float finalSpeed;

    [Header("카메라 우선순위변경")]
    [SerializeField] private CinemachineVirtualCamera vCamera;

    [Header("참조할 카메라")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [Header("카메라 방향")]
    [SerializeField] private Transform cameraPoint;

    [Header("lockOn 카메라")]
    [SerializeField] private Transform lockOnCamera;

    [Header("평시 카메라")]
    [SerializeField] private Transform moveCamera;

    //플레이어의 현재 상태
    private bool haveTarget = false;
    private bool isRun = false;
    private bool isRolling = false;

    //플레이어 forward를 정하기위한 카메라 방향값
    private Vector3 lookForward;
    private Vector3 lookRight;
    private Vector3 moveDir;

    //담아야 할 적
    private float detectionAngle =90f;
    [SerializeField]private float detectionDistance = 50f;
    public List<GameObject> targetList = new List<GameObject>();

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
    private void Start()
    {
        
    }
    private void Update()
    {
        //transform.Rotate(Vector3.up * 10f, Time.deltaTime);
        Debug.DrawRay(transform.position, EulerToVector(detectionAngle / 2) * detectionDistance, Color.green);
        Debug.DrawRay(transform.position, EulerToVector(-detectionAngle / 2) * detectionDistance, Color.green);
        TargetDetection();

        LockOnTarget();
        move();
        //DrawLine();
    }    

    private void LockOnTarget()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            haveTarget = !haveTarget;
            Debug.Log(haveTarget);
        }
        //적체크카메라
        CheckEnemyCamera();

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
        finalSpeed = isRun ? runSpeed : Speed;
        
        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                    
        bool move = moveInput.magnitude != 0;

        //플레이어의 forward값을 카메라의 forward와 일치
        
        if (!haveTarget&&move)
        {
            lookForward = new Vector3(cameraPoint.forward.x, 0f, cameraPoint.forward.z).normalized;
            lookRight = new Vector3(cameraPoint.right.x, 0f, cameraPoint.right.z).normalized;
            player.forward = lookForward;
        }
        else if (haveTarget)
        {
            lookForward= new Vector3(lockOnCamera.forward.x, 0f, lockOnCamera.forward.z).normalized;
            lookRight = new Vector3(lockOnCamera.right.x, 0f, lockOnCamera.right.z).normalized;
            Debug.DrawRay(player.position, moveDir * 5f, Color.black);
            player.forward = lookForward;

        }
            moveDir = lookForward * moveInput.y + lookRight * moveInput.x;
            

       
        
        //락온이 아닐 때 이동
        if (!haveTarget&&!isRolling)
        {
            //animator.SetBool("lockOn", haveTarget);
            //
            //Vector3 cameraForward = moveCamera.forward; // 카메라의 forward 방향
            //Vector3 moveDirection = Vector3.zero; // 이동할 방향
            //
            //if (Input.GetKey(KeyCode.W))
            //{
            //    moveDirection += cameraForward;
            //}
            //if (Input.GetKey(KeyCode.S))
            //{
            //    moveDirection -= cameraForward;
            //}
            //if (Input.GetKey(KeyCode.A))
            //{
            //    moveDirection -= moveCamera.right; // 좌측 방향
            //}
            //if (Input.GetKey(KeyCode.D))
            //{
            //    moveDirection += moveCamera.right; // 우측 방향
            //}
            //// 정규화된 이동 방향 벡터로 설정
            //moveDirection = moveDirection.normalized;
            //moveDirection.y = 0;
            //
            //transform.position += moveDirection * finalSpeed * Time.deltaTime;
            //// 이동 방향을 시각적으로 표시
            //Debug.DrawRay(player.position, moveDirection * 4f, Color.black);

            //_percent = Mathf.Clamp01(Speed);
            //animator.SetFloat("a", _percent, 0.1f, Time.deltaTime);
            
            var playerGroundPos = new Vector3(player.position.x, 0, transform.position.z);
            var cameraGroundPos = new Vector3(Camera.main.transform.position.x, 0, Camera.main.transform.position.z);
            
            var cameraToPlayer = (playerGroundPos - cameraGroundPos);
            
            var forward = cameraToPlayer.normalized;
            var right = Vector3.Cross(Vector3.up, forward);
            
            Debug.DrawLine(transform.position, transform.position + forward * moveInput.y, Color.red);
            Debug.DrawLine(transform.position, transform.position + right * moveInput.x, Color.blue);
            
            transform.Translate(forward * moveInput.y*Time.deltaTime*finalSpeed);
            transform.Translate(right * moveInput.x * Time.deltaTime * finalSpeed);

            
            percent = moveInput.x;
            animator.SetFloat("x", percent,0.01f, Time.deltaTime); 
            percent = moveInput.y;
            animator.SetFloat("y", percent, 0.01f, Time.deltaTime);            
        }

        //락온에 따른 이동
        if (move&&!isRolling&&haveTarget)
        {
           
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
        animator.SetBool("lockOn", haveTarget);
        animator.SetBool("runing", isRun);
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
            x = Mathf.Clamp(x, -1f, 20f);
        }
        else
        {
            x = Mathf.Clamp(x, 335f, 361f);
        }
        cameraPoint.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, 0f);        
    }
    private void CheckEnemyCamera()
    {

        //나중에 적의 position 인자로 받아올 것.
        CheckEnemy();
        //플레이어와 적의 위치 계산 후 위치에 따른 카메라 위치 조정
        float distance = 5f;
        Vector3 directionEnemy = (enemy.position - transform.position).normalized;
        directionEnemy.y = 0;
        Vector3 cameraPosition = player.position + (directionEnemy * -distance) + Vector3.up *2.5f;
        Debug.DrawRay(player.position, directionEnemy * 40f, Color.red);
        lockOnCamera.position = cameraPosition;
        lockOnCamera.forward = directionEnemy;      
        
    }
    private void CheckEnemy()
    {
        
       
        
    }
   
    

    private IEnumerator Rolling()
    {
        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        
        isRolling = true;        

        float rollSpeed = 5f;     
        float timer = 0f;

        _percent = moveInput.x;
        percent = moveInput.y;

        animator.SetFloat("x", _percent, 0.1f, Time.deltaTime);
        animator.SetFloat("y", percent, 0.1f, Time.deltaTime);
        animator.SetTrigger("rolling");

        Vector3 directionRoll = moveDir;

        // 구르기 지속 시간
        while (timer < 1f) 
        {
            timer += Time.deltaTime;
            if (timer>0.1f&& timer<0.9f)
            {
                // 구르는 방향으로 순간적으로 이동
                transform.position +=  directionRoll.normalized * rollSpeed * Time.deltaTime;
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);             
            }            
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
        isRolling = false;                
    }

    Vector3 EulerToVector(float _in)
    {
        _in += moveCamera.eulerAngles.y;
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
            float targetRadian = Vector3.Dot(player.forward,
                (objs[i].transform.position - player.position).normalized);
            if (targetRadian > radianRange)
            {
                targetList.Add(objs[i].gameObject);
                Debug.Log(targetList);
                Debug.DrawLine(player.position, objs[i].transform.position, Color.black);
            }
        }
        Debug.Log("Detected targets: " + targetList.Count);
    }

}