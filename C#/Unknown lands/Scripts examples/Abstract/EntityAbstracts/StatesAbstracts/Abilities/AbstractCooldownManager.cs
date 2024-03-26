using System.Collections.Generic;
using UnityEngine;

namespace Assets.Project.Scripts {
    public abstract class AbstractCooldownManager : MonoBehaviour, ICooldownManager {
        protected Dictionary<string, float> _cooldownTime;
        protected Dictionary<string, float> _timeLeft;

        protected virtual void Start() { InitializeCooldowns(); }

        protected virtual void InitializeCooldowns() {
            _cooldownTime = new Dictionary<string, float>();
            _timeLeft = new Dictionary<string, float>();
        }

        public virtual void ReduceAllCooldowns(float value) {
            List<string> keys = new List<string>(_timeLeft.Keys);
            foreach (string key in keys) {
                _timeLeft[key] = _timeLeft[key] - value >= 0 ? _timeLeft[key] - value : 0;
            }
        }

        public virtual void ReduceCooldown(string cooldownName, float value) {
            ValidateCooldown(cooldownName);
            _timeLeft[cooldownName] = _timeLeft[cooldownName] - value >= 0 ? _timeLeft[cooldownName] - value : 0;
        }

        public virtual void ResetAllCooldowns() {
            List<string> keys = new List<string>(_timeLeft.Keys);
            foreach (string key in keys) {
                _timeLeft[key] = _cooldownTime[key];
            }
        }

        public virtual void ResetCooldown(string cooldownName) {
            ValidateCooldown(cooldownName);
            _timeLeft[cooldownName] = _cooldownTime[cooldownName];
        }
        
        public virtual bool IsCooldownReady(string cooldownName) {
            ValidateCooldown(cooldownName);
            return _timeLeft[cooldownName] == 0;
        }

        protected void ValidateCooldown(string cooldownName) {
            if (!_cooldownTime.ContainsKey(cooldownName)) {
                throw new System.Exception("Wrong cooldown");
            }
        }
    }
}
