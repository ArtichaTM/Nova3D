using UnityEngine;
using R3;
using UnityEngine.Assertions;
using System;

public class MiscellaneousFunctions : MonoBehaviour
{
    public static void IntroAnimation() {
        GameObject ship = MainLogic.mainLogic.Ship;
        Transform CameraTarget = ship.transform.GetChild(0);
        Assert.AreEqual(CameraTarget.name, "CameraTarget");
        Transform MainCamera = MainLogic.mainLogic.MainCamera.transform;
        StateSwitcher switcher = MainLogic.mainLogic.StateSwitcher;
        switcher.SwitchState(State.CameraAnimation);

        SerialDisposable instant_disposable = new();
        float speed = 0f;
        instant_disposable.Disposable = Observable
            .NextFrame()
            .Delay(TimeSpan.FromSeconds(2))
            .Subscribe(_ => {
                SerialDisposable instant_disposable2 = new();
                instant_disposable2.Disposable = Observable
                    .EveryUpdate()
                    .TakeWhile(_ => Vector3.Distance(CameraTarget.position, MainCamera.position) > 2)
                    .Subscribe(_ => {
                        MainCamera.Translate(
                            Settings.GameStartCameraArriveSpeed.Value
                            *speed
                            *Time.deltaTime
                            *(CameraTarget.position - MainCamera.position)
                        );
                        speed += Time.deltaTime;
                    }, _ => {}, _ => {
                        MainCamera.parent = CameraTarget;
                        MainLogic.mainLogic.StateSwitcher.SwitchState(State.Game);
                        instant_disposable2.Dispose();
                    });
                instant_disposable.Dispose();
            })
        ;
    }
}
