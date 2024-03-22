using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using static JSONUtility.Functions;

namespace WeaponsTypes {
    namespace JSONInfo {
    using VectorList3 = List<float>;

        [Serializable]
        public struct PreWeaponsList {
            public List<PreWeaponInfo> Weapons;
        }

        [Serializable]
        public struct PreWeaponInfo {
            public string Name;
            public string PrefabPath;
            public bool SpawnWithoutOffset;
            public VectorList3 BoundaryCenter;
            public VectorList3 BoundarySize;
        }

        public struct WeaponsList {
            public Dictionary<string, WeaponInfo> Weapons;

            public WeaponsList(PreWeaponsList preList) {
                #region Assertions
                Assert.AreNotEqual(preList.Weapons.Count, 0, "Weapons JSON can't have 0 weapons");
                #endregion

                Weapons = new(preList.Weapons.Count);
                WeaponsList current = this;
                preList.Weapons.ForEach((PreWeaponInfo preInfo) => 
                    current.Weapons.Add(preInfo.Name, new(ref preInfo))
                );

                #region Assertions
                Assert.IsTrue(Weapons.ContainsKey("Default"), "Weapons should contain Default weapon");
                #endregion
            }
        }

        public struct WeaponInfo {
            public string PrefabPath;
            public bool SpawnWithoutOffset;
            public Vector3 BoundaryCenter;
            public Vector3 BoundarySize;

            public WeaponInfo(ref PreWeaponInfo preInfo) {
                #region Assertions
                Assert.IsNotNull(preInfo.PrefabPath);
                Assert.IsNotNull(preInfo.BoundaryCenter);
                Assert.IsNotNull(preInfo.BoundarySize);
                Assert.IsTrue(preInfo.PrefabPath.StartsWith("Assets"));
                #endregion

                PrefabPath = preInfo.PrefabPath;
                SpawnWithoutOffset = preInfo.SpawnWithoutOffset;
                BoundaryCenter = ListToVector(preInfo.BoundaryCenter);
                BoundarySize = ListToVector(preInfo.BoundarySize);
            }
        }

        static class Functions {
            public static WeaponsList Load(TextAsset file) =>
                new(JsonUtility.FromJson<PreWeaponsList>(file.text));
        }
    }
}