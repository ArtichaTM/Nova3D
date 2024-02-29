using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class MouseLock : MonoBehaviour
{
    Rigidbody rb;

    void Start()
    {
        enabled = false;
    }

    void OnEnable() {
        rb = GetComponent<Rigidbody>();
        Cursor.visible = false;
    }

    void OnDisable() {
        Cursor.visible = true;  
    }

    void Update()
    {
        // rb.AddTorque(0, 1f, 0);
    }
}
