using Assets.Scripts.Player;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Project.Scripts {
    public abstract class AbstractMovePredictor : MonoBehaviour {
        [Header("НЕ МЕНЯТЬ ПОСЛЕ ЗАПУСКА!")]
        [SerializeField] protected int ticksToMaxSpeed = 4;
        [Tooltip("Минимальная сила нажатия кнопки для начала бега")]
        [SerializeField] protected float minTriggerPower = 0.2f;
        [Tooltip("Минимальная сила нажатия кнопки остановки персонажа")]
        [SerializeField] protected float minStopPower = 0.7f;
        [Tooltip("Размер массива из силы нажатия кнопки, по которому осуществляется анализ состояния игрока. " +
            "Чем больше, тем хуже игрок реагирует на медленнее кнопок")]
        [SerializeField] protected int queSize = 3;

        // Буффер, хранящий силу нажатия клавиш
        protected List<float> _pressPowerX;
        protected List<float> _pressPowerY;

        protected int _ticksX = 0;
        protected int _ticksY = 0;
        protected bool _isRunningX = false;
        protected bool _isRunningY = false;

        public abstract Vector2 CalculateMoveDirection();

        public virtual ValueTuple<bool, bool> GetRunningState() {
            return new ValueTuple<bool, bool>(_isRunningX, _isRunningY);
        }

        public void HandlePhysics(Vector2 inputVector, bool isStopping) {
            _isRunningX = PredictBehaviour(_pressPowerX, _isRunningX);
            _isRunningY = PredictBehaviour(_pressPowerY, _isRunningY);

            _ticksX = UpdateSpeed(_isRunningX && !isStopping, _ticksX, Math.Sign(inputVector.x));
            _ticksY = UpdateSpeed(_isRunningY && !isStopping, _ticksY, Math.Sign(inputVector.y));
        }

        public void HandleLogic(Vector2 inputVector) {
            MemoryAdd(_pressPowerX, queSize, inputVector.x);
            MemoryAdd(_pressPowerY, queSize, inputVector.y);
        }


        protected virtual void Awake() {
            _pressPowerX = new List<float>();
            _pressPowerY = new List<float>();
        }

        protected bool CheckPattern(List<float> que, float startVal, float stopVal) {
            bool isIncreasing = true;

            for (int i = 1; i < que.Count; ++i) {
                isIncreasing &= Mathf.Abs(que[i - 1]) >= Mathf.Abs(que[i]);
            }

            if (isIncreasing && Mathf.Abs(que[0]) > startVal) {
                return true;
            } else if (Mathf.Abs(que[0]) < stopVal) {
                return false;
            } else {
                return true;
            }
        }

        protected void MemoryAdd<T>(List<T> que, int max_size, T value) {
            if (que.Count == max_size) {
                que.RemoveAt(max_size - 1);
            }
            que.Insert(0, value);
        }

        protected virtual bool PredictBehaviour(List<float> que, bool currentState) {
            bool val = currentState;

            if (que.Count == queSize) {
                val = CheckPattern(que, minTriggerPower, minStopPower);
            }

            return val;
        }

        protected int UpdateSpeed(bool isRunning, int ticks, int direction) {
            if (isRunning) {
                ticks += Mathf.Abs(ticks + direction) < ticksToMaxSpeed ? direction : 0;
            } else { // Устремляем скорость к 0
                ticks -= ticks != 0 ? (int)Mathf.Sign(ticks) : 0;
            }
            return ticks;
        }
    }
}