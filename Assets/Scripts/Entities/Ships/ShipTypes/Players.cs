using System.Threading;
using UnityEngine;

namespace ShipTypes {
    namespace Players {
        public class Default : PlayerShip {
            public override JSONInfo.ShipInfo ShipInfo {
                get => MainLogic.Instance.Assets.ShipsInfo.Ships[GetType().Name];
            }

            public Default(Vector3 position, Quaternion rotation) : base(position, rotation) {}
        }
    }
}