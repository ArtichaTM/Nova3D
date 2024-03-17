using UnityEngine;
using UnityEngine.Assertions;

public class AssetsExplorer : MonoBehaviour
{
    public Material ProjectionAura;

    void Start() {
        Assert.IsNotNull(ProjectionAura);
    }
}
