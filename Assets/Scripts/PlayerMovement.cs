using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    public float radiusOfDashDamage = 1.2f;
    public float dashDamage = 10;
    private Quaternion angle = Quaternion.Euler(0, 45, 0);
    public GameObject dashDestinationMarker;
    public GameObject Marker;
    public Gun gun;
    public GameObject gunPosition;
    public GameObject powerLandingEffect;
    public AnimationCurve screenShakeCurve;
    public float screenShakeDuration = 0.5f;
    public AudioClip jumpSound;
    public AudioClip landingSound;
    public AudioClip dashSound;
    public AudioClip drop;
    public AudioClip loot;
    private Rigidbody _rigidbody;
    private AudioSource _audioSource;
    private bool isDashing;
    private Damagable playerDamagable;
    private float timeOfStart;
    private Vector3 dashTargetPosition;
    private bool startDashing = false;
    private bool startJumping = false;
    private bool isJumping = false;
    private CameraMover cameraMover;
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        cameraMover = FindAnyObjectByType<CameraMover>();
        playerDamagable = GetComponent<Damagable>();
    }

    private void Update()
    {
        if ((Input.GetMouseButtonDown(1)|| Input.GetKeyDown(KeyCode.LeftAlt)) && !isDashing)
        {
            startDashing = true;
            _audioSource.PlayOneShot(dashSound);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            startJumping = true;
            isJumping = true;
        }
        if(gun != null && Input.GetKeyDown(KeyCode.Q)){
            DropGun();
        }
    }

    private void FixedUpdate()
    {
        _rigidbody.useGravity = !isDashing;
        var isOnGround = Physics.BoxCast(transform.position + Vector3.up*0.15f, new Vector3(0.5f, 0.1f, 0.5f), Vector3.down, transform.rotation, 0.2f);
        if(isOnGround && isJumping && !startJumping){
            cameraMover.ScreenShake(screenShakeDuration, screenShakeCurve);
            _audioSource.PlayOneShot(landingSound);
            Destroy(Instantiate(powerLandingEffect, transform.position, transform.rotation), 0.5f);
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
            movement.y = _rigidbody.linearVelocity.y;
            _rigidbody.linearVelocity = movement;
        }
        HandleMouse();
        startJumping = false;
    }
    void OnCollisionEnter(Collision other)
    {
        if(other.transform.gameObject.layer == LayerMask.NameToLayer("Loot") && (gun == null || gun.ammo == 0)){
            if(gun != null){
                DropGun();
            }
            _audioSource.PlayOneShot(loot);
            gun = other.gameObject.GetComponent<Gun>();
            gun.transform.parent = transform;
            Utils.SetLayer(gun.gameObject, "Player");
            var gun_rb = gun.GetComponent<Rigidbody>();
            gun_rb.isKinematic = true;
            gun.transform.position = gunPosition.transform.position;
        }
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
        if(gun != null){
            gun.transform.LookAt(Marker.transform.position + Vector3.up*0.5f);
        }
        if (startDashing)
        {
            isDashing = true;
            startDashing = false;
            timeOfStart = Time.time;
            transform.position += Vector3.up * dashHeight;
            dashTargetPosition = dashPosition + Vector3.up * dashHeight;
        }
        

        if (Input.GetMouseButton(0) && gun != null)
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
        foreach (var _ in Physics.BoxCastAll(transform.position + Vector3.up*0.6f, Vector3.one/2, movement, transform.rotation,
                    currentSpeed*Time.fixedDeltaTime , LayerMask.GetMask("Default")))
        {
            isDashing = false;
            return;
        }
        foreach (var sphereHit in Physics.SphereCastAll(transform.position + Vector3.up*radiusOfDashDamage + Vector3.forward*0.5f, radiusOfDashDamage, movement,
                     distanceOfSphereCast, LayerMask.GetMask("Enemy")))
        {
            DamageEnemy(sphereHit.collider);
            cameraMover.ScreenShake(screenShakeDuration, screenShakeCurve);
        }
        if (movement.magnitude < currentSpeed * Time.fixedDeltaTime)
        {
            _rigidbody.linearVelocity = movement / Time.fixedDeltaTime;
            isDashing = false;
            return;
        }

        _rigidbody.linearVelocity = movement.normalized * currentSpeed;
    }

    private void DamageEnemy(Collider other)
    {
        var damagable = other.GetComponentInParent<Damagable>();
        playerDamagable.Heal(damagable.health);
        damagable.TakeDamage(dashDamage);
    }
    private void DropGun(){
        _audioSource.PlayOneShot(drop);
        gun.transform.parent = null;
        Utils.SetLayer(gun.gameObject, "PlayerBullet");
        var gun_rb =  gun.GetComponent<Rigidbody>();
        gun_rb.isKinematic = false;
        gun.GetComponent<Bullet>().enabled = true;
        gun = null;
    }
}