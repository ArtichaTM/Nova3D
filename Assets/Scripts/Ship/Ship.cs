using UnityEngine;
using R3;
using UnityEngine.Assertions;

public class Ship : MonoBehaviour
{
    #region Components
    MouseLock mouseLock;
    Rigidbody ribi;
    #endregion

    #region Disposables
    readonly CompositeDisposable Disposables = new();
    CompositeDisposable PauseDisposables = new();
    #endregion

    #region Variables
    public ReactiveProperty<float> HorizontalSpeed {get; private set;} = new(100f);
    ReactiveProperty<float> AppliedHorizontalSpeed = new();
    public ReactiveProperty<float> RotationSpeed {get; private set;} = new(100f);
    ReactiveProperty<float> AppliedRotationSpeed = new();
    #endregion

    void Start()
    {
        mouseLock = GetComponent<MouseLock>();
        ribi = GetComponent<Rigidbody>();
        MainLogic.instance.Paused
            .Where(x => x == true)
            .Subscribe(_ => PauseGame())
            .AddTo(Disposables);
        MainLogic.instance.Paused
            .Where(x => x == false)
            .Subscribe(_ => ResumeGame())
            .AddTo(Disposables);
        MainLogic.instance.Finished
            .Where (x => x == true)
            .Subscribe(_ => FinishGame())
            .AddTo(Disposables);
        HorizontalSpeed
            .Subscribe(_ => RecalculateHorizontalSpeed())
            .AddTo(Disposables);
        RotationSpeed
            .Subscribe(_ => RecalculateRotationSpeed())
            .AddTo(Disposables);
    }

    void PauseGame() {
        mouseLock.enabled = false;
        PauseDisposables.Dispose();
        PauseDisposables = new();
    }

    void RecalculateHorizontalSpeed() {
        AppliedHorizontalSpeed.Value = HorizontalSpeed.Value * Time.fixedDeltaTime;
    }

    void RecalculateRotationSpeed() {
        AppliedRotationSpeed.Value = RotationSpeed.Value * Time.fixedDeltaTime;
    }

    void ObservableFixedUpdate() {
        if (Input.GetKey(KeyCode.W)) {
            ribi.AddRelativeForce(0f, 0f, AppliedHorizontalSpeed.Value);
        }
        else if (Input.GetKey(KeyCode.S)) {
            ribi.AddRelativeForce(0f, 0f, -AppliedHorizontalSpeed.Value);
        }
    }

    void ResumeGame()
    {
        mouseLock.enabled = true;
        mouseLock.MouseDelta
            .Subscribe((Vector2 delta) => {
                ribi.AddRelativeTorque(
                    delta.y * AppliedRotationSpeed.Value,
                    delta.x * AppliedRotationSpeed.Value,
                    0f
                );
            })
            .AddTo(PauseDisposables);
        Observable
            .EveryUpdate(UnityFrameProvider.FixedUpdate)
            .Subscribe(_ => ObservableFixedUpdate())
            .AddTo(PauseDisposables);
    }

    void FinishGame() {
        Disposables.Dispose();
        if (!MiscellaneousFunctions.instance.IsIntroAnimating.Value) {
            MainLogic.instance.MainCamera.transform.parent = null;
        }
    }

    void OnDestroy() => FinishGame();
}
