using UnityEngine;
using System.Collections;
using System;
using Unity.VisualScripting;
using UnityEngine.Assertions;

public class Ship : MonoBehaviour
{
    bool GameOn = false;
    ShipInGame shipInGame;

    void Start() {
        shipInGame = GetComponent<ShipInGame>();
        Debug.Log(shipInGame);
    }

    public void StartGame() {
        Assert.IsFalse(GameOn);
        shipInGame.enabled = true;
        shipInGame.rb.velocity = new Vector3(0, 0, 5f);
    }

    public void FinishGame() {
        Assert.IsTrue(GameOn);
        shipInGame.enabled = false;
    }
}
