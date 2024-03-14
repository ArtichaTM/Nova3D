using UnityEngine;
using R3;


public class ProjectionsController : MonoBehaviour
{
    BoxCollider Boundary => MainLogic.Instance.Boundary.GetComponent<BoxCollider>();
    Transform Model = null;
    Transform ProjectionsParent;

    readonly public ReactiveProperty<bool> Paused = new(true);
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

        ProjectionsParent = new GameObject("Projections").transform;
        ProjectionsParent.SetPositionAndRotation(transform.position, transform.rotation);
        ProjectionsParent.parent = transform;

        Bounds bounds = Boundary.bounds;
        for (short sign_x = -1; sign_x < 2; sign_x += 1) {
            for (short sign_y = -1; sign_y < 2; sign_y += 1) {
                for (short sign_z = -1; sign_z < 2; sign_z += 1) {
                    if (sign_x == 0 && sign_y == 0 && sign_z == 0) continue;

                    Vector3 position_offset = bounds.size;
                    position_offset.x *= sign_x;
                    position_offset.y *= sign_y;
                    position_offset.z *= sign_z;
                    Transform projection = Instantiate(
                        original: Model,
                        position: transform.position + position_offset,
                        rotation: transform.rotation,
                        parent  : ProjectionsParent
                    );
                    projection.name = $"{sign_x} {sign_y} {sign_z}";
                }
            }
        }
    }
}
