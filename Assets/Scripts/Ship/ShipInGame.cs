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

    // public ReactiveProperty<float> maxCooldown = new(0f);
    // public ReactiveProperty<float> cooldown = new(0f);
    public ReactiveProperty<float> speedPosition = new(40f);
    public ReactiveProperty<float> speedRotationMultiplier = new(1f);
    public ReactiveProperty<Vector2> shipMultipliers = new();

    public ReactiveProperty<Vector2> speedRotationSummary = new();

    public CompositeDisposable disposable = new();

    CompositeDisposable PauseDisposable = new();

    public void Start() {
        RiBo = GetComponent<Rigidbody>();
        MouseLock = GetComponent<MouseLock>();
    }

    // void Update()
    // {
    //     if (cooldown.Value > 0) {
    //         cooldown.Value -= Time.deltaTime;
    //     }
    //     if (Input.GetMouseButton(0) && !IsReloading()) {
    //         Shot();
    //     }
    // }

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
        PauseDisposable.Dispose();
    }

    public void Resume() {
        RiBo = GetComponent<Rigidbody>();
        MouseLock = GetComponent<MouseLock>();
        Vector3 coefficients = GetComponent<Collider>().bounds.extents.normalized;
        shipMultipliers.Value = new Vector2(coefficients.y, coefficients.x) * 20;
        MouseLock.enabled = true;
        MouseLock.MouseDelta.Subscribe(RotateShip).AddTo(PauseDisposable);
        Observable
            .EveryUpdate(UnityFrameProvider.FixedUpdate)
            .Subscribe(_ => CheckInputs())
            .AddTo(PauseDisposable);
        speedRotationMultiplier
            .Subscribe((float mult) => {
                speedRotationSummary.Value=CalculateRotationSpeed(mult, shipMultipliers.Value);
            })
            .AddTo(disposable);
        shipMultipliers
            .Subscribe((Vector2 extents) => {
                speedRotationSummary.Value=CalculateRotationSpeed(speedRotationMultiplier.Value, extents);
            })
            .AddTo(disposable);
    }

    void RotateShip(Vector2 delta) {
        RiBo.AddRelativeTorque(
            delta.y*speedRotationSummary.Value.y,
            delta.x*speedRotationSummary.Value.x,
            0f
        );
    }

    static Vector2 CalculateRotationSpeed(float multiplier, Vector2 extents) => multiplier*extents;

    // bool IsReloading() { return cooldown.Value > 0; }

    // void Shot() {
    //     cooldown.Value = maxCooldown.Value;
    // }
}
