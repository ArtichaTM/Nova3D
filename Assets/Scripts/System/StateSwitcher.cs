using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

public enum State {
    Game = 0, Upgrades, InGameMenu, MainMenu,
    Unlocks, Learn, Glossary, Changelog, Settings, Scores, Credits, Start
}

public class StateSwitcher : MonoBehaviour
{
    public State state {
        get; private set;
    } = State.Start;
    public UI_Controller lastAnimation {
        get; private set;
    }
    MainLogic mainLogic;

    UI_Controller controllerMainMenu;
    UI_Controller controllerSettings;
    UI_Controller controllerInGameMenu;

    public bool animating {
        get => lastAnimation.IsAnimating;
    }

    void Start()
    {
        controllerMainMenu = GameObject.Find("UI_MainMenu").GetComponent<UI_Controller>();
        Assert.IsNotNull(controllerMainMenu);
        controllerSettings = GameObject.Find("UI_Settings").GetComponent<UI_Controller>();
        Assert.IsNotNull(controllerSettings);
        controllerInGameMenu = GameObject.Find("UI_InGameMenu").GetComponent<UI_Controller>();
        Assert.IsNotNull(controllerInGameMenu);
        mainLogic = GameObject.Find("SystemScripts").GetComponent<MainLogic>();
        Assert.IsNotNull(mainLogic);
    }

    private UI_Controller StateToController(State state) => state switch {
        State.MainMenu => controllerMainMenu,
        State.Settings => controllerSettings,
        State.InGameMenu => controllerInGameMenu,
        _ => throw new ArgumentException("There's no UI_Controller for " + state),
    };

    public void PostInit()
    {
        // Main Menu
        controllerMainMenu.ui.Q<Button>("Start").clicked += () => {
            StartCoroutine(SwitchState(State.Game));
        };
        controllerMainMenu.ui.Q<Button>("Unlocks").clicked += () => {
            StartCoroutine(SwitchState(State.Unlocks));
        };
        controllerMainMenu.ui.Q<Button>("Learn").clicked += () => {
            StartCoroutine(SwitchState(State.Learn));
        };
        controllerMainMenu.ui.Q<Button>("Glossary").clicked += () => {
            StartCoroutine(SwitchState(State.Glossary));
        };
        controllerMainMenu.ui.Q<Button>("Changelog").clicked += () => {
            StartCoroutine(SwitchState(State.Changelog));
        };
        controllerMainMenu.ui.Q<Button>("Settings").clicked += () => {
            StartCoroutine(SwitchState(State.Settings));
        };
        controllerMainMenu.ui.Q<Button>("Scores").clicked += () => {
            StartCoroutine(SwitchState(State.Scores));
        };
        controllerMainMenu.ui.Q<Button>("Credits").clicked += () => {
            StartCoroutine(SwitchState(State.Credits));
        };
        controllerMainMenu.ui.Q<Button>("Exit").clicked += () => {
            Application.Quit();
        };

        // Settings
        controllerSettings.ui.Q<Slider>("transitionSpeed").value = Settings.transitionsSpeed;

        // InGameMenu
        controllerInGameMenu.ui.Q<Button>("Continue").clicked += () => {
            StartCoroutine(SwitchState(State.Game));
        };
        controllerInGameMenu.ui.Q<Button>("MainMenu").clicked += () => {
            StartCoroutine(SwitchState(State.MainMenu));
        };
    }

    public void SettingsSave() {
        Settings.transitionsSpeed = controllerSettings.ui.Q<Slider>("transitionSpeed").value;
    }

    IEnumerator SwitchMenu(State to) {
        lastAnimation = StateToController(state);
        lastAnimation.FadeOut();
        yield return new WaitUntil(() => !animating);
        lastAnimation.enabled = false;

        state = to;
        lastAnimation = StateToController(to);
        lastAnimation.enabled = true;
        lastAnimation.FadeIn();
        // yield return new WaitUntil(() => !animating);
    }

    private void TargetFadeIn(State to) {
        lastAnimation = StateToController(to);
        lastAnimation.enabled = true;
        StateToController(to).FadeIn();
        state = to;
    }

