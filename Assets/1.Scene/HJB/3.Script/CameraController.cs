using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    //[Header("�÷��̾� �ӵ�")]    
    //[SerializeField] private float Speed = 5f;
    //[SerializeField] private float runSpeed = 8f;
    private float finalSpeed;    

    [Header("������ ī�޶� �� ī�޶� �켱��������(CM vcam1)")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [Header("ī�޶� ���� (Camera Arm)")]
    [SerializeField] private Transform cameraPoint;

    [Header("lockOn ī�޶� (CM vcam2)")]
    [SerializeField] private Transform lockOnCamera;

    [Header("��� ī�޶� (CM vcam1)")]
    [SerializeField] private Transform moveCamera;

    [Header("WeaponBase �߰�")]
    [SerializeField] private WeaponBase weaponBase;
    
    //�÷��̾��� ���� ����
    private bool haveTarget = false;
    private bool isRun = false;
    private bool state = false;
    public bool rolling = true;
    public bool isRolling = false;
    public bool isParalysed = false;

    //�÷��̾� forward�� ���ϱ����� ī�޶� ���Ⱚ
    private Vector3 lookForward;
    private Vector3 lookRight;
    private Vector3 moveDir;

    //��ƾ� �� ���� ����
    [SerializeField] private float detectionDistance = 50f;
    private float detectionAngle = 90f;
    private List<GameObject> targetList = new List<GameObject>();       

    //���� Ÿ���� �� ������Ʈ ����
    [SerializeField]private GameObject targetEnemy = null;

    //Ű �Է°� ���� ����
    private float moveInputX;
    private float moveInputZ;

    //�ִϸ��̼� Blend�� ������ ����
    float _percent;
    float percent;
    
    //������ �÷��̾� ������Ʈ
    private Collider _collider;
    private Animator animator;
    private Rigidbody rigid;
    private PlayerAttack attack;
    private PlayerData data;

    //LockOnTargetUI �̹���
    [SerializeField] private GameObject lockOnTargetUI;

    [Header("Audio �߰�")]
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

    //���⼭ ��� ���¸� �ϳ��� ��� ������ �ؾ��ϳ�
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
            Debug.Log($"���� : {state}");
            Debug.Log($"Ÿ�� : {haveTarget}");
        }
    }

    #region // ���� ����, Ÿ�� Ȯ��, ī�޶� �켱���� �Ѱ� �޼���
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
        //��üũī�޶�
        CheckEnemyCamera(GetClosestEnemyInFront());


        if (haveTarget)
        {
            //������ Ȱ��ȭ �����̸� ���� ã��
            virtualCamera.Priority = 5;
            lockOnTargetUI.SetActive(true);            
        }
        else
        {   //������ ��Ȱ��ȭ�� ī�޶� ȸ���� �����ϰ� ī�޶��� ��������� null�� ��ȯ
            virtualCamera.Follow = cameraPoint;
            RotateCamera();
            virtualCamera.Priority = 20;
            lockOnTargetUI.SetActive(false);
        }

        //target�� Player�� �Ÿ��� �־�����
        if (targetEnemy!=null)
        {
            float distance = (targetEnemy.transform.position - transform.position).sqrMagnitude;
            distance =Mathf.Sqrt(distance);
            if (distance>detectionDistance)
            {
                haveTarget = false;
            }                  
        }

        //��� ī�޶� ��ġ ����
        Vector3 cameraY = transform.position + new Vector3(0, 1f, 0);
        cameraPoint.position = Vector3.MoveTowards(cameraPoint.position, cameraY, 20f * Time.deltaTime);
    }
    #endregion

    private void move()
    {               
        //�÷��̾� �̵��ӵ� ����
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
        //���� �÷��̾ ���� �������� Ȯ��
        bool move = new Vector3(moveInputX,0,moveInputZ).magnitude != 0;        

        //�÷��̾��� forward���� ī�޶��� forward�� ��ġ
        if (!haveTarget &&move&&!isRolling)
        {
            if (!haveTarget && move && !isRolling)
            {
                lookForward = new Vector3(cameraPoint.forward.x, 0f, cameraPoint.forward.z).normalized;
                lookRight = new Vector3(cameraPoint.right.x, 0f, cameraPoint.right.z).normalized;

                // �÷��̾ ������ �� ī�޶� ������ ���󰡵��� ��
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

        //������ �ƴ� �� �̵�
        if (!haveTarget && !state)
        {

            var playerGroundPos = new Vector3(transform.position.x, 0, transform.position.z);
            var cameraGroundPos = new Vector3(Camera.main.transform.position.x, 0, Camera.main.transform.position.z);

            var cameraToPlayer = (playerGroundPos - cameraGroundPos);

            var forward = cameraToPlayer.normalized;
            var right = Vector3.Cross(Vector3.up, forward);

            Debug.DrawLine(transform.position, transform.position + forward * moveInputZ, Color.red);
            Debug.DrawLine(transform.position, transform.position + right * moveInputX, Color.blue);                    

            //�ٶ󺸴� �������� ȸ�� �� �ٽ� ������ �ٶ󺸴� ������ ���� ���� ����
            if (!(moveInputX == 0 && moveInputZ == 0))
            {
                //�̵��� ȸ���� �Բ� ó��
                rigid.MovePosition(transform.position + (forward * moveInputZ + right * moveInputX) * finalSpeed * Time.deltaTime);
                //ȸ���ϴ� �κ�
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * (10f+finalSpeed));
            }        
        }

        //���¿� ���� �̵�
        if (haveTarget&& !state)
        {
            //�÷��̾� �̵�
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
        //������        
        if (!state&&rolling && move && Input.GetKeyDown(KeyCode.Space)
            &&attack.skillEnabled&& true==data.UseStamina(30f))
        {            
            StartCoroutine(Rolling());
        }

        //������ ������ ���� ����
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

        // �ش� �ӵ� �̻��� ������ �־�� �߼Ҹ��� �÷����Ѵ�.
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

    #region // ī�޶� ȸ�� �� �� ����
    //ī�޶� ȸ��
    private void RotateCamera()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = cameraPoint.rotation.eulerAngles;

        float x = camAngle.x - mouseDelta.y;
        //ī�޶� ȸ���� ����
        if (x < 180f)
        {
            x = Mathf.Clamp(x, -1f,60f);
        }
        else
        {
            x = Mathf.Clamp(x, 320f, 361f);
        }
        cameraPoint.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, 0f);


        //��� ī�޶� ������� �ϱ�
        


    }
    #endregion

    #region // ���� üũ�Ͽ� ���� �ٶ� ī�޶� ����(GameObject _enemy)
    private void CheckEnemyCamera(GameObject _enemy)
    {
        if (_enemy == null)
        {
            return;
        }
        //�÷��̾�� ���� ��ġ ��� �� ��ġ�� ���� ī�޶� ��ġ ����
        float distance = 5f;
        float upDistace = 2.5f;
        Vector3 directionEnemy = (_enemy.transform.position - transform.position).normalized;
        directionEnemy.y = 0;
        
        Vector3 cameraPosition = transform.position + (directionEnemy * -distance) + Vector3.up * upDistace;
        Debug.DrawRay(transform.position, directionEnemy * 40f, Color.red);
        lockOnCamera.position = cameraPosition;
        lockOnCamera.forward = directionEnemy;
                
        //���콺 �ٷ� ī�޶� �Ÿ� ���� �ϱ�
    }
    #endregion


    #region // �ڷ�ƾ ������ ����
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
        //�ٸ� ���̾� ���� ��.
        this.gameObject.tag = "Enemy";
        
        // ������ ���� �ð�
        while (timer < 1f)
        {
            timer += Time.deltaTime;
            if (timer > 0.1f && timer < 1f)
            {
                // ������ �������� �̵�                
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


    //���� ����� �����ϱ�����
    private Vector3 EulerToVector(float angle)
    {
        angle += moveCamera.eulerAngles.y;
        angle *= Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
    }

    #region // ī�޶� ���� -45 ~ 45�� ���� ���� ��� �޼���    
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

    #region // ī�޶� ���� ���� ���� ����� ���� ã�� �޼���(return GameObject)�� �ڷ�ƾ
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

    // ������ Ǫ�� ���� Ÿ���� �ٲ�� ���� �����ϱ� ���� �ڷ�ƾ���� ������
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