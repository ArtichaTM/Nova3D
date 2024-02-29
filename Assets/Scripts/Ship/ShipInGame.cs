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
            rb.AddRelativeForce(new Vector3(0, speed * Time.fixedDeltaTime, 0));
        }
        else if (Input.GetKey(KeyCode.S)) {
            rb.AddRelativeForce(new Vector3(0, -speed * Time.fixedDeltaTime, 0));
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

    bool IsReloading() { return cooldown > 0; }

    void Shot() {
        cooldown = maxCooldown;

    }
}
