using UnityEngine;
using R3;
using UnityEngine.Assertions;
using System;

public class MiscellaneousFunctions : MonoBehaviour
{
    public static MiscellaneousFunctions Instance {get; private set;}

    public ReactiveProperty<bool> IsIntroAnimating {get; private set;} = new(false);
    public SerialDisposable AnimationDelayDisposable {get; private set;} = new();
    public SerialDisposable AnimationDisposable {get; private set;} = new();

    void Start() { Instance = this; }

    public void IntroAnimation() {
        if (!AnimationDelayDisposable.IsDisposed) AnimationDelayDisposable.Dispose();
        if (!AnimationDisposable.IsDisposed) AnimationDisposable.Dispose();

        GameObject ship = MainLogic.Instance.Ship;
        Transform CameraTarget = ship.transform.GetChild(0);
        Assert.AreEqual(CameraTarget.name, "CameraTarget", "CameraTarget should be first child of ship");
        Transform MainCamera = MainLogic.Instance.MainCamera.transform;
        IsIntroAnimating.Value = true;

        AnimationDelayDisposable = new();
        float speed = 0f;
        AnimationDelayDisposable.Disposable = Observable
            .NextFrame()
            .Delay(TimeSpan.FromSeconds(Settings.CameraAnimationDelay))
            .Subscribe(_ => {
                if (
                    StateSwitcher.Instance.CurrentState.Value != State.CameraAnimation
                    &&
                    StateSwitcher.Instance.CurrentState.Value != State.InGameMenu
                ) return;
                AnimationDisposable = new();
                AnimationDisposable.Disposable = Observable
                    .EveryUpdate()
                    .TakeWhile(_ => 
                        Vector3.Distance(CameraTarget.position, MainCamera.position)
                        >
                        Settings.CameraAnimationDistanceMinimum
                    )
                    .Subscribe(_ => {
                        MainCamera.Translate(
                            Settings.GameStartCameraArriveSpeed.Value
                            *speed
                            *Time.deltaTime
                            *(CameraTarget.position - MainCamera.position)
                        );
                        speed += Time.deltaTime;
                    }, _ => AnimationDisposable.Dispose(), _ => {
                        // Based on current state, pausing or continue-ing game
                        switch (StateSwitcher.Instance.CurrentState.Value) {
                            case State.CameraAnimation: {
                                StateSwitcher.Instance.SwitchState(State.Game);
                                break;
                            }
                            case State.Upgrades:
                            case State.InGameMenu: {
                                MainLogic.Instance.Paused.Value = false;
                                MainLogic.Instance.Paused.Value = true;
                                break;
                            }
                            default:
                                throw new NotSupportedException($"How {StateSwitcher.Instance.CurrentState.Value}?");
                        }
                        MainCamera.parent = CameraTarget;
                        IsIntroAnimating.Value = false;
                        AnimationDisposable.Dispose();
                    })
                    ;
                AnimationDelayDisposable.Dispose();
            })
        ;
    }
}
