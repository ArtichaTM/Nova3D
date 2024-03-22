using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace JSONUtility {
    using VectorList3 = List<float>;
    static class Functions {
        public static Vector3 ListToVector(VectorList3 list) {
            Assert.AreEqual(list.Count, 3);
            return new Vector3(
                list[0],
                list[1],
                list[2]
            );
        }
    }
}