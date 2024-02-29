using UnityEngine;


public class MouseLock : MonoBehaviour
{
    public delegate void Coordinates(float x, float y);
    Rigidbody rb;

    public Coordinates OnAxisChange;

    void OnEnable() {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Debug.Log("InVisible");
    }

    void OnDisable() {
        Cursor.lockState = CursorLockMode.None;
        Debug.Log("Visible");
    }

    void Update() => OnAxisChange(
        Input.GetAxis("Mouse X")*Settings.InvertMouseHorizontal(),
        Input.GetAxis("Mouse Y")*Settings.InvertMouseVertical());
}
