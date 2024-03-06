using UnityEngine;
using R3;

public class Ship : MonoBehaviour
{
    readonly CompositeDisposable Disposables = new();
    CompositeDisposable PauseDisposables = new();

    void Start()
    {
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
    }

    void PauseGame()
    {
        PauseDisposables.Dispose();
        PauseDisposables = new();
    }

    void ResumeGame()
    {
    }

    void FinishGame() {
    }
}
