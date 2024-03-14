using UnityEngine;
using R3;

public class ParticleController : MonoBehaviour
{
    #region Disposables
    DisposableBag Disposables;
    #endregion

    public ParticleSystem[] ParticleSystems {get; private set; }

    readonly public ReactiveProperty<bool> Paused = new(true);

    void Start()
    {
        ParticleSystems = GetComponentsInChildren<ParticleSystem>();
        gameObject.SetActive(true);

        Disposables = new();
        Transform TargetObject = transform.parent;
        foreach (ParticleTargetSettings particleSettings in TargetObject.GetComponents<ParticleTargetSettings>()) {
            foreach (ParticleSystem _particleSystem in particleSettings.targets) {
                GameObject SettingsTargetObject = _particleSystem.gameObject;
                ParticleSystem sys = null;
                foreach (ParticleSystem PossibleParticleSystem in ParticleSystems) {
                    if (PossibleParticleSystem.gameObject.name == SettingsTargetObject.name) {
                        sys = PossibleParticleSystem;
                        break;
                    }
                }
                if (sys == null)
                    throw new UnityException("Can't find target Particle System. Is it renamed, or object set incorrect?");

                if (particleSettings.DisableEmissionsOnDisable) {
                    Paused
                        .Subscribe(x => {
                            ParticleSystem.EmissionModule emissions = sys.emission;
                            emissions.enabled = x;
                        })
                        .AddTo(ref Disposables)
                        ;
                }
                if (!particleSettings.StartEnabled) {
                    ParticleSystem.EmissionModule emissions = sys.emission;
                    emissions.enabled = false;
                }
                if (particleSettings.RotationCopy) {
                    SettingsTargetObject.transform.rotation = TargetObject.rotation;
                }
                if (particleSettings.ShapeRadiusCopy) {
                    ParticleSystem.ShapeModule shape = sys.shape;
                    shape.radius = TargetObject.localScale.x;
                }
            }
        }
    }

    void OnDestroy() => Disposables.Dispose();
}
