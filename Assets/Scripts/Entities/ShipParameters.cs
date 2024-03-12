using System.Collections.Generic;
using UnityEngine;
using R3;

public struct Parameter {
    private readonly float additions;
    private readonly float multipliers;
    public readonly float Value;

    Parameter(float _additions, float _multipliers)
    {
        additions = _additions;
        multipliers = _multipliers;
        Value = additions*multipliers;
    }

    public static Parameter Create(float _additions, float _multiplier) => new(_additions, _multiplier);
    public static Parameter Create(float startValue) => Create(startValue, 1f);
    public static Parameter Create() => Create(0f, 1f);

    #region operators
    public static Parameter operator +(Parameter a, float     b) => new(a.additions+b, a.multipliers  );
    public static Parameter operator -(Parameter a, float     b) => new(a.additions-b, a.multipliers  );
    public static Parameter operator *(Parameter a, float     b) => new(a.additions  , a.multipliers*b);
    public static Parameter operator /(Parameter a, float     b) => new(a.additions  , a.multipliers/b);
    public static Parameter operator +(Parameter a, Parameter b) => new(a.additions+b.additions, a.multipliers+b.multipliers);
    public static Parameter operator -(Parameter a, Parameter b) => new(a.additions-b.additions, a.multipliers-b.multipliers);
    public static Parameter operator *(Parameter a, Parameter b) => new(a.additions*b.additions, a.multipliers*b.multipliers);
    public static Parameter operator /(Parameter a, Parameter b) => new(a.additions/b.additions, a.multipliers/b.multipliers);
    public static implicit operator float(Parameter parameter) => parameter.Value;
    #endregion
}

public class ShipParameters : MonoBehaviour
{
    #region ColdVariables
    readonly public ReactiveProperty<Parameter> SpeedForward = new(Parameter.Create(10f));
    readonly public ReactiveProperty<Parameter> SpeedBackwards = new(Parameter.Create(5f));
    readonly public ReactiveProperty<Parameter> SpeedRotationRoll = new(Parameter.Create(.04f));
    readonly public ReactiveProperty<Parameter> SpeedRotationYaw = new(Parameter.Create(1f));
    readonly public ReactiveProperty<Parameter> SpeedRotationPitch = new(Parameter.Create(10f));
    readonly public ReactiveProperty<Parameter> ShipBasicAbilities = new(Parameter.Create(1f));
    readonly public ReactiveProperty<Parameter> MaxHealth = new(Parameter.Create(100f));
    readonly public ReactiveProperty<Parameter> MaxShield = new(Parameter.Create(100f));
    #endregion

    #region HotVariables
    readonly public ReactiveProperty<Parameter> Health = new(Parameter.Create());
    readonly public ReactiveProperty<Parameter> Shield = new(Parameter.Create());
    #endregion

    #region Shortcuts
    public float MovementSpeed => (SpeedForward.Value+SpeedBackwards.Value)/2;
    public float ShipDurability => Shield.Value+Health.Value;
    public float ShipDurabilityMax => MaxShield.Value+MaxHealth.Value;

    public readonly List<ParticleController> ParticleThrusters = new();
    #endregion

    List<Transform> GetAllThrusters() => MainLogic.FindChildrenByName(transform, "ThrustTarget");

    void Start()
    {
        Shield.Value = MaxShield.Value/2;
        Health.Value = MaxHealth.Value/2;

        foreach(Transform thrusterTarget in GetAllThrusters()) {
            ParticleThrusters.Add(MainLogic.Instance.AddParticleSystem("Thruster", thrusterTarget));
        }
    }
}
