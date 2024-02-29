using System.Collections;
using UnityEngine;

public class MainLogic : MonoBehaviour
{
    StateSwitcher stateSwitcher;

    void Start()
    {
        this.stateSwitcher = this.GetComponent<StateSwitcher>();
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
        Debug.Log("Starting!");
        yield break;
    }

    public IEnumerator PauseGame() {
        yield break;
    }

    public IEnumerator ResumeGame() {
        yield break;
    }

    public IEnumerator FinishGame() {
        yield break;
    }

    void Update()
    {
        switch (stateSwitcher.state) {
            case State.MainMenu: {
                return;
            }
            case State.Game: {
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    stateSwitcher.StartCoroutine(stateSwitcher.SwitchState(State.InGameMenu));
                    return;
                }
                if (Input.GetKeyDown(KeyCode.Space)) {
                    stateSwitcher.StartCoroutine(stateSwitcher.SwitchState(State.Upgrades));
                    return;
                }
                break;
            }
            case State.InGameMenu: {
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    stateSwitcher.StartCoroutine(stateSwitcher.SwitchState(State.Game));
                    return;
                }
                break;
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
