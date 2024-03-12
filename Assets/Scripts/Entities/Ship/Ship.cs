using UnityEngine;
using R3;
using UnityEngine.Assertions;

public class Ship : MonoBehaviour
{
    #region Components
    Rigidbody ribi;
    #endregion

    #region Disposables
    CompositeDisposable Disposables;
    CompositeDisposable PauseDisposables = new();
    #endregion

    #region Variables
    public ReactiveProperty<float> HorizontalSpeed {get; private set;} = new(100f);
    public readonly ReactiveProperty<float> AppliedHorizontalSpeed = new();
    public ReactiveProperty<float> RotationSpeed {get; private set;} = new(100f);
    public readonly ReactiveProperty<float> AppliedRotationSpeed = new();
    #endregion

    void Start()
    {
        Disposables = new();
        ribi = GetComponent<Rigidbody>();
        MainLogic.Instance.Paused
            .Where(x => x == true)
            .Subscribe(_ => PauseGame())
            .AddTo(Disposables)
            ;
        MainLogic.Instance.Paused
            .Where(x => x == false)
            .Subscribe(_ => ResumeGame())
            .AddTo(Disposables)
            ;
        HorizontalSpeed
            .Subscribe(_ => RecalculateHorizontalSpeed())
            .AddTo(Disposables)
            ;
        RotationSpeed
            .Subscribe(_ => RecalculateRotationSpeed())
            .AddTo(Disposables)
            ;
    }
    void RecalculateHorizontalSpeed() => AppliedHorizontalSpeed.Value =
        HorizontalSpeed.Value
        *Time.fixedDeltaTime
        ;

    void RecalculateRotationSpeed() => AppliedRotationSpeed.Value =
        RotationSpeed.Value
        *Time.fixedDeltaTime
        ;

    void ObservableFixedUpdate() {
        if 
            (Input.GetKey(KeyCode.W))
            ribi.AddRelativeForce(0f, 0f, AppliedHorizontalSpeed.Value);
        else if
            (Input.GetKey(KeyCode.S))
            ribi.AddRelativeForce(0f, 0f, -AppliedHorizontalSpeed.Value);
        if
            (Input.GetKey(KeyCode.A))
            ribi.AddRelativeTorque(0f, 0f, AppliedRotationSpeed.Value/100);
        else if
            (Input.GetKey(KeyCode.D))
            ribi.AddRelativeTorque(0f, 0f, -AppliedRotationSpeed.Value/100);
    }

    void ResumeGame()
    {
        Observable
            .EveryUpdate(UnityFrameProvider.FixedUpdate)
            .Subscribe(_ => ObservableFixedUpdate())
            .AddTo(PauseDisposables)
            ;
    }

    void PauseGame() {
        PauseDisposables.Dispose();
        PauseDisposables = new();
    }

    void FinishGame() {
        Disposables.Dispose();
    }

    void OnDestroy() => FinishGame();
}
