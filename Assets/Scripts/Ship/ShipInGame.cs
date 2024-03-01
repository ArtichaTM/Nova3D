using UnityEngine;
using R3;
using System;

public class ShipInGame : MonoBehaviour
{
    public Rigidbody RiBo {
        get; private set;
    }

    public MouseLock MouseLock {
        get; private set;
    }

    public ReactiveProperty<float> maxCooldown = new(0f);
    public ReactiveProperty<float> cooldown = new(0f);
    public ReactiveProperty<float> speedPosition = new(1f);
    public ReactiveProperty<float> speedRotation = new(1f);
    public CompositeDisposable disposable = new();

    void Start()
    {
        RiBo = GetComponent<Rigidbody>();
        MouseLock = GetComponent<MouseLock>();
        MouseLock.OnAxisChange += RotateShip;
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

    void CheckInputs() {
        if (Input.GetKey(KeyCode.W)) {
            RiBo.AddRelativeForce(new Vector3(0f, 0f,  speedPosition.Value * Time.fixedDeltaTime));
        }
        else if (Input.GetKey(KeyCode.S)) {
            RiBo.AddRelativeForce(new Vector3(0f, 0f, -speedPosition.Value * Time.fixedDeltaTime));
        }
    }

    public void Pause() {
        MouseLock.enabled = false;
    }

    public void Resume() {
        RiBo = GetComponent<Rigidbody>();
        MouseLock = GetComponent<MouseLock>();
        MouseLock.enabled = true;
    }

    void RotateShip(float x, float y) {
        RiBo.AddTorque(y, x, 0f);
    }

    bool IsReloading() { return cooldown.Value > 0; }

    void Shot() {
        cooldown.Value = maxCooldown.Value;
    }
}
