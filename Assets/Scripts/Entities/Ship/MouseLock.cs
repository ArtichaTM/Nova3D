using R3;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ShipParameters))]
public class MouseLock : MonoBehaviour
{
    Rigidbody ribi;
    ShipParameters parameters;

    CompositeDisposable PauseDisposables = new();
    CompositeDisposable Disposables = new();

    void Awake() {
        ribi = GetComponent<Rigidbody>();
        parameters = GetComponent<ShipParameters>();
        Disposables.Dispose();
        Disposables = new();
        MainLogic.Instance.Paused
            .Skip(1)
            .Where(x => x == true)
            .Subscribe(_ => PauseGame())
            .AddTo(Disposables)
            ;
        MainLogic.Instance.Paused
            .Where(x => x == false)
            .Subscribe(_ => ResumeGame())
            .AddTo(Disposables)
            ;
    }

    void ResumeGame() {
        Cursor.lockState = CursorLockMode.Locked;
        Observable
            .EveryUpdate()
            .Subscribe(_ => ribi.AddRelativeTorque(
                Input.GetAxis("Mouse X")*Settings.InvertMouseHorizontal() * parameters.SpeedRotationYaw.Value,
                Input.GetAxis("Mouse Y")*Settings.InvertMouseVertical() * parameters.SpeedRotationPitch.Value,
                0f
            ))
            .AddTo(PauseDisposables)
            ;
    }

    void PauseGame() {
        Cursor.lockState = CursorLockMode.None;
        PauseDisposables.Dispose();
        PauseDisposables = new();
    }

    void OnDestroy() {
        if (!PauseDisposables.IsDisposed) PauseDisposables.Dispose();
        Disposables.Dispose();
    }
}
