using Assets.Project.Scripts;
using Zenject;

namespace Assets.Scripts {
    /*
    public class PlayerDamagedState : State {
        AudioData data;
        Animator animator;
        PlayerData playerData;
        State moveState;
        AbstractStateController stateMachine;
        Collider2D objectCollider;

        float timeLeft;

        public override void Enter(AbstractStateController machine) {
            data = FindObjectOfType<AudioData>();
            playerData = gameObject.GetComponent<PlayerData>();
            moveState = gameObject.GetComponent<PlayerMovementState>();
            animator = gameObject.GetComponent<Animator>();

            stateMachine = machine;
            timeLeft = playerData.gameSettings.invincibilityTime.GetValue();
            objectCollider = GetDamageCollider(gameObject);
            objectCollider.enabled = false;

            SoundManager.PlaySound(data.spear1);
            animator.Play("damage");

        }

        public override void HandleLogic() {
            timeLeft -= Time.deltaTime;
            if (timeLeft < 0) {
                stateMachine.ChangeState(moveState);
                return;
            }
        }

        public override void Exit() {
            objectCollider.enabled = true;
        }

        private Collider2D GetDamageCollider(GameObject gameObject) {
            Collider2D[] coliders = gameObject.GetComponents<BoxCollider2D>();
            for (int i = 0; i < coliders.Length; ++i) {
                objectCollider = coliders[i];
                if (objectCollider.isTrigger == true) {
                    return objectCollider;
                }
            }
            return null;
        }
    }
    */
    public class PlayerDamagedState : AbstractDamagedState {
        [Inject] protected PlayerData _data;

        protected string frontDamageAnimation = "Damage";
        protected string backDamageAnimation = "Damage back";

        protected override void Awake() {
            base.Awake();
            _nextState = GetComponent<AbstractMovementState>();
        }

        public override void Enter(AbstractStateController machine) {
            base.Enter(machine);
            _invincibilityTime = _data.gameSettings.invincibilityTime.GetValue();
            _animator.Play(_animator.GetBool("Direction") ? frontDamageAnimation : backDamageAnimation);
        }

    }
}
