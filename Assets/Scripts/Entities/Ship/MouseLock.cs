using R3;
using UnityEngine;

public class MouseLock : MonoBehaviour
{
    public ReactiveProperty<Vector2> MouseDelta {
        get; private set;
    } = new(Vector2.zero);

    SerialDisposable PauseDisposables = new();
    CompositeDisposable Disposables = new();

    void Awake() {
        Disposables.Dispose();
        Disposables = new();
        MainLogic.instance.Paused
            .Skip(1)
            .Where(x => x == true)
            .Subscribe(_ => PauseGame())
            .AddTo(Disposables)
            ;
        MainLogic.instance.Paused
            .Where(x => x == false)
            .Subscribe(_ => ResumeGame())
            .AddTo(Disposables)
            ;
    }

    void ResumeGame() {
        Cursor.lockState = CursorLockMode.Locked;
        PauseDisposables.Disposable = Observable
            .EveryUpdate()
            .Subscribe(_ => MouseDelta.Value=new Vector2(
                Input.GetAxis("Mouse X")*Settings.InvertMouseHorizontal(),
                Input.GetAxis("Mouse Y")*Settings.InvertMouseVertical()
            ))
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
