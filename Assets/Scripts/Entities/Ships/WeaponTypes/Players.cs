using System.Threading;
using UnityEngine;
using R3;

namespace WeaponsTypes {
    namespace Players {
        class Simple : PlayerWeapon
        {
            DisposableBag Disposables;
            public override void FireWeapon() {}

            public void Start()
            {
                new Thread(() => {
                    PrefabModel = Resources.Load<GameObject>(WeaponInfo.PrefabPath);
                }).Start();
            }

            public void OnDestroy() {
                Disposables.Dispose();
            }
        }
    }
}
