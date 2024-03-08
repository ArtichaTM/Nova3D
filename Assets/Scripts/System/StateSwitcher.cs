using System;
using System.Collections;
using R3;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

public enum State {
    Game = 0, Upgrades, CameraAnimation,
    InGameMenu, MainMenu,
    Unlocks, Learn, Glossary, Changelog, Settings, Scores, Credits, Start
}

public class StateSwitcher : MonoBehaviour
{
    public static StateSwitcher instance;

    public ReactiveProperty<State> state {
        get; private set;
    } = new(State.Start);
    public UI_Controller lastAnimation {
        get; private set;
    }

    UI_Controller controllerMainMenu;
    UI_Controller controllerSettings;
    UI_Controller controllerInGameMenu;

    public bool Animating {
        get => lastAnimation.IsAnimating.Value;
    }

    void Start()
    {
        instance = this;
        controllerMainMenu = GameObject.Find("UI_MainMenu").GetComponent<UI_Controller>();
        Assert.IsNotNull(controllerMainMenu);
        controllerSettings = GameObject.Find("UI_Settings").GetComponent<UI_Controller>();
        Assert.IsNotNull(controllerSettings);
        controllerInGameMenu = GameObject.Find("UI_InGameMenu").GetComponent<UI_Controller>();
        Assert.IsNotNull(controllerInGameMenu);
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
        controllerMainMenu.ui.Q<Button>("Start"    ).clicked += () => SwitchState(State.CameraAnimation);
        // controllerMainMenu.ui.Q<Button>("Unlocks"  ).clicked += () => SwitchState(State.Unlocks        );
        // controllerMainMenu.ui.Q<Button>("Learn"    ).clicked += () => SwitchState(State.Learn          );
        // controllerMainMenu.ui.Q<Button>("Glossary" ).clicked += () => SwitchState(State.Glossary       );
        // controllerMainMenu.ui.Q<Button>("Changelog").clicked += () => SwitchState(State.Changelog      );
        controllerMainMenu.ui.Q<Button>("Settings" ).clicked += () => SwitchState(State.Settings       );
        // controllerMainMenu.ui.Q<Button>("Scores"   ).clicked += () => SwitchState(State.Scores         );
        // controllerMainMenu.ui.Q<Button>("Credits"  ).clicked += () => SwitchState(State.Credits        );
        controllerMainMenu.ui.Q<Button>("Exit"     ).clicked += () => MainLogic.instance.Quit();
        #endregion

        #region SettingsUI
        controllerSettings.ui.Q<Slider>("transitionSpeed").value = Settings.transitionsSpeed.Value;
        controllerSettings.ui.Q<Toggle>("mouseInvertVertical").value = Settings.invertedMouseVertical.Value;
        controllerSettings.ui.Q<Toggle>("mouseInvertHorizontal").value = Settings.invertedMouseHorizontal.Value;
        #endregion

        #region InGameMenuUI
        controllerInGameMenu.ui.Q<Button>("Continue").clicked += () => {
            if (MiscellaneousFunctions.instance.IsIntroAnimating.Value)
                SwitchState(State.CameraAnimation);
            else
                SwitchState(State.Game);
        };
        controllerInGameMenu.ui.Q<Button>("MainMenu").clicked += () => SwitchState(State.MainMenu);
        #endregion
    }

    public void SettingsSave() {
        Settings.transitionsSpeed.Value = controllerSettings.ui.Q<Slider>("transitionSpeed").value;
        Settings.invertedMouseVertical.Value = controllerSettings.ui.Q<Toggle>("mouseInvertVertical").value;
        Settings.invertedMouseHorizontal.Value = controllerSettings.ui.Q<Toggle>("mouseInvertHorizontal").value;
    }

    public void SwitchMenu(State to) {
        StartCoroutine(SwitchMenuAsync(to));
    }

    public IEnumerator SwitchMenuAsync(State to) {
        lastAnimation = StateToController(state.Value);
        lastAnimation.FadeOut();
        yield return new WaitUntil(() => !Animating);
        lastAnimation.enabled = false;

        state.Value = to;
        lastAnimation = StateToController(to);
        lastAnimation.enabled = true;
        lastAnimation.FadeIn();
    }

    void TargetFadeIn(UI_Controller to) {
        lastAnimation = to;
        lastAnimation.enabled = true;
        lastAnimation.FadeIn();
    }

    void TargetFadeIn(State to) {
        TargetFadeIn(StateToController(to));
        state.Value = to;
    }

    void TargetFadeOut(State to) {
        TargetFadeOut(StateToController(to));
    }

