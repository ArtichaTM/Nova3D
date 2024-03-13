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

[RequireComponent(typeof(MainLogic))]
public class StateSwitcher : MonoBehaviour
{
    public static StateSwitcher Instance;

    public ReactiveProperty<State> State {
        get; private set;
    } = new(global::State.Start);
    public UI_Controller LastAnimation {
        get; private set;
    }

    UI_Controller controllerMainMenu;
    UI_Controller controllerSettings;
    UI_Controller controllerInGameMenu;

    public bool Animating {
        get => LastAnimation.IsAnimating.Value;
    }

    void Start()
    {
        #region Assertions
        Assert.IsNotNull(GameObject.Find("UI/Menus/UI_MainMenu"));
        Assert.IsNotNull(GameObject.Find("UI/Menus/UI_Settings"));
        Assert.IsNotNull(GameObject.Find("UI/Menus/UI_InGameMenu"));
        Assert.IsNotNull(GameObject.Find("UI/Menus/UI_MainMenu").GetComponent<UI_Controller>());
        Assert.IsNotNull(GameObject.Find("UI/Menus/UI_Settings").GetComponent<UI_Controller>());
        Assert.IsNotNull(GameObject.Find("UI/Menus/UI_InGameMenu").GetComponent<UI_Controller>());
        #endregion

        Instance = this;
        controllerMainMenu = GameObject.Find("UI/Menus/UI_MainMenu").GetComponent<UI_Controller>();
        controllerSettings = GameObject.Find("UI/Menus/UI_Settings").GetComponent<UI_Controller>();
        controllerInGameMenu = GameObject.Find("UI/Menus/UI_InGameMenu").GetComponent<UI_Controller>();
    }

    private UI_Controller StateToController(State state) => state switch {
        global::State.MainMenu => controllerMainMenu,
        global::State.Settings => controllerSettings,
        global::State.InGameMenu => controllerInGameMenu,
        _ => throw new ArgumentException("There's no UI_Controller for " + state),
    };

    public void PostInit()
    {
        #region MenuUI
        controllerMainMenu.ui.Q<Button>("Start"    ).clicked += () => SwitchState(global::State.CameraAnimation);
        // controllerMainMenu.ui.Q<Button>("Unlocks"  ).clicked += () => SwitchState(State.Unlocks        );
        // controllerMainMenu.ui.Q<Button>("Learn"    ).clicked += () => SwitchState(State.Learn          );
        // controllerMainMenu.ui.Q<Button>("Glossary" ).clicked += () => SwitchState(State.Glossary       );
        // controllerMainMenu.ui.Q<Button>("Changelog").clicked += () => SwitchState(State.Changelog      );
        controllerMainMenu.ui.Q<Button>("Settings" ).clicked += () => SwitchState(global::State.Settings       );
        // controllerMainMenu.ui.Q<Button>("Scores"   ).clicked += () => SwitchState(State.Scores         );
        // controllerMainMenu.ui.Q<Button>("Credits"  ).clicked += () => SwitchState(State.Credits        );
        controllerMainMenu.ui.Q<Button>("Exit"     ).clicked += () => MainLogic.Instance.Quit();
        #endregion

        #region SettingsUI
        controllerSettings.ui.Q<Slider>("transitionSpeed").value = Settings.transitionsSpeed.Value;
        controllerSettings.ui.Q<Toggle>("mouseInvertVertical").value = Settings.invertedMouseVertical.Value;
        controllerSettings.ui.Q<Toggle>("mouseInvertHorizontal").value = Settings.invertedMouseHorizontal.Value;
        #endregion

        #region InGameMenuUI
        controllerInGameMenu.ui.Q<Button>("Continue").clicked += () => {
            if (MiscellaneousFunctions.Instance.IsIntroAnimating.Value)
                SwitchState(global::State.CameraAnimation);
            else
                SwitchState(global::State.Game);
        };
        controllerInGameMenu.ui.Q<Button>("MainMenu").clicked += () => SwitchState(global::State.MainMenu);
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
        LastAnimation = StateToController(State.Value);
        LastAnimation.FadeOut();
        yield return new WaitUntil(() => !Animating);
        LastAnimation.enabled = false;

        State.Value = to;
        LastAnimation = StateToController(to);
        LastAnimation.enabled = true;
        LastAnimation.FadeIn();
    }

    void TargetFadeIn(UI_Controller to) {
        LastAnimation = to;
        LastAnimation.enabled = true;
        LastAnimation.FadeIn();
    }

    void TargetFadeIn(State to) {
        TargetFadeIn(StateToController(to));
        State.Value = to;
    }

    void TargetFadeOut(State to) {
        TargetFadeOut(StateToController(to));
    }

    void TargetFadeOut(UI_Controller to) {
        LastAnimation = to;
        LastAnimation.FadeOut();
    }

