using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipInGame : MonoBehaviour
{
    public Rigidbody rb {
        get; private set;
    }

    public MouseLock mouseLock {
        get; private set;
    }

    private float cooldown = 0f;
    public float maxCooldown = 3f;
    public float speed = 10f;
    public float _Angle = 90;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mouseLock = GetComponent<MouseLock>();
        mouseLock.OnAxisChange += RotateShip;
        enabled = false;
    }

    void Update()
    {
        if (cooldown > 0) {
            cooldown -= Time.deltaTime;
        }
        if (Input.GetMouseButton(0) && !IsReloading()) {
            Shot();
        }
    }

    void FixedUpdate() {
        if (Input.GetKey(KeyCode.W)) {
            rb.AddRelativeForce(new Vector3(0f, 0f,  speed * Time.fixedDeltaTime));
        }
        else if (Input.GetKey(KeyCode.S)) {
            rb.AddRelativeForce(new Vector3(0f, 0f, -speed * Time.fixedDeltaTime));
        }
    }

    void OnEnable() {
        rb = GetComponent<Rigidbody>();
        mouseLock = GetComponent<MouseLock>();
        mouseLock.enabled = true;
    }

    void OnDisable() {
        mouseLock.enabled = false;
    }

    void RotateShip(float x, float y) {
        rb.AddTorque(y, x, 0f);
    }

    bool IsReloading() { return cooldown > 0; }

    void Shot() {
        cooldown = maxCooldown;

    }
}
