using System;
using R3;
using R3.Triggers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BoundaryTeleporter : MonoBehaviour
{
    #region Components
    BoxCollider CenterCollider;
    Rigidbody ribo;
    #endregion

    #region Disposables
    readonly CompositeDisposable Disposables = new();
    #endregion

    static readonly float size = 0f;

    void Start()
    {
        ribo = GetComponent<Rigidbody>();

        GameObject colliderObject = new GameObject("TeleportTrigger");
        colliderObject.transform.SetPositionAndRotation(transform.position + ribo.centerOfMass, transform.rotation);
        colliderObject.transform.parent = transform;
        colliderObject.tag = "BoundaryColliders";

        CenterCollider = colliderObject.AddComponent<BoxCollider>();
        CenterCollider.isTrigger = true;
        CenterCollider.providesContacts = true;
        CenterCollider.size = new(size, size, size);

        CenterCollider
            .OnTriggerExitAsObservable()
            .Where(x => x.gameObject.name == "Boundary")
            .Subscribe(InitiateTeleport)
            .AddTo(Disposables);
    }

    void InitiateTeleport(Collider boundary) {
        Vector3 fromCenter = CenterCollider.transform.position - boundary.transform.position;
        Vector3 extents = boundary.bounds.extents;

        if (Math.Abs(fromCenter.x) > extents.x) {
            if (fromCenter.x < 0)
                ribo.MovePosition(new(extents.x, 0f, 0f));
            else
                ribo.MovePosition(new(-extents.x, 0f, 0f));
        }
        else if (Math.Abs(fromCenter.y) > extents.y) {
            if (fromCenter.y < 0)
                ribo.MovePosition(new(0f, extents.y, 0f));
            else
                ribo.MovePosition(new(0f, -extents.y, 0f));
        }
        else if (Math.Abs(fromCenter.z) > extents.z) {
            if (fromCenter.z < 0)
                ribo.MovePosition(new(0f, 0f, extents.z));
            else
                ribo.MovePosition(new(0f, 0f, -extents.z));
        }
        Debug.Log($"target bounds: {boundary.bounds}, Vector: {fromCenter}");
    }

    void OnDestroy() {
        Disposables.Dispose();
        if (!CenterCollider.gameObject.IsDestroyed())
            Destroy(CenterCollider.gameObject);
    }
}
