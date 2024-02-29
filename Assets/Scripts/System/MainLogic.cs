using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class MainLogic : MonoBehaviour
{
    StateSwitcher stateSwitcher;

    void Start()
    {
        stateSwitcher = GetComponent<StateSwitcher>();
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
        // Debug.Log("StartGame()");
        yield break;
    }

    public IEnumerator PauseGame() {
        // Debug.Log("PauseGame()");
        yield break;
    }

    public IEnumerator ResumeGame() {
        // Debug.Log("ResumeGame()");
        yield break;
    }

    public IEnumerator FinishGame() {
        // Debug.Log("FinishGame()");
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
