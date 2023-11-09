using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    //[Header("플레이어 속도")]    
    //[SerializeField] private float Speed = 5f;
    //[SerializeField] private float runSpeed = 8f;
    private float finalSpeed;
    private bool stamina = true;


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

    [Header("Player Data")]
    [SerializeField] private PlayerData data;
    [SerializeField] private WeaponBase weaponBase;
    [SerializeField] private PlayerAttack attack;
    private Collider _collider;
    //플레이어의 현재 상태
    private bool haveTarget = false;
    private bool isRun = false;
    private bool isRolling = false;
    private bool state = false;

    //플레이어 forward를 정하기위한 카메라 방향값
    private Vector3 lookForward;
    private Vector3 lookRight;
    private Vector3 moveDir;

    //담아야 할 적의 정보
    [SerializeField] private float detectionDistance = 50f;
    private float detectionAngle = 90f;
    public List<GameObject> targetList = new List<GameObject>();

    [Header("플레이어")]
    public Transform player;

    //락온 타겟이 될 오브젝트 변수
    [SerializeField]private GameObject targetEnemy = null;

    private Animator animator;
    private Rigidbody rigidbody;

    private float moveInputX;
    private float moveInputZ;

    //애니메이션 Blend값 저장할 변수
    float _percent;
    float percent;
    //중력

    private bool check = true;
    private void Awake()
    {
        animator = player.GetComponent<Animator>();
        _collider =player.GetComponent<Collider>();
        rigidbody = GetComponent<Rigidbody>();
        
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        //rigidbody.velocity = Vector3.zero;
        StateCheck();
        Debug.DrawRay(transform.position, EulerToVector(0) * detectionDistance, Color.green);
        Debug.DrawRay(transform.position, EulerToVector(detectionAngle / 2) * detectionDistance, Color.green);
        Debug.DrawRay(transform.position, EulerToVector(-detectionAngle / 2) * detectionDistance, Color.green);
        TargetDetection();

        LockOnTargetCheck();
        move();
        cameraPoint.position = Vector3.MoveTowards(cameraPoint.position, transform.position, 20f*Time.deltaTime);
    }
    

    //여기서 모든 상태를 하나로 묶어서 관리를 해야하나
    public void StateCheck()
    {
        //weaponBase.canDamageEnemy 앞 c를 C로 수정
        if (isRolling || weaponBase.CanDamageEnemy ||attack.hold)  
        {
            state = true;
        }
        else
        {
            state = false;
        }
    }

    private void LockOnTargetCheck()
    {
        if (Input.GetMouseButtonDown(2)&&targetList.Count!=0)
        {
            haveTarget = !haveTarget;
        }
        //적체크카메라
        CheckEnemyCamera(GetClosestEnemyInFront());

        if (targetEnemy!=null)
        {
            float distance = (targetEnemy.transform.position - transform.position).sqrMagnitude;
            distance =Mathf.Sqrt(distance);
            if (distance>detectionDistance)
            {
                haveTarget = false;
            }                  
        }

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
        //플레이어 이동속도 결정
        if (Input.GetKey(KeyCode.LeftShift)&&stamina)
        {            
            //stamina = data.UseStamina(0.1f);
            isRun = true;
        }
        else
        {
            isRun = false;
        }        
        finalSpeed = data.GetCurrentPlayerSpeed(isRun);
        if (!state)
        {

            moveInputX = Input.GetAxis("Horizontal");
            moveInputZ = Input.GetAxis("Vertical");
        }

        bool move = new Vector3(moveInputX,0,moveInputZ).magnitude != 0;
        

        //플레이어의 forward값을 카메라의 forward와 일치

        if (!haveTarget &&move&&!isRolling)
        {
            if (!haveTarget && move && !isRolling)
            {
                lookForward = new Vector3(cameraPoint.forward.x, 0f, cameraPoint.forward.z).normalized;
                lookRight = new Vector3(cameraPoint.right.x, 0f, cameraPoint.right.z).normalized;
                                               
                // 플레이어가 움직일 때 카메라 방향을 따라가도록 함
                //transform.forward = lookForward;                               
            }
        }
        else if (haveTarget)
        {
            lookForward = new Vector3(lockOnCamera.forward.x, 0f, lockOnCamera.forward.z).normalized;
            lookRight = new Vector3(lockOnCamera.right.x, 0f, lockOnCamera.right.z).normalized;
            Debug.DrawRay(transform.position, moveDir * 5f, Color.black);
            transform.forward = lookForward;

        }
        moveDir = lookForward * moveInputZ + lookRight * moveInputX;




        //락온이 아닐 때 이동
        if (!haveTarget && !state)
        {

            var playerGroundPos = new Vector3(transform.position.x, 0, transform.position.z);
            var cameraGroundPos = new Vector3(Camera.main.transform.position.x, 0, Camera.main.transform.position.z);

            var cameraToPlayer = (playerGroundPos - cameraGroundPos);

            var forward = cameraToPlayer.normalized;
            var right = Vector3.Cross(Vector3.up, forward);

            Debug.DrawLine(transform.position, transform.position + forward * moveInputZ, Color.red);
            Debug.DrawLine(transform.position, transform.position + right * moveInputX, Color.blue);

            //rigidbody.velocity = (forward * moveInputZ + right * moveInputX) * finalSpeed;

            

            // 바라보는 방향으로 회전 후 다시 정면을 바라보는 현상을 막기 위해 설정
            if (!(moveInputX == 0 && moveInputZ == 0))
            {
                // 이동과 회전을 함께 처리
                rigidbody.MovePosition(transform.position + (forward * moveInputZ + right * moveInputX) * finalSpeed * Time.deltaTime);
                // 회전하는 부분. Point 1.
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * (10f+finalSpeed));
            }
        
        }

        //락온에 따른 이동
        if (!state && haveTarget)
        {
            //플레이어 이동
            rigidbody.MovePosition(rigidbody.position + moveDir * finalSpeed * Time.deltaTime);
        }

        if (!haveTarget)
        {
            if (move)
            {
                percent = finalSpeed;

            }
            else
            {
                percent = 0;
            }
            animator.SetFloat("x", percent,0.1f,Time.deltaTime);
        }
        else
        {
            percent = moveInputX;
            animator.SetFloat("x", percent);
            percent = moveInputZ;
            animator.SetFloat("y", percent);//, 0.01f, Time.deltaTime

        }

        
        
        animator.SetBool("lockOn", haveTarget);
        animator.SetBool("runing", isRun);
        //구르기        
        if (!state &&move&& Input.GetKeyDown(KeyCode.Space)&&stamina)
        {
            //stamina = data.UseStamina(10f);
            StartCoroutine(Rolling());
        }
    }

    //카메라 회전
    private void RotateCamera()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = cameraPoint.rotation.eulerAngles;

        float x = camAngle.x - mouseDelta.y;
        //카메라 회전각 제어
        if (x < 180f)
        {
            x = Mathf.Clamp(x, -1f, 20f);
        }
        else
        {
            x = Mathf.Clamp(x, 335f, 361f);
        }
        cameraPoint.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, 0f);
    }
    //적을 체크하여 적을 바라볼 카메라 제어
    private void CheckEnemyCamera(GameObject _enemy)
    {

        if (_enemy == null)
        {
            return;
        }
        //나중에 적의 position 인자로 받아올 것.

        //플레이어와 적의 위치 계산 후 위치에 따른 카메라 위치 조정
        float distance = 5f;
        Vector3 directionEnemy = (_enemy.transform.position - transform.position).normalized;
        directionEnemy.y = 0;
        Vector3 cameraPosition = transform.position + (directionEnemy * -distance) + Vector3.up * 2.5f;
        Debug.DrawRay(transform.position, directionEnemy * 40f, Color.red);
        lockOnCamera.position = cameraPosition;
        lockOnCamera.forward = directionEnemy;

    }

    private IEnumerator Rolling()
    {
        float X = Input.GetAxis("Horizontal");
        float Z = Input.GetAxis("Vertical");

        isRolling = true;

        float rollSpeed = 5f;
        float timer = 0f;

        _percent = X;
        percent = Z;

        animator.SetFloat("x", _percent);
        animator.SetFloat("y", percent);
        animator.SetTrigger("rolling");

        Vector3 directionRoll = moveDir;

        _collider.enabled = false;
        // 구르기 지속 시간
        while (timer < 1f)
        {
            timer += Time.deltaTime;
            if (timer > 0.1f && timer < 1f)
            {
                // 구르는 방향으로 순간적으로 이동
                //transform.position += directionRoll.normalized * rollSpeed * Time.deltaTime;
                //transform.position = new Vector3(transform.position.x, 0, transform.position.z);
                float distanceToMove = rollSpeed * Time.deltaTime;
                Vector3 newPosition = transform.position + directionRoll.normalized * distanceToMove;

                rigidbody.MovePosition(new Vector3(newPosition.x, -2.384186e-07f, newPosition.z));
            }
            yield return null;
        }
        animator.SetTrigger("Default");               
        _collider.enabled = true;
        isRolling = false;
    }

    private Vector3 EulerToVector(float angle)
    {
        angle += moveCamera.eulerAngles.y;
        angle *= Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
    }

    //카메라 기준 정면 방향 적을 담는 메서드
    private void TargetDetection()
    {
        Collider[] objs = Physics.OverlapSphere(transform.position, detectionDistance);
        targetList.Clear();
        
        float radianRange = Mathf.Cos((detectionAngle / 2) * Mathf.Deg2Rad);

       
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i].CompareTag("Enemy"))
            { 
                float targetRadian = Vector3.Dot(transform.forward,
                    (objs[i].transform.position - transform.position).normalized);
                if (targetRadian > radianRange)
                {
                    targetList.Add(objs[i].gameObject);                
                    Debug.DrawLine(transform.position, objs[i].transform.position, Color.black);

                }
            }
        }        
    }
    //카메라 정면 기준 가장 가까운 적을 찾는 메서드
    private GameObject GetClosestEnemyInFront()
    {
        if (haveTarget && targetEnemy != null)
        {
            return targetEnemy;
        }        
        else
        {
            StartCoroutine(GetclosersetInFrontDelay());
        }
        
        return targetEnemy;
    }
    private IEnumerator GetclosersetInFrontDelay()
    {
        //카메라 전환중에 다른타겟을 보지않도록 하기위해 딜레이
        yield return new WaitForSeconds(2f);
        GameObject closestEnemy = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 cameraForward = moveCamera.forward;
        Vector3 playerPosition = player.position;

        foreach (GameObject enemy in targetList)
        {
            //여기 계산식 좀 더 정교하게 바꿀 것
            Vector3 directionToEnemy = enemy.transform.position - playerPosition;
            float distanceSqrToEnemy = directionToEnemy.sqrMagnitude;

            float targetRadian = Vector3.Dot(directionToEnemy, cameraForward);
            if (targetRadian > 0)
            {
                if (distanceSqrToEnemy < closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqrToEnemy;
                    closestEnemy = enemy;
                }
            }
        }

        targetEnemy = closestEnemy;
        
    }
}