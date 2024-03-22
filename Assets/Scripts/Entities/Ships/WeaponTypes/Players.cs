using System.Threading;
using UnityEngine;

namespace WeaponsTypes {
    namespace Players {
        class Simple : IPlayerWeapon
        {
            public Ship PlayerShip { get; set; }
            public ShipParameters Parameters { get; set; }
            public GameObject PrefabModel { get; set; }

            static readonly string ModelPrefabPath = "Assets/Models/WeaponProjectiles/Simple.blend";

            public void FireWeapon()
            {
                
            }

            public void Start()
            {
                new Thread(() => {
                    PrefabModel = Resources.Load<GameObject>(ModelPrefabPath);
                }).Start();
            }
        }
    }
}
