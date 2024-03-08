using UnityEngine;
using R3;

public class ShipParameters : MonoBehaviour
{
    #region ConstantVariables
    readonly public ReactiveProperty<float> SpeedForward = new(1f);
    readonly public ReactiveProperty<float> SpeedBackwards = new(1f);
    readonly public ReactiveProperty<float> SpeedRotationX = new(1f);
    readonly public ReactiveProperty<float> SpeedRotationY = new(1f);
    #endregion

    #region VariableVariables
    readonly public ReactiveProperty<float> MaxHealth = new(100f);
    readonly public ReactiveProperty<float> MaxShields = new(100f);
    readonly public ReactiveProperty<float> Health = new(100f);
    readonly public ReactiveProperty<float> Shields = new(100f);
    #endregion

    void Start()
    {

    }
}
