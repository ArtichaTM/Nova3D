using System.Collections.Generic;
using UnityEngine;

public class AllParticles : MonoBehaviour
{
    readonly Dictionary<string, Transform> particles = new();

    void Start() {
        foreach (Transform child in transform) {
            particles.Add(child.name, child);
            child.gameObject.SetActive(false);
        }
    }

    public Transform SetParticle(string name, Transform target) {
        Transform particle = particles[name];
        particle.parent = target;
        return particle;
    }

    public Transform SetParticle(string name, Transform target, bool enable) {
        Transform particle = SetParticle(name, target);
        particle.gameObject.SetActive(enable);
        return particle;
    }
}
