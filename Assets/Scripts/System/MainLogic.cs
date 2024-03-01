using System.Collections;
using R3;
using UnityEngine;
using UnityEngine.Assertions;

public class MainLogic : MonoBehaviour
{
    bool Paused = true;
    bool Finished = true;
    StateSwitcher stateSwitcher;
    Ship ShipScript;
    Camera mainCamera;

    void Start()
    {
        stateSwitcher = GetComponent<StateSwitcher>();
        ShipScript = GameObject.Find("Ship").GetComponent<Ship>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        foreach (Transform child in GameObject.Find("UI").transform) {
            child.gameObject.SetActive(true);
        }
        Invoke(nameof(OnRuntimeLoad), 0);
    }

    void OnRuntimeLoad() {
        stateSwitcher.PostInit();
        stateSwitcher.StartCoroutine(stateSwitcher.SwitchState(State.MainMenu));
    }

    public IEnumerator StartGame() {
        Assert.IsTrue(Paused);
        Assert.IsTrue(Finished);
        ShipScript.StartGame();
        Finished = false;
        ShipScript.ResumeGame();
        StartCoroutine(ResumeGame());
        yield break;
    }

    public IEnumerator PauseGame() {
        Assert.IsFalse(Paused);
        Assert.IsFalse(Finished);
        ShipScript.PauseGame();
        Time.timeScale = 0f;
        Paused = true;
        yield break;
    }

    public IEnumerator ResumeGame() {
        Assert.IsTrue(Paused);
        Assert.IsFalse(Finished);
        ShipScript.ResumeGame();
        Time.timeScale = 1f;
        Paused = false;
        yield break;
    }

    public IEnumerator FinishGame() {
        Assert.IsTrue(Paused);
        Assert.IsFalse(Finished);
        if (!Paused)
            StartCoroutine(PauseGame());
        ShipScript.FinishGame();
        Finished = true;
        yield break;
    }

    void Update()
    {
        switch (stateSwitcher.state)
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
}
