using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Enemy/EnemyData", fileName = "EnemyData")]
public class EnemyData : ScriptableObject
{
    public float MaxHealth = 100f;      //최대체력
    public float Damage = 20f;          //데미지
    public float Force = 0f;            //미는힘
    public float Speed = 2f;            //스피드
    public float AttackDistance = 2f;   //공격거리
    public float TimegetAttack = 2.267f;  //공격속도
    public float DetectRange = 5f;    //플레이어 감지범위

}
