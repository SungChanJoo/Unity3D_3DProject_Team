using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTest : MonoBehaviour
{


        private void OnParticleCollision(GameObject other)
        {
            Debug.Log("플레이어 데미지 받음!");
        }
}   
