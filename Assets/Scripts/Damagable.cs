using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Damagable : MonoBehaviour
{
    public float maxHealth = 10;
    public float health;

    public delegate void OnDead();

    public event OnDead onDead = () => { };
    private void Start()
    {
        health = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            onDead();
            Destroy(gameObject);
        }
    }

    public void Heal(float heal)
    {
        health += heal;
    }
}
