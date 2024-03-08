using R3;
using UnityEngine;

public class MouseLock : MonoBehaviour
{
    public ReactiveProperty<Vector2> MouseDelta {
        get; private set;
    } = new(Vector2.zero);

    SerialDisposable PauseDisposable = new();
    CompositeDisposable Disposables;

    void Start() {
        Disposables = new();
        MainLogic.instance.Paused
            .Subscribe(x => enabled = !x)
            .AddTo(Disposables)
            ;
    }

    void OnEnable() {
        Cursor.lockState = CursorLockMode.Locked;
        PauseDisposable.Disposable = Observable
            .EveryUpdate()
            .Subscribe(_ => MouseDelta.Value=new Vector2(
                Input.GetAxis("Mouse X")*Settings.InvertMouseHorizontal(),
                Input.GetAxis("Mouse Y")*Settings.InvertMouseVertical()
            ));
    }

    void OnDisable() {
        Cursor.lockState = CursorLockMode.None;
        PauseDisposable.Dispose();
        PauseDisposable = new();
    }

    void OnDestroy() {
        Disposables.Dispose();
    }
}
