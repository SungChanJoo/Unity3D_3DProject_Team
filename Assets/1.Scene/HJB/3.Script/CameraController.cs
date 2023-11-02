using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [Header("�÷��̾� �ӵ�")]
    [SerializeField] private float Speed = 5f;
    [SerializeField] private float isRunSpeed = 10f;
     private float finalSpeed;

    [Header("ī�޶� �켱��������")]
    [SerializeField] private CinemachineVirtualCamera vCamera;

    [Header("������ ī�޶�")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [Header("ī�޶� ����")]
    [SerializeField] private Transform cameraPoint;
    
    private bool haveTarget = false;
    private bool isRun = false;
    private bool isRolling = false;
    

    [Header("���")]
    public Transform player;
    public Transform enemy;

    Animator animator;
    //�ִϸ��̼� Blend�� ������ ����
    float _percent;
    float percent;
    //���� �ӵ�
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
        {   //������ Ȱ��ȭ �����̸� ���� ã��
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
        
        //�÷��̾��� forward���� ī�޶��� foward�� ��ġ
        Vector3 lookForward = new Vector3(cameraPoint.forward.x, 0f, cameraPoint.forward.z).normalized;
        Vector3 lookRight = new Vector3(cameraPoint.right.x, 0f, cameraPoint.right.z).normalized;
        Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;
        if (move)
        {
            player.forward = lookForward;

        }

        if (move&&!isRolling)
        {                
            //�÷��̾� �̵�
            transform.position += moveDir * Time.deltaTime * finalSpeed;
            //�ִϸ��̼��� Blend�� ó��
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


        //������
            
        
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
        //ī�޶� ȸ���� ����
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
        //���߿� ���� position �޾ƿ� ��.
        //�÷��̾�� ���� ��ġ ��� �� ��ġ�� ���� ī�޶� ��ġ ����
        //Vector3 directionEneny = (enemy.position - player.position).normalized;
        //Vector3 DirectionEnemy = -directionEneny;
        //Vector3 cameraPosition = vCamera.position + (DirectionEnemy * distance) + Vector3.up *6f;
        //Debug.DrawRay(transform.position, DirectionEnemy * 5f, Color.red);
        //vCamera.position = cameraPosition;
        //���������� �÷��̾��� foward����
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

        // ������ ���� ���� ���ó�
        Vector3 rollDirection;
        if (Mathf.Abs(_percent) > Mathf.Abs(percent))
        {
            if (_percent > 0)
            {
                
                Debug.Log("��");
                rollDirection = cameraPoint.right.normalized;
            }
            else
            {
                Debug.Log("�Ʒ�");                
                rollDirection = -cameraPoint.right.normalized;
            }
        }
        else
        {
            if (percent > 0)
            {
                Debug.Log("��");                
                rollDirection = cameraPoint.forward.normalized;

            }
            else
            {
                Debug.Log("��");                
                rollDirection = -cameraPoint.forward.normalized;
            }
        }        
        float timer = 0f;

        // ������ ���� �ð�
        while (timer < 1f) 
        {
            timer += Time.deltaTime;
            if (timer>0.2f)
            {
            
            
                // ������ �������� ���������� �̵�
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