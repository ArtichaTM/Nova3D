using UnityEngine;
using R3;
using UnityEngine.Assertions;
using System;

public class MiscellaneousFunctions : MonoBehaviour
{
    public static MiscellaneousFunctions Instance {get; private set;}

    public ReactiveProperty<bool> IsIntroAnimating {get; private set;} = new(false);

    void Start() { Instance = this; }

    public void IntroAnimation() {
        GameObject ship = MainLogic.Instance.Ship;
        Transform CameraTarget = ship.transform.GetChild(0);
        Assert.AreEqual(CameraTarget.name, "CameraTarget");
        Transform MainCamera = MainLogic.Instance.MainCamera.transform;
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
                        switch (StateSwitcher.Instance.State.Value) {
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
                                throw new NotSupportedException($"How {StateSwitcher.Instance.State.Value}?");
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
