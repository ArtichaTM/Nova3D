using System.Collections.Generic;
using UnityEngine;
using R3;

public class Projection : MonoBehaviour
{
    DisposableBag Disposables;

    readonly public ReactiveProperty<float> FresnelPower = new(1f);
    readonly public ReactiveProperty<Color> FresnelColor = new();

    List<MeshRenderer> renderers;

    void Start()
    {
        renderers = new List<MeshRenderer>(GetComponentsInChildren<MeshRenderer>());
        renderers.ForEach(
            x => x.material = MainLogic.Instance.Assets.ProjectionAura
        );

        Disposables = new(2);
        FresnelPower
            .Subscribe(power => renderers.ForEach(
                render => render.material.SetFloat("FresnelPower", power)
            ))
            .AddTo(ref Disposables)
            ;

        FresnelColor
            .Subscribe(color => renderers.ForEach(
                render => render.material.SetColor("FresnelColor", color)
            ))
            .AddTo(ref Disposables)
            ;
    }
}
