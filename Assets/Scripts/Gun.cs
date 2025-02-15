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
    public int ammo = 30;
    public AudioClip shot;
    public GameObject bulletSpawn;
    private AudioSource _audioSource;
    public GameObject shotEffect;
    private float timeOfLastShot;

    private void Start()
    {
        timeOfLastShot = Time.time;
        _audioSource = GetComponent<AudioSource>();
    }

    public void Shoot()
    {
        if (Time.time - timeOfLastShot < fireRate || ammo <= 0)
        {
            return;
        }
        ammo -= 1;
        for (int i = 0; i < amountOfBullets; i++)
        {
            _audioSource.PlayOneShot(shot);
            var spreadRotation = Quaternion.Euler(0,Random.value*spread - spread/2, 0);
            Destroy(Instantiate(bullet, bulletSpawn.transform.position, bulletSpawn.transform.rotation * spreadRotation), 10);
            Destroy(Instantiate(shotEffect, bulletSpawn.transform.position, bulletSpawn.transform.rotation), 0.1f);
            timeOfLastShot = Time.time;
        }
    } 
}
