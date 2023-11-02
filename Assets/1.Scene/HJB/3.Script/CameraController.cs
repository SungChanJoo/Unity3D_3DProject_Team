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

    [Header("Follow Object")]
    [SerializeField] private Transform vCamera;

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
        {   //������ Ȱ��ȭ �����̸� ���� ã��
            CheckEnemy();
        }
        else
        {   //������ ��Ȱ��ȭ�� ī�޶� ȸ���� �����ϰ� ī�޶��� ��������� null�� ��ȯ
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
            //�÷��̾��� forward���� ī�޶��� foward�� ��ġ
            player.forward = lookForward;
            //�÷��̾� �̵�
            transform.position += moveDir * Time.deltaTime * finalSpeed;
        }
        //�ִϸ��̼��� Blend�� ó��
        
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
        float distance = 5f;

        //�÷��̾�� ���� ��ġ ��� �� ��ġ�� ���� ī�޶� ��ġ ����
        Vector3 directionEneny = (enemy.position - player.position).normalized;
        Vector3 DirectionEnemy = -directionEneny;
        Vector3 cameraPosition = vCamera.position + (DirectionEnemy * distance) + Vector3.up * 3f;

        vCamera.position = cameraPosition;
        //���������� �÷��̾��� foward����
        player.forward = directionEneny * player.position.y;
    }

    
    

}