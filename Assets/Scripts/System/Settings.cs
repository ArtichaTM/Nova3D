using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static float transitionsSpeed = 0.7f;
    public static bool invertedMouseVertical = true;
    public static bool invertedMouseHorizontal = false;

    public static float InvertMouseVertical() => invertedMouseVertical switch {
        true => -1,
        false => 1
    };
    public static float InvertMouseHorizontal() => invertedMouseHorizontal switch {
        true => -1,
        false => 1
    };
}
