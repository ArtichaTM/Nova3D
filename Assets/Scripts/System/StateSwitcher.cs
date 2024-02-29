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
        get => lastAnimation.animating;
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
        };
        controllerMainMenu.ui.Q<Button>("Learn").clicked += () => {
        };
        controllerMainMenu.ui.Q<Button>("Glossary").clicked += () => {
        };
        controllerMainMenu.ui.Q<Button>("Changelog").clicked += () => {
        };
        controllerMainMenu.ui.Q<Button>("Settings").clicked += () => {
            StartCoroutine(SwitchState(State.Settings));
        };
        controllerMainMenu.ui.Q<Button>("Scores").clicked += () => {
        };
        controllerMainMenu.ui.Q<Button>("Credits").clicked += () => {
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

    public bool AllUIInitialized() {
        if (controllerMainMenu.initialized == false) {
            return false;
        }
        return true;
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

    public IEnumerator SwitchState(State to) {
        switch (state) {
            case State.Unlocks:{
                throw new NotImplementedException(); // TODO
            }
            case State.Learn: {
                throw new NotImplementedException(); // TODO
            } 
            case State.Glossary: {
                throw new NotImplementedException(); // TODO
            }
            case State.Changelog: {
                throw new NotImplementedException(); // TODO
            }
            case State.Settings: {
                switch (to) {
                    case State.MainMenu: {
                        SettingsSave();
                        StartCoroutine(SwitchMenu(State.MainMenu));
                        yield break;
                    }
                    default:
                        throw new NotSupportedException("Can't switch from from MainMenu submenu to any state besides MainMenu");
                }
            }
            case State.Scores: {
                yield break;
            }
            case State.Credits: {
                yield break;
            }
            case State.MainMenu: {
                switch (to) {
                    case State.Game: {
                        StateToController(State.MainMenu).FadeOut();
                        state = State.Game;
                        mainLogic.StartCoroutine(mainLogic.StartGame());
                        yield break;
                    }
                    case State.Unlocks:
                        throw new NotImplementedException(); // TODO
                    case State.Learn: 
                        throw new NotImplementedException(); // TODO
                    case State.Glossary: 
                        throw new NotImplementedException(); // TODO
                    case State.Changelog: 
                        throw new NotImplementedException(); // TODO
                    case State.Settings: {
                        StartCoroutine(SwitchMenu(State.Settings));
                        yield break;
                    }
                    case State.Scores: 
                        throw new NotImplementedException(); // TODO
                    case State.Credits: 
                        throw new NotImplementedException(); // TODO
                }
                break;
            }
            case State.InGameMenu: {
                switch (to) {
                    case State.Game: {
                        StateToController(State.InGameMenu).FadeOut();
                        state = State.Game;
                        mainLogic.StartCoroutine(mainLogic.ResumeGame());
                        yield break;
                    }
                    case State.MainMenu: {
                        StartCoroutine(SwitchMenu(State.MainMenu));
                        mainLogic.StartCoroutine(mainLogic.FinishGame());
                        yield break;
                    }
                    case State.Upgrades: {
                        StartCoroutine(SwitchMenu(State.Upgrades));
                        yield break;
                    }
                }
                break;
            }
            case State.Game: {
                switch (to) {
                    case State.InGameMenu: {
                        StateToController(State.InGameMenu).FadeIn();
                        state = State.InGameMenu;
                        yield break;
                    }
                    case State.Upgrades:
                        throw new NotImplementedException(); // TODO
                }
                break;
            }
            case State.Upgrades: {
                switch (to) {
                    case State.Game:
                        throw new NotImplementedException(); // TODO
                }
                break;
            }
            case State.Start: {
                Assert.AreEqual(State.MainMenu, to);
                controllerMainMenu.FadeIn();
                lastAnimation = controllerMainMenu;
                state = State.MainMenu;
                // yield return new WaitUntil(() => !animating);
                break;
            }
        }
    }
}
