using UnityEngine;

public class ParticleTargetSettings : MonoBehaviour
{

    #region InspectorParameters
    public ParticleSystem[] targets;
    public bool DisableEmissionsOnDisable;
    public bool StartEnabled;
    public bool RotationCopy;
    public bool ShapeRadiusCopy;
    #endregion
}
