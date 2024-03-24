using System;
using UnityEngine;

namespace ShipTypes {
    abstract public class ShipType
    {
        protected virtual GameObject PrefabModelOrigin {get; set;} = null;
        public virtual JSONInfo.ShipInfo ShipInfo { get; }
        public  Transform MainObject { get; protected set; } = null;
        readonly static Type[] ShipComponents = {
            typeof(BoxCollider), typeof(Rigidbody), typeof(BoundaryTeleporter),
            typeof(ShipParameters), typeof(ProjectionController)
        };

        public ShipType(string name, Vector3 position, Quaternion rotation) {
            if (PrefabModelOrigin == null) {
                PrefabModelOrigin = Resources.Load<GameObject>(ShipInfo.PrefabPath);
            }

            MainObject = new GameObject(name, ShipComponents).transform;
            Debug.Log($"Box: {MainObject.GetComponent<BoxCollider>()}");
            MainObject.SetPositionAndRotation(position, rotation);

            Transform modelObject = GameObject.Instantiate(PrefabModelOrigin, MainObject).transform;
            modelObject.name = "Model";
            modelObject.SetLocalPositionAndRotation(
                ShipInfo.ModelOffsetPosition,
                ShipInfo.ModelOffsetRotation
            );
        }
    }

    namespace Players {
        abstract public class PlayerShip : ShipType {
            readonly static Type[] ShipComponents = {
                typeof(Ship), typeof(GameShipUI), typeof(MouseLock),
            };

            public PlayerShip(Vector3 position, Quaternion rotation) : base("Ship", position, rotation) {
                foreach (Type componentType in ShipComponents) {
                    MainObject.gameObject.AddComponent(componentType);
                }
                Transform cameraTarget = new GameObject("CameraTarget").transform;
                cameraTarget.parent = MainObject;
                cameraTarget.SetLocalPositionAndRotation(
                    ShipInfo.CameraPosition,
                    ShipInfo.CameraRotation
                );

                BoxCollider boxCollider = MainObject.GetComponent<BoxCollider>();
                boxCollider.center = ShipInfo.BoundaryCenter;
                boxCollider.size = ShipInfo.BoundarySize;
            }
        }
    }
}