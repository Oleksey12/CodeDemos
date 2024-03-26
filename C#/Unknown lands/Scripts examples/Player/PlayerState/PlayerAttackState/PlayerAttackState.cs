using UnityEngine;
using Zenject;
using static GameParams;
using static UnityEngine.Rendering.DebugUI;

namespace Assets.Scripts.Player {
    /*
    public class AttackState: State {
        AudioData data;
        PlayerCooldownManager cooldowns;
        AbstractStateController stateMachine;
        SpriteRenderer spriteRenderer;
        PlayerDataController playerData;
        State movementState;
        Transform hitBox;
        Collider2D damageBox;

        GameObject spear;
        SpriteRenderer spearRenderer;

        Vector2 direction;
        bool haveEntered = false;
        float timeLeft;
        private void Awake() {
            GetAllComponents();
        }
        public override void Enter(AbstractStateController machine) {
            Vector2 mousePosition = Input.mousePosition;
            Vector2 playerScreenPos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            direction = (mousePosition - playerScreenPos).normalized;

            damageBox.enabled = true;
            hitBox.localPosition = direction.normalized * playerData.attackDamageBoxDist;
            timeLeft = playerData.attackTime;
            stateMachine = machine;


            // Вызвать анимации
            AnimationScripts.HandleDirection(spriteRenderer, direction.x);

            spearRenderer = spear.GetComponent<SpriteRenderer>();
            AnimateSpear(spearRenderer, spear);
            SoundManager.PlaySound(data.spear);
            haveEntered = true;
        }

        public override void HandleLogic() {
            if (!haveEntered)
                return;
            // Уменьшить перезарядку другой способности
            timeLeft -= Time.deltaTime;
            cooldowns.DecreaseCharge(Time.deltaTime);

        
            Vector3 spearDirection = direction.normalized * playerData.attackSpeed;
            Vector3 newPosition = spearDirection * Time.deltaTime + hitBox.localPosition;

            hitBox.localPosition = newPosition;
            DamageEnemies(damageBox);

            if (timeLeft < playerData.attackTime / 2) {
                damageBox.enabled = false;
            }
            if (timeLeft < 0) {
                stateMachine.ChangeState(movementState);
                return;
            }

        }



        public override void Exit() {
            hitBox.localPosition = direction.normalized * playerData.attackDamageBoxDist;
            damageBox.enabled = false;
            spearRenderer.enabled = false;
        }

        private void GetAllComponents() {
            data = FindObjectOfType<AudioData>();
            gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            hitBox = gameObject.transform.Find("DamageBox");
            spear = gameObject.transform.Find("Spear").gameObject;
            damageBox = hitBox.GetComponent<Collider2D>();
            cooldowns = gameObject.GetComponent<PlayerCooldownManager>();
            movementState = gameObject.GetComponent<MainState>();
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            playerData = gameObject.GetComponent<PlayerDataController>();
        }

        private void DamageEnemies(Collider2D damageBox) {
            List<Collider2D> overlapItems = new List<Collider2D>();
            ContactFilter2D filter = new ContactFilter2D();
            filter.useTriggers = true;
            damageBox.OverlapCollider(filter, overlapItems);
            foreach (Collider2D item in overlapItems) {
                var entity = item.transform.gameObject;
                Stats statsClass = entity.GetComponent<Stats>();
                if (item.isTrigger == true && entity.name != gameObject.name && statsClass != null) {
                    statsClass.TakeDamage(playerData.attackDamage);

                    Rigidbody2D enemyRb = entity.GetComponent<Rigidbody2D>();
                    if (enemyRb != null)
                        enemyRb.velocity = (entity.transform.localPosition -
                            gameObject.transform.localPosition) * playerData.attackKnockback;
                    
                }
            }
        }


        private void AnimateSpear(SpriteRenderer spriteRenderer, GameObject spear) {
            spearRenderer.enabled = true;
            if (direction.x > 0) {
                spearRenderer.flipX = false;
                spear.transform.localPosition = new Vector3(Mathf.Abs(spear.transform.localPosition.x),
                                                            spear.transform.localPosition.y, 0);
            } else {
                spear.transform.localPosition = new Vector3(-Mathf.Abs(spear.transform.localPosition.x),
                                                            spear.transform.localPosition.y, 0);

                spearRenderer.flipX = true;
            }

            spear.transform.localPosition = direction.normalized * playerData.attackSpearDist;
            float angle = Mathf.Acos(Vector2.Dot(direction.normalized, new Vector2(1, 0)));
            if (direction.normalized.y < 0) {
                angle = -angle;
            } 
            if (direction.normalized.x < 0) {
                angle = Mathf.PI + angle;
            }
            spear.transform.rotation = Quaternion.Euler(0, 0, 180 / Mathf.PI * angle);
            spearRenderer.flipX = direction.x > 0 ? false : true;
            spear.GetComponent<Animator>().Play("hit");
        }
    }
    */
    public class PlayerAttackState : AbstractShootingState {
        [Inject] protected PlayerData _data;

        protected override void Awake() {
            base.Awake();
            _cooldownName = _data.attackSettings.attackName.GetValue();
            _nextState = GetComponent<AbstractMovementState>();
        }

        public override void Enter(AbstractStateController machine) {
            base.Enter(machine);
            Vector2 direction = GetClickDirection();
            AnimationScripts.HandleDirection(_spriteRenderer, direction.x);
            _animator.SetBool("Direction", direction.y <= 0);
            _manager.Shoot(_bullet, direction * _data.attackSettings.attackSpearDist.GetValue(), direction);
        }

        protected Vector2 GetClickDirection() {
            Vector2 mousePosition = Input.mousePosition;
            Vector2 playerScreenPos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            return (mousePosition - playerScreenPos).normalized;
        }

        public override void HandleLogic() {
            base.HandleLogic();
            if (_manager.bulletsAlive == 0) {
                _machine.ChangeState(_nextState);
            }
        }
    }
}
