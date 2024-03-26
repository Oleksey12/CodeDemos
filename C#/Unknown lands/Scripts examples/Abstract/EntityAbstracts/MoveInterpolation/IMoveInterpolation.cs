using UnityEngine;

namespace Assets.Project.Scripts {
    public interface IMoveInterpolation {
        public Vector2 AdjustPlayerVelocity(float movementInputX, float movementInputY);
    }
}
