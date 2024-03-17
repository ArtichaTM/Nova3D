using System.Collections.Generic;
using UnityEngine;
using R3;

public class Projection : MonoBehaviour
{
    DisposableBag Disposables = new(2);

    readonly public ReactiveProperty<float> FresnelPower = new(1f);
    readonly public ReactiveProperty<Color> FresnelColor = new();

    List<MeshRenderer> renderers;
    public Vector3 Offset = Vector3.zero;
    public Transform ParentFollow = null;
    void Start()
    {
        renderers = new List<MeshRenderer>(GetComponentsInChildren<MeshRenderer>());
        renderers.ForEach(
            x => x.material = MainLogic.Instance.Assets.ProjectionAura
        );

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

        Observable
            .EveryUpdate(Settings.PreciseProjections.Value ? UnityFrameProvider.Update : UnityFrameProvider.FixedUpdate)
            .Subscribe(_ => transform.SetPositionAndRotation(
                ParentFollow.position + Offset,
                ParentFollow.rotation
            ), _ => {if (ParentFollow == null) return; }, null)
            .AddTo(ref Disposables)
            ;
    }

    void OnDestroy() => Disposables.Dispose();
}
