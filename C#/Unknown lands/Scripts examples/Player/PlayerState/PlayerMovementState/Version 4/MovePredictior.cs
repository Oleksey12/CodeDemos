using Assets.Project.Scripts.Abstract.EntityAbstracts.StatesAbstracts;
using Assets.Scripts.Player;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Project.Scripts {
    public class MovePredictior : AbstractMovePredictor {
        protected Vector3 _interpolationKoefs;

        protected override void Awake() {
            base.Awake();
            _interpolationKoefs = MyMath.CalculateKoefficients(0, ticksToMaxSpeed);
        }
        
        public override Vector2 CalculateMoveDirection() {
            return LinearCountVelocity(new Vector2(_ticksX, _ticksY));
        }

        protected Vector2 ParabolaCountVelocity(Vector2 speedInTicks) {
            float horizontalSpeed = MyMath.ParabolaFunc(
               Mathf.Abs(speedInTicks.x),
               new Vector2(0, ticksToMaxSpeed - 1),
               _interpolationKoefs) * Mathf.Sign(speedInTicks.x);

            float verticalSpeed = MyMath.ParabolaFunc(
                Mathf.Abs(speedInTicks.y),
               new Vector2(0, ticksToMaxSpeed - 1),
               _interpolationKoefs) * Mathf.Sign(speedInTicks.y);
            return new Vector2(horizontalSpeed, verticalSpeed).normalized;
        }

        protected Vector2 LinearCountVelocity(Vector2 speedInTicks) {
            return new Vector2(speedInTicks.x, speedInTicks.y).normalized;
        }
    }
}