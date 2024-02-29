using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipInGame : MonoBehaviour
{
    public Rigidbody rb {
        get; private set;
    }
    private float cooldown = 0f;
    public float maxCooldown = 3f;
    public float speed = 10f;
    public float _Angle = 90;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
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
            rb.AddRelativeForce(new Vector3(0, speed, 0));
        }
        else if (Input.GetKey(KeyCode.S)) {
            rb.AddRelativeForce(new Vector3(0, -speed, 0));
        }
    }

    // void OnDisable() {
    // }

    void OnEnable() {
        rb = GetComponent<Rigidbody>();
    }

    bool IsReloading() { return cooldown > 0; }

    void Shot() {
        cooldown = maxCooldown;

    }
}