    public void SwitchState(State to) => StartCoroutine(SwitchStateAsync(to));

    public IEnumerator SwitchStateAsync(State to) {
        if (State.Value == to) {
            Debug.Log($"Trying to repeat switch {to}");
            yield break;
        }
        switch (State.Value) {
            case global::State.Unlocks:
            case global::State.Learn:
            case global::State.Glossary:
            case global::State.Changelog:
            case global::State.Scores:
            case global::State.Credits: {
                if (StateToController(State.Value).IsAnimating.Value) yield break;
                switch (to) {
                    case global::State.MainMenu: {
                                SwitchMenu(to);
                        break;
                    }
                    default:
                        throw new NotSupportedException($"Unsupported switch {State.Value}->{to}");
                }
                break;
            }
            case global::State.Settings: {
                if (StateToController(State.Value).IsAnimating.Value) yield break;
                switch (to) {
                    case global::State.MainMenu: {
                                SettingsSave();
                                SwitchMenu(to);
                        break;
                    }
                    default:
                        throw new NotSupportedException($"Unsupported switch {State.Value}->{to}");
                }
                break;
            }
            case global::State.MainMenu: {
                if (StateToController(State.Value).IsAnimating.Value) yield break;
                switch (to) {
                    case global::State.CameraAnimation: {
                                TargetFadeOut(State.Value);
                                MainLogic.Instance.Finished.Value = false;
                                State.Value = to;
                        break;
                    }
                    case global::State.Settings: {
                                // TODO: Update settings
                                SwitchMenu(to);
                        break;
                    }
                    case global::State.Unlocks:
                    case global::State.Learn: 
                    case global::State.Glossary: 
                    case global::State.Changelog: 
                    case global::State.Scores: 
                    case global::State.Credits: {
                                SwitchMenu(to);
                        break;
                    }
                    default:
                        throw new NotSupportedException($"Unsupported switch {State.Value}->{to}");
                }
                break;
            }
            case global::State.InGameMenu: {
                if (StateToController(State.Value).IsAnimating.Value) yield break;
                switch (to) {
                    case global::State.Game: {
                                TargetFadeOut(State.Value);
                        if (!MiscellaneousFunctions.Instance.IsIntroAnimating.Value) {
                                    MainLogic.Instance.Paused.Value = false;
                        }
                                State.Value = to;
                        break;
                    }
                    case global::State.Upgrades: {
                                SwitchMenu(to);
                        break;
                    }
                    case global::State.MainMenu: {
                                MainLogic.Instance.Finished.Value = true;
                                SwitchMenu(to);
                        break;
                    }
                    case global::State.CameraAnimation: {
                                TargetFadeOut(State.Value);
                                State.Value = to;
                        break;
                    }
                    default:
                        throw new NotSupportedException($"Unsupported switch {State.Value}->{to}");
                }
                break;
            }
            case global::State.Game: {
                switch (to) {
                    case global::State.InGameMenu: {
                                TargetFadeIn(to);
                                MainLogic.Instance.Paused.Value = true;
                        break;
                    }
                    case global::State.Upgrades: {
                                SwitchMenu(to);
                                MainLogic.Instance.Paused.Value = true;
                        break;
                    }
                    default:
                        throw new NotSupportedException($"Unsupported switch {State.Value}->{to}");
                }
                break;
            }
            case global::State.Upgrades: {
                if (StateToController(State.Value).IsAnimating.Value) yield break;
                switch (to) {
                    case global::State.Game: {
                                TargetFadeOut(State.Value);
                                MainLogic.Instance.Paused.Value = false;
                                State.Value = to;
                        break;
                    }
                    default:
                        throw new NotSupportedException($"Unsupported switch {State.Value}->{to}");
                }
                break;
            }
            case global::State.Start: {
                switch (to) {
                    case global::State.MainMenu: {
                                TargetFadeIn(to);
                        break;
                    }
                    default:
                        throw new NotSupportedException($"Unsupported switch {State.Value}->{to}");
                }
                break;
            }
            case global::State.CameraAnimation: {
                switch (to) {
                    case global::State.Game: {
                                MainLogic.Instance.Paused.Value = false;
                                State.Value = to;
                        break;
                    }
                    case global::State.InGameMenu: {
                                TargetFadeIn(to);
                        if (!MiscellaneousFunctions.Instance.IsIntroAnimating.Value) {
                                    MainLogic.Instance.Paused.Value = false;
                        }
                        break;
                    }
                    default:
                        throw new NotSupportedException($"Unsupported switch {State.Value}->{to}");
                }
                break;
            }
            default:
                throw new NotSupportedException($"Unsupported State {State.Value}");
        }
        Updates.SwitchUpdate(to);
    }
}
