using UnityEngine;

namespace ShipTypes {
    interface IShipType {
        GameObject PrefabModel { get; set;}
        static string ShipModel;

    }
}