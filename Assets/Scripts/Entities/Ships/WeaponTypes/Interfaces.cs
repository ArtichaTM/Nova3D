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

    public interface IWeaponType {
        ShipParameters Parameters { get; set; }
        GameObject PrefabModel { get; set;}

        static string PrefabModelPath;

        abstract void Start();
        abstract void FireWeapon();

        void ReloadCompleted() {}
        void WeaponChanged(IWeaponType to) {}
    }

    namespace Players {
        public interface IPlayerWeapon : IWeaponType {
            Ship PlayerShip { get; set; }
        }
    }
}