    void TargetFadeOut(UI_Controller to) {
        lastAnimation = to;
        lastAnimation.FadeOut();
    }

    public void SwitchState(State to) => StartCoroutine(SwitchStateAsync(to));

    public IEnumerator SwitchStateAsync(State to) {
        if (state.Value == to) {
            Debug.Log($"Trying to repeat switch {to}");
            yield break;
        }
        switch (state.Value) {
            case State.Unlocks:
            case State.Learn:
            case State.Glossary:
            case State.Changelog:
            case State.Scores:
            case State.Credits: {
                if (StateToController(state.Value).IsAnimating.Value) yield break;
                switch (to) {
                    case State.MainMenu: {
                        SwitchMenu(to);
                        break;
                    }
                    default:
                        throw new NotSupportedException($"Unsupported switch {state.Value}->{to}");
                }
                break;
            }
            case State.Settings: {
                if (StateToController(state.Value).IsAnimating.Value) yield break;
                switch (to) {
                    case State.MainMenu: {
                        SettingsSave();
                        SwitchMenu(to);
                        break;
                    }
                    default:
                        throw new NotSupportedException($"Unsupported switch {state.Value}->{to}");
                }
                break;
            }
            case State.MainMenu: {
                if (StateToController(state.Value).IsAnimating.Value) yield break;
                switch (to) {
                    case State.CameraAnimation: {
                        TargetFadeOut(state.Value);
                        MainLogic.instance.Finished.Value = false;
                        state.Value = to;
                        break;
                    }
                    case State.Settings: {
                        // TODO: Update settings
                        SwitchMenu(to);
                        break;
                    }
                    case State.Unlocks:
                    case State.Learn: 
                    case State.Glossary: 
                    case State.Changelog: 
                    case State.Scores: 
                    case State.Credits: {
                        SwitchMenu(to);
                        break;
                    }
                    default:
                        throw new NotSupportedException($"Unsupported switch {state.Value}->{to}");
                }
                break;
            }
            case State.InGameMenu: {
                if (StateToController(state.Value).IsAnimating.Value) yield break;
                switch (to) {
                    case State.Game: {
                        TargetFadeOut(state.Value);
                        if (!MiscellaneousFunctions.instance.IsIntroAnimating.Value) {
                            MainLogic.instance.Paused.Value = false;
                        }
                        state.Value = to;
                        break;
                    }
                    case State.Upgrades: {
                        SwitchMenu(to);
                        break;
                    }
                    case State.MainMenu: {
                        MainLogic.instance.Finished.Value = true;
                        SwitchMenu(to);
                        break;
                    }
                    case State.CameraAnimation: {
                        TargetFadeOut(state.Value);
                        state.Value = to;
                        break;
                    }
                    default:
                        throw new NotSupportedException($"Unsupported switch {state.Value}->{to}");
                }
                break;
            }
            case State.Game: {
                switch (to) {
                    case State.InGameMenu: {
                        TargetFadeIn(to);
                        MainLogic.instance.Paused.Value = true;
                        break;
                    }
                    case State.Upgrades: {
                        SwitchMenu(to);
                        MainLogic.instance.Paused.Value = true;
                        break;
                    }
                    default:
                        throw new NotSupportedException($"Unsupported switch {state.Value}->{to}");
                }
                break;
            }
            case State.Upgrades: {
                if (StateToController(state.Value).IsAnimating.Value) yield break;
                switch (to) {
                    case State.Game: {
                        TargetFadeOut(state.Value);
                        MainLogic.instance.Paused.Value = false;
                        state.Value = to;
                        break;
                    }
                    default:
                        throw new NotSupportedException($"Unsupported switch {state.Value}->{to}");
                }
                break;
            }
            case State.Start: {
                switch (to) {
                    case State.MainMenu: {
                        TargetFadeIn(to);
                        break;
                    }
                    default:
                        throw new NotSupportedException($"Unsupported switch {state.Value}->{to}");
                }
                break;
            }
            case State.CameraAnimation: {
                switch (to) {
                    case State.Game: {
                        state.Value = to;
                        break;
                    }
                    case State.InGameMenu: {
                        TargetFadeIn(to);
                        if (!MiscellaneousFunctions.instance.IsIntroAnimating.Value) {
                            MainLogic.instance.Paused.Value = false;
                        }
                        break;
                    }
                    default:
                        throw new NotSupportedException($"Unsupported switch {state.Value}->{to}");
                }
                break;
            }
            default:
                throw new NotSupportedException($"Unsupported State {state.Value}");
        }
        Updates.SwitchUpdate(to);
    }
}
