using UnityEngine;

namespace WeaponsTypes {

    /*
    This interfaces need some explanatation.
    So, to start with:
    1. Every weapon can be used by AI ships
    2. NOT every weapon can be used by player
    So, Ship.cs using IPlayerWeapon, EnemyShip.cs using IGenericWeapon

    Notes:
    IPlayerWeapon classes should check if ship variable present
    */ 

    abstract public class WeaponType : MonoBehaviour {
        protected ShipParameters Parameters { get; set; }
        protected GameObject PrefabModel { get; set;}
        protected JSONInfo.WeaponInfo WeaponInfo => MainLogic.Instance.Assets.WeaponsInfo.Weapons[GetType().Name];

        abstract public void FireWeapon();
        public void ReloadCompleted() {}
        public void WeaponChanged(WeaponType to) {}
    }

    namespace Players {
        abstract public class PlayerWeapon : WeaponType {
            protected Ship PlayerShip { get; set; }
        }
    }
}