using UnityEngine;
using R3;

public struct Parameter {
    private readonly float additions;
    private readonly float multipliers;
    private float Value;

    Parameter(float _additions, float _multipliers)
    {
        additions = _additions;
        multipliers = _multipliers;
        Value = additions*multipliers;
    }

    public static Parameter Create(float _additions, float _multiplier) => new(_additions, _multiplier);
    public static Parameter Create(float startValue) => Create(startValue, 1f);
    public static Parameter Create() => Create(0f, 1f);

    public static Parameter operator +(Parameter a, float b) => new(a.additions+b, a.multipliers  );
    public static Parameter operator -(Parameter a, float b) => new(a.additions-b, a.multipliers  );
    public static Parameter operator *(Parameter a, float b) => new(a.additions  , a.multipliers*b);
    public static Parameter operator /(Parameter a, float b) => new(a.additions  , a.multipliers/b);
    public static implicit operator float(Parameter parameter) => parameter.Value;
}

public class ShipParameters : MonoBehaviour
{
    #region ConstantVariables
    readonly public ReactiveProperty<Parameter> SpeedForward = new(Parameter.Create(1f));
    readonly public ReactiveProperty<Parameter> SpeedBackwards = new(Parameter.Create(1f));
    readonly public ReactiveProperty<Parameter> SpeedRotationX = new(Parameter.Create(1f));
    readonly public ReactiveProperty<Parameter> SpeedRotationY = new(Parameter.Create(1f));
    readonly public ReactiveProperty<Parameter> ShipBasicAbilities = new(Parameter.Create(1f));
    #endregion

    #region VariableVariables
    readonly public ReactiveProperty<Parameter> Health = new(Parameter.Create());
    readonly public ReactiveProperty<Parameter> MaxHealth = new(Parameter.Create(100f));
    readonly public ReactiveProperty<Parameter> Shield = new(Parameter.Create());
    readonly public ReactiveProperty<Parameter> MaxShield = new(Parameter.Create(100f));
    #endregion

    void Start()
    {
        Shield.Value = MaxShield.Value/2;
        Health.Value = MaxHealth.Value/2;
    }
}
