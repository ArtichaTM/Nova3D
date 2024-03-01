using System;
using System.Collections;
using R3;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

public enum State {
    Game = 0, Upgrades, InGameMenu, MainMenu,
    Unlocks, Learn, Glossary, Changelog, Settings, Scores, Credits, Start
}

public class StateSwitcher : MonoBehaviour
{
    public ReactiveProperty<State> state {
        get; private set;
    } = new(State.Start);
    public UI_Controller lastAnimation {
        get; private set;
    }
    MainLogic mainLogic;

    UI_Controller controllerMainMenu;
    UI_Controller controllerSettings;
    UI_Controller controllerInGameMenu;

    public bool Animating {
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
        #region MenuUI
        controllerMainMenu.ui.Q<Button>("Start"    ).clicked += () => StartCoroutine(SwitchState(State.Game     ));
        controllerMainMenu.ui.Q<Button>("Unlocks"  ).clicked += () => StartCoroutine(SwitchState(State.Unlocks  ));
        controllerMainMenu.ui.Q<Button>("Learn"    ).clicked += () => StartCoroutine(SwitchState(State.Learn    ));
        controllerMainMenu.ui.Q<Button>("Glossary" ).clicked += () => StartCoroutine(SwitchState(State.Glossary ));
        controllerMainMenu.ui.Q<Button>("Changelog").clicked += () => StartCoroutine(SwitchState(State.Changelog));
        controllerMainMenu.ui.Q<Button>("Settings" ).clicked += () => StartCoroutine(SwitchState(State.Settings ));
        controllerMainMenu.ui.Q<Button>("Scores"   ).clicked += () => StartCoroutine(SwitchState(State.Scores   ));
        controllerMainMenu.ui.Q<Button>("Credits"  ).clicked += () => StartCoroutine(SwitchState(State.Credits  ));
        controllerMainMenu.ui.Q<Button>("Exit"     ).clicked += () => mainLogic.Quit();
        #endregion

        // Settings
        #region SettingsUI
        controllerSettings.ui.Q<Slider>("transitionSpeed").value = Settings.transitionsSpeed.Value;
        controllerSettings.ui.Q<Toggle>("mouseInvertVertical").value = Settings.invertedMouseVertical.Value;
        controllerSettings.ui.Q<Toggle>("mouseInvertHorizontal").value = Settings.invertedMouseHorizontal.Value;
        #endregion

        // InGameMenu
        #region InGameMenuUI
        controllerInGameMenu.ui.Q<Button>("Continue").clicked += () => StartCoroutine(SwitchState(State.Game));
        controllerInGameMenu.ui.Q<Button>("MainMenu").clicked += () => StartCoroutine(SwitchState(State.MainMenu));
        #endregion
    }

    public void SettingsSave() {
        Settings.transitionsSpeed.Value = controllerSettings.ui.Q<Slider>("transitionSpeed").value;
        Settings.invertedMouseVertical.Value = controllerSettings.ui.Q<Toggle>("mouseInvertVertical").value;
        Settings.invertedMouseHorizontal.Value = controllerSettings.ui.Q<Toggle>("mouseInvertHorizontal").value;
    }

    IEnumerator SwitchMenu(State to) {
        lastAnimation = StateToController(state.Value);
        lastAnimation.FadeOut();
        yield return new WaitUntil(() => !Animating);
        lastAnimation.enabled = false;

        state.Value = to;
        lastAnimation = StateToController(to);
        lastAnimation.enabled = true;
        lastAnimation.FadeIn();
    }

    private void TargetFadeIn(State to) {
        lastAnimation = StateToController(to);
        lastAnimation.enabled = true;
        StateToController(to).FadeIn();
        state.Value = to;
    }

    private void TargetFadeOut(State to) {
        lastAnimation = StateToController(to);
        StateToController(to).FadeOut();
    }

    public IEnumerator SwitchState(State to) {
        switch (state.Value) {
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
                        TargetFadeOut(State.MainMenu);
                        state.Value = to;
                        mainLogic.Finished.Value = false;
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
                switch (to) {
                    case State.Game: {
                        TargetFadeOut(State.InGameMenu);
                        state.Value = to;
                        mainLogic.Paused.Value = false;
                        yield break;
                    }
                    case State.Upgrades: {
                        StartCoroutine(SwitchMenu(State.Upgrades));
                        yield break;
                    }
                    case State.MainMenu: {
                        StartCoroutine(SwitchMenu(State.MainMenu));
                        mainLogic.Finished.Value = true;
                        yield break;
                    }
                    default:
                        throw new NotSupportedException();
                }
            }
            case State.Game: {
                switch (to) {
                    case State.InGameMenu: {
                        TargetFadeIn(State.InGameMenu);
                        mainLogic.Paused.Value = true;
                        yield break;
                    }
                    case State.Upgrades: {
                        StartCoroutine(SwitchMenu(State.Upgrades));
                        mainLogic.Paused.Value = true;
                        yield break;
                    }
                    default:
                        throw new NotSupportedException();
                }
            }
            case State.Upgrades: {
                switch (to) {
                    case State.Game: {
                        TargetFadeOut(State.Upgrades);
                        state.Value = to;
                        mainLogic.Paused.Value = false;
                        yield break;
                    }
                    default:
                        throw new NotSupportedException();
                }
            }
            case State.Start: {
                switch (to) {
                    case State.MainMenu: {
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
