using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Enemy/MonsterData", fileName = "MonsterData")]
public class MonsterData : ScriptableObject
{
    public float MaxHealth = 100f;      //�ִ�ü��
    public float Damage = 20f;          //������
    public float Force = 0f;            //�̴���
    public float Speed = 2f;            //���ǵ�
    public float AttackDistance = 2f;   //���ݰŸ�
    public float TimegetAttack = 2.267f;  //���ݼӵ�
    public float DetectRange = 5f;    //�÷��̾� ��������

}
