using UnityEngine;
using R3;

public class Ship : MonoBehaviour
{
    #region Components
    Rigidbody ribi;
    ShipParameters parameters;
    #endregion

    #region Disposables
    CompositeDisposable Disposables;
    CompositeDisposable PauseDisposables = new();
    #endregion

    void Start()
    {
        Disposables = new();
        ribi = GetComponent<Rigidbody>();
        parameters = GetComponent<ShipParameters>();
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
    }
    void ObservableFixedUpdate() {
        parameters.ParticleThrusters.ForEach(controller => controller.Paused.Value = Input.GetKey(KeyCode.W));
        if 
            (Input.GetKey(KeyCode.W))
            ribi.AddRelativeForce(0f, 0f, parameters.SpeedForward.Value);
        else if
            (Input.GetKey(KeyCode.S))
            ribi.AddRelativeForce(0f, 0f, -parameters.SpeedBackwards.Value);
        if
            (Input.GetKey(KeyCode.A))
            ribi.AddRelativeTorque(0f, 0f, parameters.SpeedRotationRoll.Value);
        else if
            (Input.GetKey(KeyCode.D))
            ribi.AddRelativeTorque(0f, 0f, -parameters.SpeedRotationRoll.Value);
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
