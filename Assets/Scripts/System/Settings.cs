using System.Collections;
using System.Collections.Generic;
using R3;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static ReactiveProperty<float> transitionsSpeed = new(0.7f);
    public static ReactiveProperty<bool> invertedMouseVertical = new(true);
    public static ReactiveProperty<bool> invertedMouseHorizontal = new(false);
    public static ReactiveProperty<float> GameStartCameraArriveSpeed = new(1f);

    public static float InvertMouseVertical() => invertedMouseVertical.Value switch {
        true => -1,
        false => 1
    };
    public static float InvertMouseHorizontal() => invertedMouseHorizontal.Value switch {
        true => -1,
        false => 1
    };
}
