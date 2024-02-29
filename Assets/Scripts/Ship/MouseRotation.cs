using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseRotation : MonoBehaviour
{
    public bool isPaused = false;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void RotateToMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - transform.position;
        float angle = Vector2.SignedAngle(Vector2.up, direction);
        Vector3 targetRotation = new(0, 0, angle);
        Quaternion quat = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRotation), 720 * Time.deltaTime);
        rb.MoveRotation(quat);
    }

    void Update()
    {
        // if (!PauseControl.gameIsPaused) {
            RotateToMouse();
        // }
    }
}
