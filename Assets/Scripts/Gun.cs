using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class Gun : MonoBehaviour
{
    public float fireRate = 0.2f;
    public GameObject bullet;
    public float spread = 5;
    public int amountOfBullets = 1;
    public GameObject bulletSpawn;
    private float timeOfLastShot;

    private void Start()
    {
        timeOfLastShot = Time.time;
    }

    public void Shoot()
    {
        if (Time.time - timeOfLastShot < fireRate)
        {
            return;
        }

        for (int i = 0; i < amountOfBullets; i++)
        {
            var spreadRotation = Quaternion.Euler(0,Random.value*spread - spread/2, 0);
            Instantiate(bullet, bulletSpawn.transform.position, bulletSpawn.transform.rotation * spreadRotation);
            timeOfLastShot = Time.time;
        }
    } 
}
