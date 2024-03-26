using Assets.Project.Scripts.Abstract.EntityAbstracts;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Project.Scripts.Opponent {
    public class OpponentStats : MonoBehaviour {
        protected enum modes {
            hardcoded,
            scriptableObject,
            none
        }

        [Inject] protected OpponentStatsNames _paramNames;

        [SerializeField]
        [Tooltip("Лучше не трогать, если не знаешь")] protected modes mode;

        [SerializeField] protected float maxHealth = 55f;
        [SerializeField] protected float currentHealth = 55f;
        [SerializeField] protected float chargeCooldown = 10f;
        [SerializeField] protected float chargeSpeed = 2.5f;
        [SerializeField] protected float chargeDuration = 2f;
        [SerializeField] protected float chargeActivationDistance = 1.5f;
        [SerializeField] protected string chargeName = "charge";
        [SerializeField] protected float walkSpeed = 1.5f;
        [SerializeField] protected float invincibilityTime = 0.2f;
        [SerializeField] protected float damage = 1f;
        [SerializeField] protected float attackKnockback = 0.3f;

        protected Dictionary<string, object> _paramsDict;


        protected void Awake() {
            _paramsDict = new Dictionary<string, object>();
            InitializeBaseValues();
        }

        public void SetValue<T>(string name, T val) {
            _paramsDict[name] = val;
        }

        public void IncreaseFloatValue(string name, float value) {
            SetValue(name, GetValue<float>(name) + value);
        }

        public List<string> GetAllParamsNames() {
            return new List<string>(_paramsDict.Keys);
        }
        public string GetAllParams() {
            return "test";
        }
        public T GetValue<T>(string name, object alt = null) {
            return _paramsDict.ContainsKey(name) ? (T)_paramsDict[name] : (T)alt;
        }

        protected void InitializeBaseValues() {
            SetValue(_paramNames.paramWalkSpeed, walkSpeed);
            SetValue(_paramNames.paramMaxHealth, maxHealth);
            SetValue(_paramNames.paramCurrentHealth, currentHealth);
            SetValue(_paramNames.paramInvincibilityTime, invincibilityTime);
            SetValue(_paramNames.paramDamage, damage);
            SetValue(_paramNames.paramAttackKnockback, attackKnockback);

            SetValue(_paramNames.paramChargeCooldown, chargeCooldown);
            SetValue(_paramNames.paramChargeName, chargeName);
            SetValue(_paramNames.paramChargeSpeed, chargeSpeed);
            SetValue(_paramNames.paramChargeDuration, chargeDuration);
            SetValue(_paramNames.paramChargeActivationDistance, chargeActivationDistance);
        }

    }
}
