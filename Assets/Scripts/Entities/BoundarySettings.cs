using UnityEngine;
using R3;

[RequireComponent(typeof(MeshRenderer))]
public class BoundarySettings : MonoBehaviour
{
    DisposableBag Disposables = new(11);

    readonly public ReactiveProperty<Color> Color = new(UnityEngine.Color.green);
    readonly public ReactiveProperty<float> AppearDistance = new();
    readonly public ReactiveProperty<float> HoleFactor = new();
    readonly public ReactiveProperty<float> MinimumOpacity = new();
    readonly public ReactiveProperty<float> MaximumOpacity = new();

    void Start()
    {
        MeshRenderer render = GetComponent<MeshRenderer>();

        Settings.BoundaryColor
            .Subscribe(color => Color.Value = color)
            .AddTo(ref Disposables)
            ;
        Settings.BoundaryAppearDistance
            .Subscribe(distance => AppearDistance.Value = distance)
            .AddTo(ref Disposables)
            ;
        Settings.BoundaryHoleFactor
            .Subscribe(factor => HoleFactor.Value = factor)
            .AddTo(ref Disposables)
            ;
        Settings.BoundaryMinimumOpacity
            .Subscribe(opacity => MinimumOpacity.Value = opacity)
            .AddTo(ref Disposables)
            ;
        Settings.BoundaryMaximumOpacity
            .Subscribe(opacity => MaximumOpacity.Value = opacity)
            .AddTo(ref Disposables)
            ;

        Color
            .Subscribe(color => render.material.SetColor("_Color", color))
            .AddTo(ref Disposables)
            ;
        AppearDistance
            .Subscribe(distance => render.material.SetFloat("_AppearDistance", distance))
            .AddTo(ref Disposables)
            ;
        HoleFactor
            .Subscribe(factor => render.material.SetFloat("_HoleFactor", factor))
            .AddTo(ref Disposables)
            ;
        MinimumOpacity
            .Subscribe(opacity => render.material.SetFloat("_MinimumOpacity", opacity))
            .AddTo(ref Disposables)
            ;
        MaximumOpacity
            .Subscribe(opacity => render.material.SetFloat("_MaximumOpacity", opacity))
            .AddTo(ref Disposables)
            ;
    }

    void OnDestroy() => Disposables.Dispose();
}
