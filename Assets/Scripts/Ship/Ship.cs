using UnityEngine;
using R3;

public class Ship : MonoBehaviour
{
    readonly CompositeDisposable Disposables = new();
    CompositeDisposable PauseDisposables = new();

    void Start()
    {
        MainLogic.mainLogic.Paused
            .Where(x => x == true)
            .Subscribe(_ => PauseGame())
            .AddTo(Disposables);
        MainLogic.mainLogic.Paused
            .Where(x => x == false)
            .Subscribe(_ => ResumeGame())
            .AddTo(Disposables);
        MainLogic.mainLogic.Finished
            .Where (x => x == true)
            .Subscribe(_ => FinishGame())
            .AddTo(Disposables);
    }

    void PauseGame()
    {
        Debug.Log("Ship pause");
        PauseDisposables.Dispose();
        PauseDisposables = new();
    }

    void ResumeGame()
    {
        Debug.Log("Ship resume");
    }

    void FinishGame() {
        Debug.Log("Ship finish");
    }
}
