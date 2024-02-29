using UnityEngine;
using System.Collections;
using System;
using Unity.VisualScripting;
using UnityEngine.Assertions;

public class Ship : MonoBehaviour
{
    ShipInGame shipInGame;

    void Start() {
        shipInGame = GetComponent<ShipInGame>();
        Debug.Log(shipInGame);
    }

    public void StartGame() {
        shipInGame = GetComponent<ShipInGame>();
        // shipInGame.rb.velocity = new Vector3(0, 0, 1f);
        // shipInGame.enabled = true;
    }

    public void PauseGame() {
        shipInGame.enabled = false;
    }

    public void ResumeGame() {
        shipInGame.enabled = true;
    }

    public void FinishGame() {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
