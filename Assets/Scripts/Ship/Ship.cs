using UnityEngine;
using R3;

public class Ship : MonoBehaviour
{
    ShipInGame shipInGame;

    void Start() {
        shipInGame = GetComponent<ShipInGame>();
        Debug.Log(shipInGame);
    }

    public void StartGame() {
        shipInGame = GetComponent<ShipInGame>();
        shipInGame.rb.velocity = new Vector3(0, 0, 5f);
        // shipInGame.enabled = true;
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
