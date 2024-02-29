using UnityEngine;
using System.Collections;
using System;
using Unity.Mathematics;
using Unity.VisualScripting;

public class Ship : MonoBehaviour
{
    private Rigidbody2D rb;
    private float cooldown = 0f;
    public float maxCooldown = 3f;
    public float speed;
    public float _Angle = 90;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void MoveForward() {
        rb.AddRelativeForce(new Vector2(0, 1f));
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
        MoveForward();
    }

    bool IsReloading() { return cooldown > 0; }

    void Shot() {
        cooldown = maxCooldown;

    }
}
