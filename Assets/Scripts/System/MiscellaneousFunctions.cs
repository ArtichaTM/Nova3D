using UnityEngine;
using R3;
using UnityEngine.Assertions;
using System;

public class MiscellaneousFunctions : MonoBehaviour
{
    public static MiscellaneousFunctions instance {get; private set;}

    public ReactiveProperty<bool> IsIntroAnimating {get; private set;} = new(false);

    void Start() { instance = this; }

    public void IntroAnimation() {
        GameObject ship = MainLogic.instance.Ship;
        Transform CameraTarget = ship.transform.GetChild(0);
        Assert.AreEqual(CameraTarget.name, "CameraTarget");
        Transform MainCamera = MainLogic.instance.MainCamera.transform;
        IsIntroAnimating.Value = true;

        SerialDisposable instant_disposable = new();
        float speed = 0f;
        instant_disposable.Disposable = Observable
            .NextFrame()
            .Delay(TimeSpan.FromSeconds(Settings.CameraAnimationDelay))
            .Subscribe(_ => {
                SerialDisposable instant_disposable2 = new();
                instant_disposable2.Disposable = Observable
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
                    }, _ => instant_disposable2.Dispose(), _ => {
                        // Based on current state, pausing or continue-ing game
                        switch (StateSwitcher.instance.state.Value) {
                            case State.CameraAnimation: {
                                MainLogic.instance.Paused.Value = false;
                                StateSwitcher.instance.SwitchState(State.Game);
                                break;
                            }
                            case State.Upgrades:
                            case State.InGameMenu: {
                                MainLogic.instance.Paused.Value = false;
                                MainLogic.instance.Paused.Value = true;
                                break;
                            }
                            default:
                                throw new NotSupportedException($"How {StateSwitcher.instance.state.Value}?");
                        }
                        MainCamera.parent = CameraTarget;
                        IsIntroAnimating.Value = false;
                        instant_disposable2.Dispose();
                    })
                    ;
                instant_disposable.Dispose();
            })
        ;
    }
}
