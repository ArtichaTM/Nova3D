using System.Collections.Generic;
using UnityEngine;
using R3;

public class ProjectionController : MonoBehaviour
{
    DisposableBag Disposables;

    readonly public ReactiveProperty<float> FresnelPower = new(1f);
    readonly public ReactiveProperty<Color> FresnelColor = new();

    readonly List<Projection> Projections = new(8);
    Transform Model = null;
    Transform ProjectionsParent;
    BoxCollider Boundary => MainLogic.Instance.Boundary.GetComponent<BoxCollider>();

    void Start()
    {
        foreach (Transform child in transform) {
            if (child.name == "Model") {
                Model = child;
                break;
            }
        }
        if (Model == null)
            throw new UnityException($"Can't find \"Model\" gameobject in {name}");

        ProjectionsParent = new GameObject($"{name}_Projections").transform;
        ProjectionsParent.SetPositionAndRotation(transform.position, transform.rotation);

        Bounds bounds = Boundary.bounds;
        for (short sign_x = -1; sign_x < 2; sign_x += 1)
            for (short sign_y = -1; sign_y < 2; sign_y += 1)
                for (short sign_z = -1; sign_z < 2; sign_z += 1)
        {
                    if (sign_x == 0 && sign_y == 0 && sign_z == 0) continue;

                    Vector3 position_offset = bounds.size;
                    position_offset.x *= sign_x;
                    position_offset.y *= sign_y;
                    position_offset.z *= sign_z;

                    Transform projectionTransform = Instantiate(
                        original: Model,
                        position: transform.position + position_offset,
                        rotation: transform.rotation,
                        parent  : ProjectionsParent
                    );
                    projectionTransform.name = $"{sign_x} {sign_y} {sign_z}";

                    Projection projection = projectionTransform.gameObject.AddComponent<Projection>();
                    Projections.Add(projection);
                    projection.Offset = position_offset;
                    projection.ParentFollow = transform;
        }

        Disposables = new(2);
        FresnelPower
            .Subscribe(power => Projections.ForEach(projection => projection.FresnelPower.Value = power))
            .AddTo(ref Disposables)
            ;
        FresnelColor
            .Subscribe(color => Projections.ForEach(projection => projection.FresnelColor.Value = color))
            .AddTo(ref Disposables)
            ;
    }

    void OnDestroy() {
        if (Model != null) {
            Disposables.Dispose();
            Destroy(ProjectionsParent.gameObject);
        }
    }
}
