using Assets.Project.Scripts.Abstract.EntityAbstracts.StatesAbstracts;
using Assets.Scripts.Player;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Project.Scripts {
    public class OldMovePredictor : MonoBehaviour, IDirectionAnalyzer {
        [Header("НЕ МЕНЯТЬ ПОСЛЕ ЗАПУСКА!")]
        [SerializeField] protected int ticksToMaxSpeed = 3;
        [Tooltip("Минимальная сила нажатия кнопки для начала бега")]
        [SerializeField] protected float minTriggerPower = 0.2f;
        [Tooltip("Минимальная сила нажатия кнопки остановки персонажа")]
        [SerializeField] protected float minStopPower = 0.75f;
        [Tooltip("Размер массива из силы нажатия кнопки, по которому осуществляется анализ состояния игрока. " +
            "Чем больше, тем хуже игрок реагирует на медленнее кнопок")]
        [SerializeField] protected int que_max_size = 5;
        [Tooltip("Количество тиков, после которых начнётся проигрывание анимации остановки")]
        [SerializeField] protected int ticksBeforeStopRunning = 5;
        [Tooltip("Количество тиков, после которых начнётся проигрывание анимации бега")]
        [SerializeField] protected int ticksBeforeStartRunning = 2;

        protected List<float> _queX;
        protected List<float> _queY;
        protected List<bool> _runningLog;
        protected Vector3 _interpolationKoefs;

        protected int _ticksX = 0;
        protected int _ticksY = 0;
        protected bool _isRunningX = false;
        protected bool _isRunningY = false;


        protected virtual void Awake() {
            _queX = new List<float>();
            _queY = new List<float>();
            _runningLog = new List<bool>();
            _interpolationKoefs = MyMath.CalculateKoefficients(0, ticksToMaxSpeed);
        }

        public Vector2 CalculateMoveDirection(Vector2 inputVector) {
            return LinearCountVelocity(new Vector2(_ticksX, _ticksY));
        }

        public Vector2 GetMoveCriterias() {
            return new Vector2(ticksBeforeStartRunning, ticksBeforeStopRunning);
        }
        public List<bool> GetRunningLog() {
            return _runningLog;
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

        protected void MemoryAdd<T>(List<T> que, int max_size, T value) {
            if (que.Count == max_size) {
                que.RemoveAt(max_size - 1);
            }
            que.Insert(0, value);
        }

        protected virtual bool PredictBehaviour(List<float> que, bool currentState) {
            bool val = currentState;

            if (que.Count == que_max_size) {
                val = CheckPattern(que, minTriggerPower, minStopPower);
            }

            return val;
        }

        public void UpdateInputs(Vector2 inputVector) {
            MemoryAdd(_queX, que_max_size, inputVector.x);
            MemoryAdd(_queY, que_max_size, inputVector.y);

            _isRunningX = PredictBehaviour(_queX, _isRunningX);
            _isRunningY = PredictBehaviour(_queY, _isRunningY);

            MemoryAdd(_runningLog, ticksBeforeStopRunning, _isRunningX || _isRunningY);
        }

        public void FixedUpdateInputs(Vector2 inputVector, bool isStopping) {
            _ticksX = UpdateSpeed(_isRunningX && !isStopping, _ticksX, Math.Sign(inputVector.x));
            _ticksY = UpdateSpeed(_isRunningY && !isStopping, _ticksY, Math.Sign(inputVector.y));
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