using System;
using R3;
using UnityEngine;

public class Settings : MonoBehaviour
{
    #region VisibleParameters
    public static ReactiveProperty<float> transitionsSpeed = new(0.3f);
    public static ReactiveProperty<bool> invertedMouseVertical = new(false);
    public static ReactiveProperty<bool> invertedMouseHorizontal = new(false);
    public static ReactiveProperty<float> GameStartCameraArriveSpeed = new(1f);
    public static ReactiveProperty<bool> EqualizeYawPitch = new(true);
    public static ReactiveProperty<bool> PreciseProjections = new(false);

    public static float InvertMouseVertical() => invertedMouseVertical.Value switch {
        true => -1,
        false => 1
    };
    public static float InvertMouseHorizontal() => invertedMouseHorizontal.Value switch {
        true => -1,
        false => 1
    };

    #endregion

    #region ReadonlyParameters

    public readonly static float spawnSpeed = 300f;
    public readonly static string teleporterName = "TeleportTrigger";
    public readonly static float CameraAnimationDelay = 2f;
    public readonly static float CameraAnimationDistanceMinimum = 2f;


    #endregion
}
