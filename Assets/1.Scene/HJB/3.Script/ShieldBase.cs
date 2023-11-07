using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBase : MonoBehaviour
{
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private Collider childrenCollider;
        
    private Collider _collider;

    private void Awake()
    {
        TryGetComponent(out _collider);        
        
    }
    private void Update()
    {
        ShieldOnCollider();
    }
    private void ShieldOnCollider()
    {
        if (playerAttack.hold)
        {            
            _collider.enabled = true;
            if (playerAttack.perfectParrying)
            {
                //StartCoroutine(ParryTiming());
            }

        }
        else
        {            
            _collider.enabled = false;
        }
    }
    private IEnumerator ParryTiming()
    {        
        childrenCollider.enabled = true;
        _collider.enabled = false;        

        yield return new WaitForSeconds(0.3f);
        childrenCollider.enabled = false;
        _collider.enabled = true;
        playerAttack.perfectParrying = false;
        
    }
    private void OnTriggerEnter(Collider other)
    {        
        if (!playerAttack.perfectParrying)
        {
            playerAttack.perfectParrying = true;
        }        
    }

}
