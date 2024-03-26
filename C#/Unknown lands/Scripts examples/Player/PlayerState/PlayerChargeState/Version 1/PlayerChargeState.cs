using Assets.Project.Scripts;
using Assets.Project.Scripts.Functions;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Player {
    /*
    public class ChargeState1 : State {
        [SerializeField] GameObject echo;
        [SerializeField] GameObject echoReversed;

        AudioData data;
        PlayerCooldownManager cooldowns;
        SpriteRenderer spriteRenderer;
        Animator animator;
        Collider2D objectCollider;
        PlayerDataController playerData;
        AbstractStateController stateMachine;
        Rigidbody2D rb;


        State movementState;

        float echoTimer;
        Vector2 direction;

        float timeLeft;
        float chargeSpeed;

        private void Awake() {
            GetAllComponents();
        }
        public override void Enter(AbstractStateController machine) {
            stateMachine = machine;

            timeLeft = playerData.chargeTime;
            chargeSpeed = playerData.chargeSpeed;
            echoTimer = playerData.chargeEchoCooldown;


            Vector2 mousePosition = Input.mousePosition;
            Vector2 playerScreenPos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            direction = (mousePosition - playerScreenPos).normalized;

            objectCollider = GetDamageCollider(gameObject);
            objectCollider.enabled = false;

            SoundManager.PlaySound(data.dash);

            AnimationScripts.HandleDirection(spriteRenderer, direction.x);
        }


        public override void HandleLogic() {
            // Уменьшить перезарядку другой способности
            timeLeft -= Time.deltaTime;
            cooldowns.DecreaseAttack(Time.deltaTime);

            if (timeLeft < 0) {
                stateMachine.ChangeState(movementState);
            }
            // Анимация рывка
            DashEffect();

        }
        public override void HandlePhysics() {
            rb.MovePosition(rb.position + direction * chargeSpeed * Time.deltaTime);
        }
        public override void Exit() {
            objectCollider.enabled = true;
        }


        private void GetAllComponents() {
            data = FindObjectOfType<AudioData>();
            cooldowns = gameObject.GetComponent<PlayerCooldownManager>();
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            playerData = gameObject.GetComponent<PlayerDataController>();
            rb = gameObject.GetComponent<Rigidbody2D>();
            animator = gameObject.GetComponent<Animator>();
            objectCollider = gameObject.GetComponent<Collider2D>();
            movementState = gameObject.GetComponent<MainState>();
        }

        public void DashEffect() // Управляет анимацией игрока при рывке
        {
            echoTimer -= Time.deltaTime;
            if (echoTimer <= 0) {
                echoTimer = playerData.chargeEchoCooldown;

                if (direction.y < 0) {
                    echoReversed.GetComponent<SpriteRenderer>().flipX = direction.x < 0 ? true : false;
                    SpawnEcho(echoReversed, gameObject.transform.position);
                } else {
                    echo.GetComponent<SpriteRenderer>().flipX = direction.x < 0 ? true : false;
                    SpawnEcho(echo, gameObject.transform.position);
                }
            }

        }
        private void SpawnEcho(GameObject echo, Vector3 position) // Создаёт копии игрока при рывке
        {
            GameObject echo1 = Instantiate(echo, position, Quaternion.identity);
            Destroy(echo1, 3);
        }
    }
    */


    public class PlayerChargeState : AbstractChargeState {
        [Inject] protected PlayerData _playerData;

        [SerializeField] protected GameObject echo;
        [SerializeField] protected GameObject echoReversed;

        //public AudioData _data;
        protected SpriteRenderer _spriteRenderer;
        protected Animator _animator;
        protected Collider2D _objectCollider;


        protected float _timeLeft;
        protected float _echoCooldown;
        protected float _echoTimer;

        public override void Enter(AbstractStateController machine) {
            _abilityName = _playerData.chargeSettings.chargeName.GetValue();
            _timeLeft = _playerData.chargeSettings.chargeTime.GetValue();
            _echoCooldown = _playerData.chargeSettings.chargeEchoCooldown.GetValue();

            base.Enter(machine);

            _direction = CalculateChargeDirection();
            _objectCollider.enabled = false;

            //SoundManager.PlaySound(_data.dash);
            AnimationScripts.HandleDirection(_spriteRenderer, _direction.x);
        }

        public override void HandlePhysics() {
            base.HandlePhysics();
            _timeLeft -= Time.deltaTime;
            if (_timeLeft > 0) {
                DashEffect();
                ApplyVelocity(_rb, _direction, _playerData.chargeSettings.chargeSpeed.GetValue());
            }
            else {
                ChangeState(_nextState);
            }
        }

        protected Vector2 CalculateChargeDirection() {
            Vector2 mousePosition = Input.mousePosition;
            Vector2 playerScreenPos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            return (mousePosition - playerScreenPos).normalized;
        }
        public override void Exit() {
            _objectCollider.enabled = true;
        }

        public void DashEffect() // Управляет анимацией игрока при рывке
        {
            _echoTimer -= Time.deltaTime;
            if (_echoTimer <= 0) {
                _echoTimer = _echoCooldown;
                GameObject copyToSpawn = _direction.y > 0 ? echoReversed : echo;
                AnimationScripts.HandleDirection(copyToSpawn.GetComponent<SpriteRenderer>(), _direction.x);
                SpawnEcho(copyToSpawn, gameObject.transform.position);
            }
        }

        protected void SpawnEcho(GameObject echo, Vector3 position) // Создаёт копии игрока при рывке
        {
            GameObject echo1 = Instantiate(_animator.GetBool("Direction") ? echo : echoReversed, position, Quaternion.identity);
            Destroy(echo1, 3);
        }

        protected override void GetAllComponents() {
            base.GetAllComponents();
            _objectCollider = HelperFunctions.GetDamageCollider(gameObject.GetComponents<Collider2D>());
            _nextState = GetComponent<AbstractMovementState>();
            _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            _animator = gameObject.GetComponent<Animator>();
        }

    }
}