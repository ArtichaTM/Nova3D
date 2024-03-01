using UnityEngine;
using R3;
using System;

public class Ship : MonoBehaviour
{
    public ShipInGame shipInGame {
        get; private set;
    }

    void Start() {
        shipInGame = GetComponent<ShipInGame>();
        Debug.Log(shipInGame);
    }

    public void StartGame() {
        shipInGame = GetComponent<ShipInGame>();
        shipInGame.RiBo.velocity = new Vector3(0, 0, 5f);
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
