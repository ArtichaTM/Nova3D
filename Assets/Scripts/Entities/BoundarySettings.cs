using UnityEngine;
using R3;

[RequireComponent(typeof(MeshRenderer))]
public class BoundarySettings : MonoBehaviour
{
    DisposableBag Disposables = new(5);

    public ReactiveProperty<Color> Color;
    public ReactiveProperty<float> AppearDistance;
    public ReactiveProperty<float> HoleFactor;
    public ReactiveProperty<float> MinimumOpacity;
    public ReactiveProperty<float> MaximumOpacity;

    void Start()
    {
        MeshRenderer render = GetComponent<MeshRenderer>();
        Color
            .Subscribe(color => render.material.SetColor("Color", color))
            .AddTo(ref Disposables)
            ;
        AppearDistance
            .Subscribe(distance => render.material.SetFloat("AppearDistance", distance))
            .AddTo(ref Disposables)
            ;
        HoleFactor
            .Subscribe(factor => render.material.SetFloat("HoleFactor", factor))
            .AddTo(ref Disposables)
            ;
        MinimumOpacity
            .Subscribe(opacity => render.material.SetFloat("MinimumOpacity", opacity))
            .AddTo(ref Disposables)
            ;
        MaximumOpacity
            .Subscribe(opacity => render.material.SetFloat("MaximumOpacity", opacity))
            .AddTo(ref Disposables)
            ;
    }

    void OnDestroy() => Disposables.Dispose();
}
