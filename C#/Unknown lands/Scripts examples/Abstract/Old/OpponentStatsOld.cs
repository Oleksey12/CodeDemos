using UnityEngine;


namespace Assets.Scripts.Opponent {
    
    public class OpponentStatsOld : Stats {

        OpponentMainController controller;
        public float walkSpeed { get; protected set; }
        public float invincibilityTime { get; protected set; }
        public float triggerDistance { get; protected set; }
        public float damage { get; protected set; }
        public float attackKnockback { get; protected set; }
        private void Awake() {
            controller = gameObject.GetComponent<OpponentMainController>();
            walkSpeed = 0.9f;
            currentHealth = 55f;
            invincibilityTime = 0.2f;
            triggerDistance = 0.75f;
            damage = 15f;
            attackKnockback = 0.3f;
        }
        /*
        public override void TakeDamage(float damage) {
            base.TakeDamage(damage);
            controller.ReactOnDamage();
            if(currentHealth <= 0) {
                Die();
                currentHealth = 0;
            }
        }
        protected override void Die() {
            // Проигрываем анимацию и переходим в состояние смерти
            controller.Die();
        }
        */
    }
}
