using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bullet : MonoBehaviour
{
    public float damage = 5;
    public float speed = 20;
    public int piercing = 0;
    private void OnEnable()
    {
        var ridgidbody = GetComponent<Rigidbody>();
        ridgidbody.AddForce(transform.forward*speed, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other)
    {   
        if(!enabled){
            return;
        }
        var _damagable = other.collider.gameObject.GetComponentInParent<Damagable>();
        if (_damagable != null)
        {
            _damagable.TakeDamage(damage);
        }

        if (piercing > 0)
        {
            piercing -= 1;
            return;
        }
        Destroy(gameObject);
    }
}
