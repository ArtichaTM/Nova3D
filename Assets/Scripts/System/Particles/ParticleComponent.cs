using R3;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleComponent : MonoBehaviour
{
    ParticleSystem sys;

    #region Disposables
    CompositeDisposable Disposables;
    #endregion

    #region InspectorParameters
    [SerializeField] bool DisableEmissionsOnDisable;
    [SerializeField] bool StartEnabled;
    #endregion

    void Start()
    {
        Assert.IsNotNull(transform.parent);
        Assert.IsNotNull(transform.parent.GetComponent<ParticleController>());
        Assert.IsNotNull(transform.parent.GetComponent<ParticleController>().Paused);

        sys = GetComponent<ParticleSystem>();
        Disposables = new();
        ReactiveProperty<bool> paused = transform.parent.GetComponent<ParticleController>().Paused;

        if (DisableEmissionsOnDisable) {
            paused
                .Subscribe(x => {
                    ParticleSystem.EmissionModule emissions = sys.emission;
                    emissions.enabled = x;
                })
                .AddTo(Disposables)
                ;
        }
        if (!StartEnabled) {
            ParticleSystem.EmissionModule emissions = sys.emission;
            emissions.enabled = false;
        }

    }

    void OnDestroy() => Disposables.Dispose();
}
