using UnityEngine;
using R3;
using System;

public class ShipInGame : MonoBehaviour
{
    public Rigidbody rb {
        get; private set;
    }

    public MouseLock mouseLock {
        get; private set;
    }

    public ReactiveProperty<float> maxCooldown = new(0f);
    public ReactiveProperty<float> cooldown = new(0f);
    public ReactiveProperty<float> speedPosition = new(1f);
    public ReactiveProperty<float> speedRotation = new(1f);
    public CompositeDisposable disposable = new();

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mouseLock = GetComponent<MouseLock>();
        mouseLock.OnAxisChange += RotateShip;
        Pause();
    }

    void Update()
    {
        if (cooldown.Value > 0) {
            cooldown.Value -= Time.deltaTime;
        }
        if (Input.GetMouseButton(0) && !IsReloading()) {
            Shot();
        }
    }

    void FixedUpdate() {
        if (Input.GetKey(KeyCode.W)) {
            rb.AddRelativeForce(new Vector3(0f, 0f,  speedPosition.Value * Time.fixedDeltaTime));
            // rb.AddRelativeForce(new Vector3(0f, 0f,  speed * Time.fixedDeltaTime));
        }
        else if (Input.GetKey(KeyCode.S)) {
            rb.AddRelativeForce(new Vector3(0f, 0f, -speedPosition.Value * Time.fixedDeltaTime));
            // rb.AddRelativeForce(new Vector3(0f, 0f, -speed * Time.fixedDeltaTime));
        }
    }

    public void Pause() {
        mouseLock.enabled = false;
    }

    public void Resume() {
        rb = GetComponent<Rigidbody>();
        mouseLock = GetComponent<MouseLock>();
        mouseLock.enabled = true;
    }

    void RotateShip(float x, float y) {
        rb.AddTorque(y, x, 0f);
    }

    bool IsReloading() { return cooldown.Value > 0; }

    void Shot() {
        cooldown.Value = maxCooldown.Value;
    }
}
