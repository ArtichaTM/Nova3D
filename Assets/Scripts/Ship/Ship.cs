using UnityEngine;
using R3;
using System;
using UnityEngine.Assertions;

public class Ship : MonoBehaviour
{
    Transform CameraTarget;
    Transform MainCamera;

    public ShipInGame shipInGame {
        get; private set;
    }

    void Start() {
        shipInGame = GetComponent<ShipInGame>();
        CameraTarget = gameObject.transform.GetChild(0);
        MainCamera = GameObject.Find("Main Camera").transform;
        Assert.AreEqual(CameraTarget.name, "CameraTarget");
    }

    public void StartGame() {
        shipInGame = GetComponent<ShipInGame>();
        shipInGame.RiBo.velocity = new Vector3(0, 0, 8f);

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
                        instant_disposable2.Dispose();
                    });
                instant_disposable.Dispose();
            });
            
    }

    public void PauseGame() {
        shipInGame.Pause();
    }

    public void ResumeGame() {
        shipInGame.Resume();
    }

    public void FinishGame() {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        shipInGame.disposable.Dispose();
    }
}
