using Assets.Project.Scripts;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts {
    public sealed class ParabolaInterpolation : MonoBehaviour, IMoveInterpolation {


        [SerializeField] float _lowPressStrength = 0.25f;
        [SerializeField] float _normalPressStrength = 0.35f;
         Vector3 _functionKoefs;

        private void Awake() {
            _functionKoefs = MyMath.CalculateKoefficients(_lowPressStrength, _normalPressStrength);
        }

        public Vector2 AdjustPlayerVelocity(float movementInputX, float movementInputY) {
            float horizontalSpeed = MyMath.ParabolaFunc(
                movementInputX,
                new Vector2(_lowPressStrength, _normalPressStrength),
                _functionKoefs);

            float verticalSpeed = MyMath.ParabolaFunc
                (movementInputY,
                new Vector2(_lowPressStrength, _normalPressStrength),
                _functionKoefs);


            horizontalSpeed = Mathf.Sign(movementInputX) * horizontalSpeed;
            verticalSpeed = Mathf.Sign(movementInputY) * verticalSpeed;

            return new Vector2(horizontalSpeed, verticalSpeed).normalized;
        }
    }
}