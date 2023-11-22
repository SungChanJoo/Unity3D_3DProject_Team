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

    [Header("참조할 카메라 및 카메라 우선순위변경(CM vcam1)")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [Header("카메라 방향 (Camera Arm)")]
    [SerializeField] private Transform cameraPoint;

    [Header("lockOn 카메라 (CM vcam2)")]
    [SerializeField] private Transform lockOnCamera;

    [Header("평시 카메라 (CM vcam1)")]
    [SerializeField] private Transform moveCamera;

    [Header("WeaponBase 추가")]
    [SerializeField] private WeaponBase weaponBase;
    
    //플레이어의 현재 상태
    private bool haveTarget = false;
    private bool isRun = false;
    private bool state = false;
    public bool rolling = true;
    public bool isRolling = false;
    public bool isParalysed = false;

    //플레이어 forward를 정하기위한 카메라 방향값
    private Vector3 lookForward;
    private Vector3 lookRight;
    private Vector3 moveDir;

    //담아야 할 적의 정보
    [SerializeField] private float detectionDistance = 50f;
    private float detectionAngle = 90f;
    private List<GameObject> targetList = new List<GameObject>();       

    //락온 타겟이 될 오브젝트 변수
    [SerializeField]private GameObject targetEnemy = null;

    //키 입력값 공유 저장
    private float moveInputX;
    private float moveInputZ;

    //애니메이션 Blend값 저장할 변수
    float _percent;
    float percent;
    
    //가져올 플레이어 컴포넌트
    private Collider _collider;
    private Animator animator;
    private Rigidbody rigid;
    private PlayerAttack attack;
    private PlayerData data;

    //LockOnTargetUI 이미지
    [SerializeField] private GameObject lockOnTargetUI;

    [Header("Audio 추가")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip footstepClip;
    private float footstepTimer = 0;

    private bool check = true;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        _collider =GetComponent<Collider>();
        rigid = GetComponent<Rigidbody>();
        attack = GetComponent<PlayerAttack>();
        data = GetComponent<PlayerData>();

    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cameraPoint.transform.position = new Vector3(GameManager.Instance.playerData.PlayerPosition_x,
                                             GameManager.Instance.playerData.PlayerPosition_y,
                                             GameManager.Instance.playerData.PlayerPosition_z);
    }
    private void Update()
    {        
        StateCheck();
        Debug.DrawRay(transform.position, EulerToVector(0) * detectionDistance, Color.green);
        Debug.DrawRay(transform.position, EulerToVector(detectionAngle*0.6f ) * detectionDistance, Color.green);
        Debug.DrawRay(transform.position, EulerToVector(-detectionAngle*0.6f ) * detectionDistance, Color.green);
        LockOnTargetCheck();
        TargetDetection();
        move();
                
    }

    //여기서 모든 상태를 하나로 묶어서 관리를 해야하나
    public void StateCheck()
    {        
        if (isRolling || attack.hold || isParalysed)  
        {
            
            state = true;
        }
        else
        {
            state = false;
            
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log($"상태 : {state}");
            Debug.Log($"타겟 : {haveTarget}");
        }
    }

    #region // 락온 상태, 타겟 확인, 카메라 우선순위 총괄 메서드
    private void LockOnTargetCheck()
    {
        if (Input.GetMouseButtonDown(2)&&targetList.Count!=0)
        {
            haveTarget = !haveTarget;            
        }
        else if(Input.GetMouseButtonDown(2)&&targetList.Count==0)
        {
            haveTarget = false;
        }

        if (targetEnemy == null)
        {
            haveTarget = false;
        }        
        //적체크카메라
        CheckEnemyCamera(GetClosestEnemyInFront());


        if (haveTarget)
        {
            //락온이 활성화 상태이면 적을 찾기
            virtualCamera.Priority = 5;
            lockOnTargetUI.SetActive(true);            
        }
        else
        {   //락온이 비활성화면 카메라 회전이 가능하고 카메라의 고정기능을 null로 반환
            virtualCamera.Follow = cameraPoint;
            RotateCamera();
            virtualCamera.Priority = 20;
            lockOnTargetUI.SetActive(false);
        }

        //target과 Player의 거리가 멀어지면
        if (targetEnemy!=null)
        {
            float distance = (targetEnemy.transform.position - transform.position).sqrMagnitude;
            distance =Mathf.Sqrt(distance);
            if (distance>detectionDistance)
            {
                haveTarget = false;
            }                  
        }

        //평시 카메라 위치 조정
        Vector3 cameraY = transform.position + new Vector3(0, 1f, 0);
        cameraPoint.position = Vector3.MoveTowards(cameraPoint.position, cameraY, 20f * Time.deltaTime);
    }
    #endregion

    private void move()
    {               
        //플레이어 이동속도 결정
        if (Input.GetKey(KeyCode.LeftShift)&&true== data.UseStamina(0.1f))
        {               
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
        //현재 플레이어가 정지 상태인지 확인
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

            //바라보는 방향으로 회전 후 다시 정면을 바라보는 현상을 막기 위해 설정
            if (!(moveInputX == 0 && moveInputZ == 0))
            {
                //이동과 회전을 함께 처리
                rigid.MovePosition(transform.position + (forward * moveInputZ + right * moveInputX) * finalSpeed * Time.deltaTime);
                //회전하는 부분
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * (10f+finalSpeed));
            }        
        }

        //락온에 따른 이동
        if (haveTarget&& !state)
        {
            //플레이어 이동
            rigid.MovePosition(rigid.position + moveDir * finalSpeed * Time.deltaTime);
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
        if (!state&&rolling && move && Input.GetKeyDown(KeyCode.Space)
            &&attack.skillEnabled&& true==data.UseStamina(30f))
        {            
            StartCoroutine(Rolling());
        }

        //경사면을 오르기 위한 검출
        Debug.DrawRay(transform.position, Vector3.down * 0.3f, Color.red);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.3f))
        {
            if (hit.collider.CompareTag("Stairs"))
            {
                
                rigid.useGravity = false;                
                rigid.velocity = Vector3.zero;
            }
            else
            {                
                rigid.useGravity = true;
            }
        }
        else
        {
            rigid.useGravity = true;
        }

        // 해당 속도 이상을 가지고 있어야 발소리를 플레이한다.
        float minimumFootstepSoundVelocity = isRun ? 0.5f : 0.1f;

        if (Mathf.Abs(moveInputX) >= minimumFootstepSoundVelocity
            || Mathf.Abs(moveInputZ) >= minimumFootstepSoundVelocity)
        {
            PlayFootstepSound();
        }
    }

    private void PlayFootstepSound()
    {
        if (isRolling || attack.isActing)
        {
            //audioSource.Stop();
            return;
        }

        footstepTimer += Time.deltaTime;
        float interval = isRun ? .35f : 0.5f;

        if (footstepTimer > interval)
        {
            audioSource.PlayOneShot(footstepClip);
            footstepTimer = 0;
        }
    }

    #region // 카메라 회전 및 각 제어
    //카메라 회전
    private void RotateCamera()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = cameraPoint.rotation.eulerAngles;

        float x = camAngle.x - mouseDelta.y;
        //카메라 회전각 제어
        if (x < 180f)
        {
            x = Mathf.Clamp(x, -1f,60f);
        }
        else
        {
            x = Mathf.Clamp(x, 320f, 361f);
        }
        cameraPoint.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, 0f);


        //평시 카메라 따라오게 하기
        


    }
    #endregion

    #region // 적을 체크하여 적을 바라볼 카메라 제어(GameObject _enemy)
    private void CheckEnemyCamera(GameObject _enemy)
    {
        if (_enemy == null)
        {
            return;
        }
        //플레이어와 적의 위치 계산 후 위치에 따른 카메라 위치 조정
        float distance = 5f;
        float upDistace = 2.5f;
        Vector3 directionEnemy = (_enemy.transform.position - transform.position).normalized;
        directionEnemy.y = 0;
        
        Vector3 cameraPosition = transform.position + (directionEnemy * -distance) + Vector3.up * upDistace;
        Debug.DrawRay(transform.position, directionEnemy * 40f, Color.red);
        lockOnCamera.position = cameraPosition;
        lockOnCamera.forward = directionEnemy;
                
        //마우스 휠로 카메라 거리 조절 하기
    }
    #endregion


    #region // 코루틴 구르기 제어
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
        //다른 레이어 넣을 것.
        this.gameObject.tag = "Enemy";
        
        // 구르기 지속 시간
        while (timer < 1f)
        {
            timer += Time.deltaTime;
            if (timer > 0.1f && timer < 1f)
            {
                // 구르는 방향으로 이동                
                float distanceToMove = rollSpeed * Time.deltaTime;
                Vector3 newPosition = transform.position + directionRoll.normalized * distanceToMove;

                rigid.MovePosition(new Vector3(newPosition.x,transform.position.y, newPosition.z));
            }
            yield return null;
        }
        animator.SetTrigger("Default");
        isRolling = false;        
        yield return new WaitForSeconds(0.3f);
        this.gameObject.tag = "Player";

    }
    #endregion


    //각도 계산을 쉽게하기위해
    private Vector3 EulerToVector(float angle)
    {
        angle += moveCamera.eulerAngles.y;
        angle *= Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
    }

    #region // 카메라 기준 -45 ~ 45도 방향 적을 담는 메서드    
    private void TargetDetection()
    {
        Collider[] objs = Physics.OverlapSphere(transform.position, detectionDistance);
        targetList.Clear();
        
        float radianRange = Mathf.Cos((detectionAngle*0.7f) * Mathf.Deg2Rad);
       
       
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i].CompareTag("Enemy"))
            { 
                float targetRadian = Vector3.Dot(cameraPoint.forward,
                    (objs[i].transform.position - transform.position).normalized);
                if (targetRadian > radianRange)
                {
                    targetList.Add(objs[i].gameObject);                
                    Debug.DrawLine(transform.position, objs[i].transform.position, Color.black);

                }
            }
        }        
    }
    #endregion

    #region // 카메라 정면 기준 가장 가까운 적을 찾는 메서드(return GameObject)와 코루틴
    private GameObject GetClosestEnemyInFront()
    {
        
        if (haveTarget)
        {
            
            return targetEnemy;
        }
        else
        {
            StartCoroutine(GetclosersetInFrontDelay());
        }
        //Debug.Log(targetEnemy);
        return targetEnemy;
    }

    // 락온을 푸는 도중 타겟이 바뀌는 것을 방지하기 위해 코루틴으로 딜레이
    private IEnumerator GetclosersetInFrontDelay()
    {
        GameObject closestEnemy = null;
        float maxDot = -Mathf.Infinity;
        Vector3 cameraForward = cameraPoint.forward;
        Vector3 playerPosition = transform.position;

        foreach (GameObject enemy in targetList)
        {
            Vector3 directionToEnemy = enemy.transform.position - playerPosition;
            float targetRadian = Vector3.Dot(directionToEnemy.normalized, cameraForward.normalized);

            if (targetRadian > maxDot)
            {
                maxDot = targetRadian;
                closestEnemy = enemy;
            }
        }        
        targetEnemy = closestEnemy;
        yield return new WaitForSeconds(1f);
        
    }
    #endregion
}