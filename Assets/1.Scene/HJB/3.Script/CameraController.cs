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
    private bool stamina = true;


    [Header("ī�޶� �켱��������")]
    [SerializeField] private CinemachineVirtualCamera vCamera;

    [Header("������ ī�޶�")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [Header("ī�޶� ����")]
    [SerializeField] private Transform cameraPoint;

    [Header("lockOn ī�޶�")]
    [SerializeField] private Transform lockOnCamera;

    [Header("��� ī�޶�")]
    [SerializeField] private Transform moveCamera;

    [Header("Player Data")]
    [SerializeField] private PlayerData data;
    [SerializeField] private WeaponBase weaponBase;
    [SerializeField] private PlayerAttack attack;
    private Collider _collider;
    //�÷��̾��� ���� ����
    private bool haveTarget = false;
    private bool isRun = false;
    private bool isRolling = false;
    private bool state = false;

    //�÷��̾� forward�� ���ϱ����� ī�޶� ���Ⱚ
    private Vector3 lookForward;
    private Vector3 lookRight;
    private Vector3 moveDir;

    //��ƾ� �� ���� ����
    [SerializeField] private float detectionDistance = 50f;
    private float detectionAngle = 90f;
    public List<GameObject> targetList = new List<GameObject>();

    [Header("�÷��̾�")]
    public Transform player;

    //���� Ÿ���� �� ������Ʈ ����
    [SerializeField]private GameObject targetEnemy = null;

    private Animator animator;
    private Rigidbody rigidbody;

    private float moveInputX;
    private float moveInputZ;

    //�ִϸ��̼� Blend�� ������ ����
    float _percent;
    float percent;
    //�߷�

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
    

    //���⼭ ��� ���¸� �ϳ��� ��� ������ �ؾ��ϳ�
    public void StateCheck()
    {
        //weaponBase.canDamageEnemy �� c�� C�� ����
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
        //��üũī�޶�
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
            //������ Ȱ��ȭ �����̸� ���� ã��
            vCamera.Priority = 5;
        }
        else
        {   //������ ��Ȱ��ȭ�� ī�޶� ȸ���� �����ϰ� ī�޶��� ��������� null�� ��ȯ

            RotateCamera();
            vCamera.Priority = 20;
        }
    }
    
    private void move()
    {   
        //�÷��̾� �̵��ӵ� ����
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

            //rigidbody.velocity = (forward * moveInputZ + right * moveInputX) * finalSpeed;

            

            // �ٶ󺸴� �������� ȸ�� �� �ٽ� ������ �ٶ󺸴� ������ ���� ���� ����
            if (!(moveInputX == 0 && moveInputZ == 0))
            {
                // �̵��� ȸ���� �Բ� ó��
                rigidbody.MovePosition(transform.position + (forward * moveInputZ + right * moveInputX) * finalSpeed * Time.deltaTime);
                // ȸ���ϴ� �κ�. Point 1.
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * (10f+finalSpeed));
            }
        
        }

        //���¿� ���� �̵�
        if (!state && haveTarget)
        {
            //�÷��̾� �̵�
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
        //������        
        if (!state &&move&& Input.GetKeyDown(KeyCode.Space)&&stamina)
        {
            //stamina = data.UseStamina(10f);
            StartCoroutine(Rolling());
        }
    }

    //ī�޶� ȸ��
    private void RotateCamera()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = cameraPoint.rotation.eulerAngles;

        float x = camAngle.x - mouseDelta.y;
        //ī�޶� ȸ���� ����
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
    //���� üũ�Ͽ� ���� �ٶ� ī�޶� ����
    private void CheckEnemyCamera(GameObject _enemy)
    {

        if (_enemy == null)
        {
            return;
        }
        //���߿� ���� position ���ڷ� �޾ƿ� ��.

        //�÷��̾�� ���� ��ġ ��� �� ��ġ�� ���� ī�޶� ��ġ ����
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
        // ������ ���� �ð�
        while (timer < 1f)
        {
            timer += Time.deltaTime;
            if (timer > 0.1f && timer < 1f)
            {
                // ������ �������� ���������� �̵�
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

    //ī�޶� ���� ���� ���� ���� ��� �޼���
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
    //ī�޶� ���� ���� ���� ����� ���� ã�� �޼���
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
        //ī�޶� ��ȯ�߿� �ٸ�Ÿ���� �����ʵ��� �ϱ����� ������
        yield return new WaitForSeconds(2f);
        GameObject closestEnemy = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 cameraForward = moveCamera.forward;
        Vector3 playerPosition = player.position;

        foreach (GameObject enemy in targetList)
        {
            //���� ���� �� �� �����ϰ� �ٲ� ��
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