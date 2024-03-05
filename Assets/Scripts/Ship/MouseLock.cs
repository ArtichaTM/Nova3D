using R3;
using UnityEngine;

public class MouseLock : MonoBehaviour
{
    public ReactiveProperty<Vector2> MouseDelta {
        get; private set;
    } = new(Vector2.zero);

    readonly SerialDisposable disposable = new();

    void OnEnable() {
        Cursor.lockState = CursorLockMode.Locked;
        disposable.Disposable = Observable
            .EveryUpdate()
            .Subscribe(_ => MouseDelta.Value=new Vector2(
                Input.GetAxis("Mouse X")*Settings.InvertMouseHorizontal(),
                Input.GetAxis("Mouse Y")*Settings.InvertMouseVertical()
            ));
    }

    void OnDisable() {
        Cursor.lockState = CursorLockMode.None;
        disposable.Dispose();
    }
}
