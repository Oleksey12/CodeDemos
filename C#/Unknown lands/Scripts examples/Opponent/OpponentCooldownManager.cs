using Assets.Project.Scripts;
using Assets.Project.Scripts.Opponent;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace Assets.Scripts {
    public class OpponentCooldownManager: AbstractCooldownManager {
        [Inject] protected OpponentStatsNames _paramNames;
        protected OpponentStats _stats;

        protected virtual void Awake() {
            _stats = GetComponent<OpponentStats>();
        }

        protected override void InitializeCooldowns() {
            base.InitializeCooldowns();
            // Can't update cooldown time by this implementation
            float cooldown1 = _stats.GetValue<float>(_paramNames.paramChargeCooldown);
            string ability1 = _stats.GetValue<string>(_paramNames.paramChargeName);

            _cooldownTime.Add(ability1, cooldown1);
            _timeLeft.Add(ability1, cooldown1);
        }

        protected void FixedUpdate() {
            ReduceAllCooldowns(Time.deltaTime);
        }
    }
}
