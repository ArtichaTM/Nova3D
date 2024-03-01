using System.Collections;
using R3;
using UnityEngine;
using UnityEngine.Assertions;

public class MainLogic : MonoBehaviour
{
    public ReactiveProperty<bool> Paused = new(true);
    public ReactiveProperty<bool> Finished = new(true);
    StateSwitcher stateSwitcher;
    Ship ShipScript;
    Camera mainCamera;

    CompositeDisposable GameDisposable = new();

    void Start()
    {
        stateSwitcher = GetComponent<StateSwitcher>();
        ShipScript = GameObject.Find("Ship").GetComponent<Ship>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        foreach (Transform child in GameObject.Find("UI").transform) {
            child.gameObject.SetActive(true);
        }
        ObservableSystem.DefaultTimeProvider = UnityTimeProvider.Update;
        ObservableSystem.DefaultFrameProvider = UnityFrameProvider.Update;
        Invoke(nameof(OnRuntimeLoad), 0);

        Paused
            .Skip(1)
            .Where((bool value) => value==false)
            .Subscribe(_ => ResumeGame())
            .AddTo(GameDisposable);
        Paused
            .Skip(1)
            .Where((bool value) => value==true)
            .Subscribe(_ => PauseGame())
            .AddTo(GameDisposable);
        Finished
            .Where((bool value) => value==false)
            .Subscribe(_ => StartGame())
            .AddTo(GameDisposable);
        Finished
            .Skip(1)
            .Where((bool value) => value==true)
            .Subscribe(_ => FinishGame())
            .AddTo(GameDisposable);
    }

    void OnRuntimeLoad() {
        stateSwitcher.PostInit();
        stateSwitcher.StartCoroutine(stateSwitcher.SwitchState(State.MainMenu));
    }

    public void StartGame() {
        Assert.IsTrue(Paused.Value);
        ShipScript.StartGame();
        Paused.Value = false;
        ResumeGame();
    }

    public void PauseGame() {
        Assert.IsFalse(Finished.Value);
        ShipScript.PauseGame();
        Time.timeScale = 0f;
    }

    public void ResumeGame() {
        Assert.IsFalse(Finished.Value);
        ShipScript.ResumeGame();
        Time.timeScale = 1f;
    }

    public void FinishGame() {
        Assert.IsTrue(Paused.Value);
        if (!Paused.Value)
            Paused.Value = true;
        ShipScript.FinishGame();
    }

    void OnDestroy() => Quit();

    void Update()
    {
        switch (stateSwitcher.state.Value)
        {
            case State.MainMenu: {
                return;
            }
            case State.Game: {
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    stateSwitcher.StartCoroutine(stateSwitcher.SwitchState(State.InGameMenu));
                    return;
                }
                else if (Input.GetKeyDown(KeyCode.Space)) {
                    stateSwitcher.StartCoroutine(stateSwitcher.SwitchState(State.Upgrades));
                    return;
                }
                return;
            }
            case State.InGameMenu: {
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    stateSwitcher.StartCoroutine(stateSwitcher.SwitchState(State.Game));
                    return;
                }
                return;
            }
            case State.Unlocks:
            case State.Learn:
            case State.Glossary:
            case State.Changelog:
            case State.Settings:
            case State.Scores:
            case State.Credits: {
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    StartCoroutine(stateSwitcher.SwitchState(State.MainMenu));
                    return;
                }
                return;
            }
        }
    }

    public void Quit() {
        Debug.Log("Quit!");
        GameDisposable.Dispose();
        Application.Quit();
    }
}
