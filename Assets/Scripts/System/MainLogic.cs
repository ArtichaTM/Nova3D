using System;
using UnityEngine;
using R3;
using UnityEngine.Assertions;


static class Updates {
    static SerialDisposable disposable = new();

    static void Game() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            StateSwitcher.instance.SwitchState(State.InGameMenu);
            return;
        }
        else if (Input.GetKeyDown(KeyCode.Space)) {
            StateSwitcher.instance.SwitchState(State.Upgrades);
            return;
        }
        return;
    }

    static void InGameMenu() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (MiscellaneousFunctions.instance.IsIntroAnimating.Value)
                StateSwitcher.instance.SwitchState(State.CameraAnimation);
            else
                StateSwitcher.instance.SwitchState(State.Game);
            return;
        }
        return;
    }

    static void MainMenuSubmenu() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            StateSwitcher.instance.SwitchState(State.MainMenu);
            return;
        }
    }

    static void CameraAnimation() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            StateSwitcher.instance.SwitchState(State.InGameMenu);
            return;
        }
    }

    static Action StateToUpdate(State state) => state switch {
        State.Game => Game,
        State.InGameMenu => InGameMenu,
        State.CameraAnimation => CameraAnimation,

        State.Unlocks => MainMenuSubmenu,
        State.Learn => MainMenuSubmenu,
        State.Glossary => MainMenuSubmenu,
        State.Changelog => MainMenuSubmenu,
        State.Settings => MainMenuSubmenu,
        State.Scores => MainMenuSubmenu,
        State.Credits => MainMenuSubmenu,
        _ => null
    };

    public static void SwitchUpdate(State to) {
        disposable.Dispose();
        disposable = new();

        Action updateFunction = StateToUpdate(to);
        if (StateToUpdate(to) == null) return;

        disposable.Disposable = Observable
            .EveryUpdate()
            .Subscribe(_ => updateFunction());
    }
}

public class MainLogic : MonoBehaviour
{
    public static MainLogic instance {get; private set;}

    public ReactiveProperty<bool> Paused = new(true);
    public ReactiveProperty<bool> Finished = new(true);

    #region InspectorProperties
    [SerializeField]
    GameObject _ShipExample;
    GameObject ShipExample => _ShipExample;

    [SerializeField]
    GameObject _MainCamera;
    public GameObject MainCamera => _MainCamera;

    [SerializeField]
    GameObject _DefaultCamera;
    public GameObject DefaultCamera => _DefaultCamera;
    #endregion


    public GameObject Ship {get; private set;} = null;

    CompositeDisposable GameDisposable = new();

    void Start()
    {
        Assert.IsNotNull(_ShipExample, "_ShipExample can't be null. Check script in inspector");
        Assert.IsNotNull(_MainCamera, "_MainCamera example can't be null. Check script in inspector");
        Assert.IsNotNull(_DefaultCamera, "_DefaultCamera can't be null. Check script in inspector");
        instance = this;
        foreach (Transform child in GameObject.Find("UI").transform) {
            child.gameObject.SetActive(true);
        }
        ObservableSystem.DefaultTimeProvider = UnityTimeProvider.Update;
        ObservableSystem.DefaultFrameProvider = UnityFrameProvider.Update;
        Invoke(nameof(OnRuntimeLoad), 0);

        Paused
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
            .Skip(1) // Finished default value is true => Subscribe instantly calls
            .Where((bool value) => value==true)
            .Subscribe(_ => FinishGame())
            .AddTo(GameDisposable);
    }

    void OnRuntimeLoad() {
        StateSwitcher.instance.PostInit();
        StateSwitcher.instance.SwitchState(State.MainMenu);
    }

    public void StartGame() {
        Ship = Instantiate(ShipExample);
        Ship.name = "Ship";
        Ship.SetActive(true);
        MiscellaneousFunctions.instance.IntroAnimation();
        Ship.GetComponent<Rigidbody>().AddRelativeForce(0f, 0f, Settings.spawnSpeed);
        Time.timeScale = 1f;
    }

    public void PauseGame() {
        Time.timeScale = 0f;
    }

    public void ResumeGame() {
        Time.timeScale = 1f;
    }

    public void FinishGame() {
        Destroy(Ship);
        Ship = null;
        MainCamera.transform.SetPositionAndRotation(
            DefaultCamera.transform.position,
            DefaultCamera.transform.rotation
        );
    }

    void OnDestroy() => Quit();

    public void Quit() {
        GameDisposable.Dispose();
        Paused.Value = true;
        Finished.Value = true;
        Application.Quit();
    }
}
