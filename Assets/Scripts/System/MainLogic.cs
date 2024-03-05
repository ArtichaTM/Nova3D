using UnityEngine;
using R3;

public class MainLogic : MonoBehaviour
{
    public static MainLogic mainLogic {get; private set;}

    public ReactiveProperty<bool> Paused = new(true);
    public ReactiveProperty<bool> Finished = new(true);
    public StateSwitcher stateSwitcher {get; private set;}

    #region InspectorProperties
    [SerializeField]
    GameObject _ShipExample;
    GameObject ShipExample => _ShipExample;

    [SerializeField]
    GameObject _MainCamera;
    public GameObject MainCamera => _MainCamera;
    #endregion


    public GameObject Ship {get; private set;} = null;

    CompositeDisposable GameDisposable = new();

    void Start()
    {
        mainLogic = this;
        stateSwitcher = GetComponent<StateSwitcher>();
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
        stateSwitcher.PostInit();
        stateSwitcher.SwitchState(State.MainMenu);
    }

    public void StartGame() {
        Ship = Instantiate(ShipExample);
        Ship.name = "Ship";
        Ship.SetActive(true);
        // MiscellaneousFunctions.IntroAnimation();
        Ship.GetComponent<Rigidbody>().AddRelativeForce(0f, 0f, 300f);
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
    }

    void OnDestroy() => Quit();

    void Update()
    {
        switch (stateSwitcher.state.Value)
        {
            case State.MainMenu: {
                return;
            }
            case State.CameraAnimation: {
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    stateSwitcher.SwitchState(State.InGameMenu);
                    return;
                }
                return;
            }
            case State.Game: {
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    stateSwitcher.SwitchState(State.InGameMenu);
                    return;
                }
                else if (Input.GetKeyDown(KeyCode.Space)) {
                    stateSwitcher.SwitchState(State.Upgrades);
                    return;
                }
                return;
            }
            case State.InGameMenu: {
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    stateSwitcher.SwitchState(State.Game);
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
                    stateSwitcher.SwitchState(State.MainMenu);
                    return;
                }
                return;
            }
        }
    }

    public void Quit() {
        GameDisposable.Dispose();
        Paused.Value = true;
        Finished.Value = true;
        Application.Quit();
    }
}
