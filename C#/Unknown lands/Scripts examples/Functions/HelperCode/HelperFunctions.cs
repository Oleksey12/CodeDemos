using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Scripts.Functions {
    public static class HelperFunctions {
        public static Collider2D GetDamageCollider(Collider2D[] coliders) {
            for (int i = 0; i < coliders.Length; ++i) {
               Collider2D objectCollider = coliders[i];
                if (objectCollider.isTrigger == true) {
                    return objectCollider;
                }
            }
            return null;
        }
    }
}
