using R3;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    // ParticleComponent[] ParticleComponents;

    readonly public ReactiveProperty<bool> Paused = new(true);

    void Start()
    {
        // ParticleComponents = GetComponentsInChildren<ParticleComponent>();
        gameObject.SetActive(true);
    }
}
