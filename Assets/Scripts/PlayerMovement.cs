using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5.0f;
    public float dashSpeed = 10.0f;
    public float radiusOfJumpHit = 3f;
    public float jumpForce = 10f;
    public float accelerationTime = 0.2f;
    public float decelerationTime = 0.2f;
    public float dashHeight = 0.1f;
    public float distanceOfSphereCast = 0.5f;
    public float radiusOfSphereCast = 1;
    public float dashDamage = 10;
    private Quaternion angle = Quaternion.Euler(0, 45, 0);
    public GameObject dashDestinationMarker;
    public GameObject Marker;
    public Gun gun;
    public GameObject powerLandingEffect;
    public AudioClip jumpSound;
    public AudioClip landingSound;
    public AudioClip dashSound;
    private Rigidbody _rigidbody;
    private AudioSource _audioSource;
    private bool isDashing;
    
    private float timeOfStart;
    private Vector3 dashTargetPosition;
    private bool startDashing = false;
    private bool startJumping = false;
    private bool isJumping = false;
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if ((Input.GetMouseButtonDown(1)|| Input.GetKeyDown(KeyCode.LeftShift)) && !isDashing)
        {
            startDashing = true;
            _audioSource.PlayOneShot(dashSound);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            startJumping = true;
            isJumping = true;
        }
    }

    private void FixedUpdate()
    {
        _rigidbody.useGravity = !isDashing;
        var isOnGround = Physics.BoxCast(transform.position + Vector3.up*0.15f, new Vector3(0.5f, 0.1f, 0.5f), Vector3.down, transform.rotation, 0.2f);
        if(isOnGround && isJumping && !startJumping){
            _audioSource.PlayOneShot(landingSound);
            Destroy(Instantiate(powerLandingEffect, transform.position, transform.rotation), 1);
            var hits = Physics.SphereCastAll(transform.position + Vector3.up*0.5f, radiusOfJumpHit, Vector3.down);
            foreach (var hit in hits)
            {
                if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy")){
                    var component = hit.collider.gameObject.GetComponentInParent<Damagable>();
                    if(component != null){
                        if(transform.position.y - hit.collider.transform.position.y <= 1){
                            component.TakeDamage(dashDamage);
                        }
                    }
                }
            }
            isJumping = false;
        }
        if (!isDashing)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(horizontal, 0, vertical) * speed;
            if(startJumping && isOnGround){
                _rigidbody.AddForce(jumpForce*Vector3.up, ForceMode.Impulse);
                _audioSource.PlayOneShot(jumpSound);
            }
            movement = angle * movement;
            movement.y = _rigidbody.velocity.y;
            _rigidbody.velocity = movement;
        }
        HandleMouse();
        startJumping = false;
    }

    private void HandleMouse()
    {
        var mousePosition = Input.mousePosition;
        var ray = Camera.main.ScreenPointToRay(mousePosition);
        var plane = new Plane(Vector3.up, transform.position);
        plane.Raycast(ray, out var distance);
        var dashPosition = ray.GetPoint(distance);
        var physicsRay = new Ray(dashPosition + Vector3.up * 100, Vector3.down);
        var didHit = Physics.Raycast(physicsRay, out var hit, Mathf.Infinity,
            LayerMask.GetMask("Default"));
        Marker.transform.position = didHit ? hit.point : dashPosition;
        dashDestinationMarker.SetActive(didHit && Marker.transform.position.y <= dashPosition.y + 0.001f);
        transform.LookAt(dashPosition);
        gun.transform.LookAt(Marker.transform.position + Vector3.up*0.5f);
        if (startDashing)
        {
            isDashing = true;
            startDashing = false;
            timeOfStart = Time.time;
            transform.position += Vector3.up * dashHeight;
            dashTargetPosition = dashPosition + Vector3.up * dashHeight;
        }
        

        if (Input.GetMouseButton(0))
        {
            gun.Shoot();
        }

        if (isDashing)
        {
            Dash();
        }
    }

    private void Dash()
    {
        var currentSpeed = dashSpeed * Mathf.Clamp01((Time.time - timeOfStart) / accelerationTime);
        Vector3 movement = dashTargetPosition - transform.position;
        foreach (var sphereHit in Physics.SphereCastAll(transform.position + Vector3.up*(radiusOfSphereCast+0.1f) + Vector3.forward*0.5f, radiusOfSphereCast, movement,
                     distanceOfSphereCast, LayerMask.GetMask("Enemy", "Default")))
        {
            if (sphereHit.collider.gameObject.layer == LayerMask.NameToLayer("Default") )
            {
                isDashing = false;
                return;
            }
            DamageEnemy(sphereHit.collider);
        }

        if (movement.magnitude < currentSpeed * Time.fixedDeltaTime)
        {
            _rigidbody.velocity = movement.normalized * Time.fixedDeltaTime;
            isDashing = false;
            return;
        }

        _rigidbody.velocity = movement.normalized * currentSpeed;
    }

    private void DamageEnemy(Collider other)
    {
        var damagable = other.GetComponentInParent<Damagable>();
        damagable.TakeDamage(dashDamage);
    }
}