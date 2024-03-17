using R3;
using UnityEngine;

public class Settings : MonoBehaviour
{

    #region VisibleParameters
    public static SerializableReactiveProperty<float> transitionsSpeed = new(0.3f);
    public static SerializableReactiveProperty<bool> invertedMouseVertical = new(false);
    public static SerializableReactiveProperty<bool> invertedMouseHorizontal = new(false);
    public static SerializableReactiveProperty<float> GameStartCameraArriveSpeed = new(1f);
    public static SerializableReactiveProperty<bool> EqualizeYawPitch = new(true);
    public static SerializableReactiveProperty<bool> PreciseProjections = new(false);
    public static SerializableReactiveProperty<Color> BoundaryColor = new(Color.green);
    public static SerializableReactiveProperty<float> BoundaryAppearDistance = new(20f);
    public static SerializableReactiveProperty<float> BoundaryHoleFactor = new(20f);
    public static SerializableReactiveProperty<float> BoundaryMinimumOpacity = new(0f);
    public static SerializableReactiveProperty<float> BoundaryMaximumOpacity = new(1f);

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