    private void TargetFadeOut(State to) {
        lastAnimation = StateToController(to);
        StateToController(to).FadeOut();
    }

    public IEnumerator SwitchState(State to) {
        switch (state) {
            case State.Unlocks:
            case State.Learn:
            case State.Glossary:
            case State.Changelog:
            case State.Scores:
            case State.Credits: {
                switch (to) {
                    case State.MainMenu: {
                        StartCoroutine(SwitchMenu(State.MainMenu));
                        yield break;
                    }
                    default:
                        throw new NotSupportedException("Can't switch from from " + state + " submenu to any state besides MainMenu");
                }
            }
            case State.Settings: {
                switch (to) {
                    case State.MainMenu: {
                        SettingsSave();
                        StartCoroutine(SwitchMenu(State.MainMenu));
                        yield break;
                    }
                    default:
                        throw new NotSupportedException();
                }
            }
            case State.MainMenu: {
                switch (to) {
                    case State.Game: {
                        // StateToController(State.MainMenu).FadeOut();
                        // state = State.Game;
                        TargetFadeOut(State.MainMenu);
                        state = to;
                        mainLogic.StartCoroutine(mainLogic.StartGame());
                        yield break;
                    }
                    case State.Settings: {
                        // TODO: Update settings
                        StartCoroutine(SwitchMenu(to));
                        yield break;
                    }
                    case State.Unlocks:
                    case State.Learn: 
                    case State.Glossary: 
                    case State.Changelog: 
                    case State.Scores: 
                    case State.Credits: {
                        StartCoroutine(SwitchMenu(to));
                        yield break;
                    }
                    default:
                        throw new NotSupportedException();
                }
            }
            case State.InGameMenu: {
                UnityEngine.Cursor.visible = false;
                switch (to) {
                    case State.Game: {
                        // lastAnimation = StateToController(State.InGameMenu);
                        // StateToController(State.InGameMenu).FadeOut();
                        // state = State.Game;
                        TargetFadeOut(State.InGameMenu);
                        state = to;
                        mainLogic.StartCoroutine(mainLogic.ResumeGame());
                        yield break;
                    }
                    case State.Upgrades: {
                        StartCoroutine(SwitchMenu(State.Upgrades));
                        yield break;
                    }
                    case State.MainMenu: {
                        StartCoroutine(SwitchMenu(State.MainMenu));
                        mainLogic.StartCoroutine(mainLogic.FinishGame());
                        yield break;
                    }
                    default:
                        throw new NotSupportedException();
                }
            }
            case State.Game: {
                switch (to) {
                    case State.InGameMenu: {
                        // lastAnimation = StateToController(State.InGameMenu);
                        // lastAnimation.enabled = true;
                        // lastAnimation.FadeIn();
                        TargetFadeIn(State.InGameMenu);
                        StartCoroutine(mainLogic.PauseGame());
                        // state = State.InGameMenu;
                        yield break;
                    }
                    case State.Upgrades: {
                        StartCoroutine(SwitchMenu(State.Upgrades));
                        StartCoroutine(mainLogic.PauseGame());
                        yield break;
                    }
                    default:
                        throw new NotSupportedException();
                }
            }
            case State.Upgrades: {
                switch (to) {
                    case State.Game: {
                        // lastAnimation = StateToController(State.Upgrades);
                        // lastAnimation.FadeOut();
                        // yield return new WaitUntil(() => !animating);
                        // lastAnimation.enabled = true;
                        TargetFadeOut(State.Upgrades);
                        state = to;
                        StartCoroutine(mainLogic.ResumeGame());
                        yield break;
                    }
                    default:
                        throw new NotSupportedException();
                }
            }
            case State.Start: {
                switch (to) {
                    case State.MainMenu: {
                        // lastAnimation = StateToController(State.MainMenu);
                        // lastAnimation.enabled = true;
                        // lastAnimation.FadeIn();
                        // state = State.MainMenu;
                        TargetFadeIn(State.MainMenu);
                        yield break;
                    }
                    default:
                        throw new NotSupportedException();
                }
            }
            default:
                throw new NotSupportedException();
        }
    }
}
