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

    public ReactiveProperty<State> CurrentState {
        get; private set;
    } = new(State.Start);
    public UI_Controller LastAnimation {
        get; private set;
    }

    UI_Controller controllerMainMenu;
    UI_Controller controllerSettings;
    UI_Controller controllerInGameMenu;

    public bool Animating {
        get => LastAnimation.IsAnimating.Value;
    }

    DisposableBag SettingsObservables;

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
        State.MainMenu => controllerMainMenu,
        State.Settings => controllerSettings,
        State.InGameMenu => controllerInGameMenu,
        _ => throw new ArgumentException("There's no UI_Controller for " + state),
    };

    public void PostInit()
    {
        UI_Init();
        #region InGameMenuUI
        controllerInGameMenu.ui.Q<Button>("Continue").clicked += () => {
            if (MiscellaneousFunctions.Instance.IsIntroAnimating.Value)
                SwitchState(State.CameraAnimation);
            else
                SwitchState(State.Game);
        };
        controllerInGameMenu.ui.Q<Button>("MainMenu").clicked += () => SwitchState(State.MainMenu);
        #endregion
    }

    void UI_Init() {
        #region MenuUI
        controllerMainMenu.ui.Q<Button>("Start"    ).clicked += () => SwitchState(State.CameraAnimation);
        // controllerMainMenu.ui.Q<Button>("Unlocks"  ).clicked += () => SwitchState(State.Unlocks        );
        // controllerMainMenu.ui.Q<Button>("Learn"    ).clicked += () => SwitchState(State.Learn          );
        // controllerMainMenu.ui.Q<Button>("Glossary" ).clicked += () => SwitchState(State.Glossary       );
        // controllerMainMenu.ui.Q<Button>("Changelog").clicked += () => SwitchState(State.Changelog      );
        controllerMainMenu.ui.Q<Button>("Settings" ).clicked += () => SwitchState(State.Settings       );
        // controllerMainMenu.ui.Q<Button>("Scores"   ).clicked += () => SwitchState(State.Scores         );
        // controllerMainMenu.ui.Q<Button>("Credits"  ).clicked += () => SwitchState(State.Credits        );
        controllerMainMenu.ui.Q<Button>("Exit"     ).clicked += () => MainLogic.Instance.Quit();
        #endregion

        #region SettingsUI
        controllerSettings.ui.Q<Slider>("transitionSpeed"       ).value = Settings.transitionsSpeed.Value       ;
        controllerSettings.ui.Q<Toggle>("mouseInvertVertical"   ).value = Settings.invertedMouseVertical.Value  ;
        controllerSettings.ui.Q<Toggle>("mouseInvertHorizontal" ).value = Settings.invertedMouseHorizontal.Value;
        controllerSettings.ui.Q<Toggle>("equalizeYawPitch"      ).value = Settings.EqualizeYawPitch.Value       ;
        controllerSettings.ui.Q<Toggle>("preciseProjections"    ).value = Settings.PreciseProjections.Value     ;

        controllerSettings.ui.Q<Slider>("boundaryAppearDistance").value = Settings.BoundaryAppearDistance.Value ;
        controllerSettings.ui.Q<Slider>("boundaryHoleFactor"    ).value = Settings.BoundaryHoleFactor.Value     ;
        controllerSettings.ui.Q<Slider>("boundaryMinimumOpacity").value = Settings.BoundaryMinimumOpacity.Value ;
        controllerSettings.ui.Q<Slider>("boundaryMaximumOpacity").value = Settings.BoundaryMaximumOpacity.Value ;
        #endregion
    }

    void RegisterForSettingsChanges() {
        SettingsObservables = new(9);
        VisualElement el = controllerSettings.ui;
        Observable
            .EveryValueChanged(el.Q<Slider>("transitionSpeed"), x => x.value)
            .Subscribe(value => Settings.transitionsSpeed.Value=value)
            .AddTo(ref SettingsObservables)
            ;
        Observable
            .EveryValueChanged(el.Q<Toggle>("mouseInvertVertical"), x => x.value)
            .Subscribe(value => Settings.invertedMouseVertical.Value=value)
            .AddTo(ref SettingsObservables)
            ;
        Observable
            .EveryValueChanged(el.Q<Toggle>("mouseInvertHorizontal"), x => x.value)
            .Subscribe(value => Settings.invertedMouseHorizontal.Value=value)
            .AddTo(ref SettingsObservables)
            ;
        Observable
            .EveryValueChanged(el.Q<Toggle>("equalizeYawPitch"), x => x.value)
            .Subscribe(value => Settings.EqualizeYawPitch.Value=value)
            .AddTo(ref SettingsObservables)
            ;
        Observable
            .EveryValueChanged(el.Q<Toggle>("preciseProjections"), x => x.value)
            .Subscribe(value => Settings.PreciseProjections.Value=value)
            .AddTo(ref SettingsObservables)
            ;
        Observable
            .EveryValueChanged(el.Q<Slider>("boundaryAppearDistance"), x => x.value)
            .Subscribe(value => Settings.BoundaryAppearDistance.Value=value)
            .AddTo(ref SettingsObservables)
            ;
        Observable
            .EveryValueChanged(el.Q<Slider>("boundaryHoleFactor"), x => x.value)
            .Subscribe(value => Settings.BoundaryHoleFactor.Value=value)
            .AddTo(ref SettingsObservables)
            ;
        Observable
            .EveryValueChanged(el.Q<Slider>("boundaryMinimumOpacity"), x => x.value)
            .Subscribe(value => {
                Settings.BoundaryMinimumOpacity.Value=value;
                el.Q<Slider>("boundaryMaximumOpacity").lowValue = value;
            })
            .AddTo(ref SettingsObservables)
            ;
        Observable
            .EveryValueChanged(el.Q<Slider>("boundaryMaximumOpacity"), x => x.value)
            .Subscribe(value => {
                Settings.BoundaryMaximumOpacity.Value=value;
                el.Q<Slider>("boundaryMinimumOpacity").highValue = value;
            })
            .AddTo(ref SettingsObservables)
            ;
    }

    void UnRegisterForSettingsChanges() {
        SettingsObservables.Dispose();
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
        LastAnimation = StateToController(CurrentState.Value);
        LastAnimation.FadeOut();
        yield return new WaitUntil(() => !Animating);
        LastAnimation.enabled = false;

        CurrentState.Value = to;
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
        CurrentState.Value = to;
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
        if (CurrentState.Value == to) {
            Debug.Log($"Trying to repeat switch {to}");
            yield break;
        }
        switch (CurrentState.Value) {
            case State.Unlocks:
            case State.Learn:
            case State.Glossary:
            case State.Changelog:
            case State.Scores:
            case State.Credits: {
                if (StateToController(CurrentState.Value).IsAnimating.Value) yield break;
                switch (to) {
                    case State.MainMenu: {
                                SwitchMenu(to);
                        break;
                    }
                    default:
                        throw new NotSupportedException($"Unsupported switch {CurrentState.Value}->{to}");
                }
                break;
            }
            case State.Settings: {
                if (StateToController(CurrentState.Value).IsAnimating.Value) yield break;
                UnRegisterForSettingsChanges();
                switch (to) {
                    case State.MainMenu: {
                                SettingsSave();
                                SwitchMenu(to);
                        break;
                    }
                    default:
                        throw new NotSupportedException($"Unsupported switch {CurrentState.Value}->{to}");
                }
                break;
            }
            case State.MainMenu: {
                if (StateToController(CurrentState.Value).IsAnimating.Value) yield break;
                switch (to) {
                    case State.CameraAnimation: {
                                TargetFadeOut(CurrentState.Value);
                                MainLogic.Instance.Finished.Value = false;
                                CurrentState.Value = to;
                        break;
                    }
                    case State.Settings: {
                                // TODO: Update settings
                                RegisterForSettingsChanges();
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
                        throw new NotSupportedException($"Unsupported switch {CurrentState.Value}->{to}");
                }
                break;
            }
            case State.InGameMenu: {
                if (StateToController(CurrentState.Value).IsAnimating.Value) yield break;
                switch (to) {
                    case State.Game: {
                                TargetFadeOut(CurrentState.Value);
                        if (!MiscellaneousFunctions.Instance.IsIntroAnimating.Value) {
                                    MainLogic.Instance.Paused.Value = false;
                        }
                                CurrentState.Value = to;
                        break;
                    }
                    case State.Upgrades: {
                                SwitchMenu(to);
                        break;
                    }
                    case State.MainMenu: {
                                MainLogic.Instance.Finished.Value = true;
                                MiscellaneousFunctions.Instance.AnimationDisposable.Dispose();

                                SwitchMenu(to);
                        break;
                    }
                    case State.CameraAnimation: {
                                TargetFadeOut(CurrentState.Value);
                                CurrentState.Value = to;
                        break;
                    }
                    default:
                        throw new NotSupportedException($"Unsupported switch {CurrentState.Value}->{to}");
                }
                break;
            }
            case State.Game: {
                switch (to) {
                    case State.InGameMenu: {
                                TargetFadeIn(to);
                                MainLogic.Instance.Paused.Value = true;
                        break;
                    }
                    case State.Upgrades: {
                                SwitchMenu(to);
                                MainLogic.Instance.Paused.Value = true;
                        break;
                    }
                    default:
                        throw new NotSupportedException($"Unsupported switch {CurrentState.Value}->{to}");
                }
                break;
            }
            case State.Upgrades: {
                if (StateToController(CurrentState.Value).IsAnimating.Value) yield break;
                switch (to) {
                    case State.Game: {
                                TargetFadeOut(CurrentState.Value);
                                MainLogic.Instance.Paused.Value = false;
                                CurrentState.Value = to;
                        break;
                    }
                    default:
                        throw new NotSupportedException($"Unsupported switch {CurrentState.Value}->{to}");
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
                        throw new NotSupportedException($"Unsupported switch {CurrentState.Value}->{to}");
                }
                break;
            }
            case State.CameraAnimation: {
                switch (to) {
                    case State.Game: {
                                MainLogic.Instance.Paused.Value = false;
                                CurrentState.Value = to;
                        break;
                    }
                    case State.InGameMenu: {
                                TargetFadeIn(to);
                        if (!MiscellaneousFunctions.Instance.IsIntroAnimating.Value) {
                                    MainLogic.Instance.Paused.Value = false;
                        }
                        break;
                    }
                    default:
                        throw new NotSupportedException($"Unsupported switch {CurrentState.Value}->{to}");
                }
                break;
            }
            default:
                throw new NotSupportedException($"Unsupported State {CurrentState.Value}");
        }
        Updates.SwitchUpdate(to);
    }
}
