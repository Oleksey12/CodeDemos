using Assets.Project.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace Assets.Scripts {
    public class PlayerCooldownManager: AbstractCooldownManager {
        [Inject] protected PlayerData _playerData;
   
        protected override void InitializeCooldowns() {
            base.InitializeCooldowns();
            // Can't update cooldown time by this implementation
            float cooldown1 = _playerData.chargeSettings.chargeCooldown.GetValue();
            float cooldown2 = _playerData.attackSettings.attackCooldown.GetValue();

            string ability1 = _playerData.chargeSettings.chargeName.GetValue();
            string ability2 = _playerData.attackSettings.attackName.GetValue();

            _cooldownTime.Add(ability1, cooldown1);
            _cooldownTime.Add(ability2, cooldown2);

            _timeLeft.Add(ability1, 0);
            _timeLeft.Add(ability2, 0);
        }

        protected void FixedUpdate() {
            ReduceAllCooldowns(Time.deltaTime);
        }
    }
}
