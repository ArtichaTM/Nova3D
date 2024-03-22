using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using static JSONUtility.Functions;

namespace ShipTypes {
    namespace JSONInfo {
        using VectorList3 = List<float>;

        [Serializable]
        public struct PreShipsList {
            public List<PreShipInfo> Ships;
        }

        [Serializable]
        public struct PreShipInfo {
            public string Name;
            public string PrefabPath;
            public VectorList3 NosePosition;
            public VectorList3 BoundaryCenter;
            public VectorList3 BoundarySize;
            public VectorList3 CameraPosition;
            public VectorList3 CameraRotation;
        }

        public struct ShipsList {
            public Dictionary<string, ShipInfo> Ships;

            public ShipsList(PreShipsList preList) {
                #region Assertions
                Assert.AreNotEqual(preList.Ships.Count, 0, "Ships JSON can't have 0 ships");
                #endregion

                Ships = new(preList.Ships.Count);
                ShipsList current = this;
                preList.Ships.ForEach((PreShipInfo preInfo) => 
                    current.Ships.Add(preInfo.Name, new(ref preInfo))
                );

                #region Assertions
                Assert.IsTrue(Ships.ContainsKey("Default"), "Ships should contain Default Ship");
                #endregion
            }
        }

        public struct ShipInfo {
            public string PrefabPath;
            public Vector3 NosePosition;
            public Vector3 BoundaryCenter;
            public Vector3 BoundarySize;
            public Vector3 CameraPosition;
            public Vector3 CameraRotation;

            public ShipInfo(ref PreShipInfo preInfo) {
                #region Assertions
                Assert.IsNotNull(preInfo.PrefabPath);
                Assert.IsNotNull(preInfo.NosePosition);
                Assert.IsNotNull(preInfo.BoundaryCenter);
                Assert.IsNotNull(preInfo.BoundarySize);
                Assert.IsNotNull(preInfo.CameraPosition);
                Assert.IsNotNull(preInfo.CameraRotation);
                Assert.IsTrue(preInfo.PrefabPath.StartsWith("Assets"));
                #endregion

                PrefabPath = preInfo.PrefabPath;
                NosePosition = ListToVector(preInfo.NosePosition);
                BoundaryCenter = ListToVector(preInfo.BoundaryCenter);
                BoundarySize = ListToVector(preInfo.BoundarySize);
                CameraPosition = ListToVector(preInfo.CameraPosition);
                CameraRotation = ListToVector(preInfo.CameraRotation);
            }
        }

        static class Functions {
            public static ShipsList Load(TextAsset file) =>
                new(JsonUtility.FromJson<PreShipsList>(file.text));
        }
    }
}