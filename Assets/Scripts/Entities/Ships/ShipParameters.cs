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
    #region Disposables
    DisposableBag Disposables;
    SerialDisposable ShieldCooldownUpdateDisposable;
    SerialDisposable HealthCooldownUpdateDisposable;
    SerialDisposable ShieldRegenerationUpdateDisposable;
    SerialDisposable HealthRegenerationUpdateDisposable;
    #endregion

    #region ColdVariables
    readonly public ReactiveProperty<Parameter> SpeedForward = new(Parameter.Create(10f));
    readonly public ReactiveProperty<Parameter> SpeedBackwards = new(Parameter.Create(5f));
    readonly public ReactiveProperty<Parameter> SpeedRotationRoll = new(Parameter.Create(.04f));
    readonly public ReactiveProperty<Parameter> SpeedRotationYaw = new(Parameter.Create(1f));
    readonly public ReactiveProperty<Parameter> SpeedRotationPitch = new(Parameter.Create(10f));
    readonly public ReactiveProperty<Parameter> HealthMax = new(Parameter.Create(100f));
    readonly public ReactiveProperty<Parameter> ShieldMax = new(Parameter.Create(100f));
    readonly public ReactiveProperty<Parameter> FireCooldownMax = new(Parameter.Create(1f));

    // Applies every FixedUpdate
    // Delay in seconds
    readonly public ReactiveProperty<Parameter> ShieldRegeneration = new(Parameter.Create(10f));
    readonly public ReactiveProperty<Parameter> HealthRegeneration = new(Parameter.Create(10f));
    readonly public ReactiveProperty<Parameter> ShieldRegenerationDelay = new(Parameter.Create(10f));
    readonly public ReactiveProperty<Parameter> HealthRegenerationDelay = new(Parameter.Create(7f));
    #endregion

    #region HotVariables
    readonly public ReactiveProperty<float> Health = new();
    readonly public ReactiveProperty<float> Shield = new();
    readonly public ReactiveProperty<float> FireCooldown = new();
    readonly public ReactiveProperty<float> ShieldCooldwon = new();
    readonly public ReactiveProperty<float> HealthCooldwon = new();

    #endregion

    #region Shortcuts
    public float MovementSpeed => (SpeedForward.Value + SpeedBackwards.Value) / 2;
    public float ShipDurability => Shield.Value + Health.Value;
    public float ShipDurabilityMax => ShieldMax.Value + HealthMax.Value;
    #endregion

    public readonly List<ParticleController> ParticleThrusters = new();

    List<Transform> GetAllThrusters() => MainLogic.FindChildrenByName(transform, "ThrustTarget");

    void Start()
    {
        Shield.Value = ShieldMax.Value/2;
        Health.Value = HealthMax.Value/2;
        FireCooldown.Value = FireCooldownMax.Value;
        HealthCooldwon.Value = HealthRegenerationDelay.Value;
        ShieldCooldwon.Value = ShieldRegenerationDelay.Value;

        foreach(Transform thrusterTarget in GetAllThrusters()) {
            ParticleThrusters.Add(MainLogic.Instance.AddParticleSystem("Thruster", thrusterTarget));
        }

        Disposables = new(3);

        MainLogic.Instance.Paused
            .Skip(1) // Skipping first call because Paused by default
            .Where(x => x == true)
            .Subscribe(_ => Pause())
            .AddTo(ref Disposables)
            ;
        MainLogic.Instance.Paused
            .Where(x => x == false)
            .Subscribe(_ => Resume())
            .AddTo(ref Disposables)
            ;
        MainLogic.Instance.Finished
            .Skip(1)
            .Subscribe(_ => Finish())
            .AddTo(ref Disposables)
            ;
        HealthCooldownUpdateDisposable = new()
        { Disposable = Observable
            .EveryUpdate(UnityFrameProvider.FixedUpdate)
            .Subscribe(HealthCooldownUpdate)
        };
        ShieldCooldownUpdateDisposable = new()
        { Disposable = Observable
            .EveryUpdate(UnityFrameProvider.FixedUpdate)
            .Subscribe(ShieldCooldownUpdate)
        };
    }

    // void TookDamage() {
    // }

    void ShieldCooldownUpdate(Unit _) {
        ShieldCooldwon.Value -= Time.fixedDeltaTime;
        if (ShieldCooldwon.Value < 0) {
            ShieldCooldwon.Value = Parameter.Create(0f);
            ShieldCooldownUpdateDisposable.Dispose();
            ShieldRegenerationUpdateDisposable = new()
            { Disposable = Observable
                .EveryUpdate(UnityFrameProvider.FixedUpdate)
                .Subscribe(ShieldRegenerationUpdate)
            };
        }
    }

    void HealthCooldownUpdate(Unit _) {
        HealthCooldwon.Value -= Time.fixedDeltaTime;
        if (HealthCooldwon.Value < 0) {
            HealthCooldwon.Value = Parameter.Create(0f);
            HealthCooldownUpdateDisposable.Dispose();
            HealthRegenerationUpdateDisposable = new()
            { Disposable = Observable
                .EveryUpdate(UnityFrameProvider.FixedUpdate)
                .Subscribe(HealthRegenerationUpdate)
            };
        }
    }

    void HealthRegenerationUpdate(Unit _) {
        float NewHealth = Health.Value + Time.fixedDeltaTime*HealthRegeneration.Value;
        if (NewHealth > HealthMax.Value) {
            Health.Value = HealthMax.Value;
        } else {
            Health.Value = NewHealth;
        }

    }

    void ShieldRegenerationUpdate(Unit _) {
        float NewShield = Shield.Value + Time.fixedDeltaTime*ShieldRegeneration.Value;
        if (NewShield > ShieldMax.Value) {
            Shield.Value = ShieldMax.Value;
        } else {
            Shield.Value = NewShield;
        }
    }

    void Pause() {
    }

    void Resume() {
    }

    void Finish() {
        // Disposables.Dispose(); Another BUG
        if (!HealthCooldownUpdateDisposable?.IsDisposed ?? false) HealthCooldownUpdateDisposable.Dispose();
        if (!ShieldCooldownUpdateDisposable?.IsDisposed ?? false) ShieldCooldownUpdateDisposable.Dispose();
        if (!HealthRegenerationUpdateDisposable?.IsDisposed ?? false) HealthRegenerationUpdateDisposable.Dispose();
        if (!ShieldRegenerationUpdateDisposable?.IsDisposed ?? false) ShieldRegenerationUpdateDisposable.Dispose();
    }
}
