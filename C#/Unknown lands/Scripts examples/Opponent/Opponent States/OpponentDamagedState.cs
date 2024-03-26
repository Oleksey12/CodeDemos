using Assets.Project.Scripts;
using Assets.Project.Scripts.Opponent;
using Zenject;

namespace Assets.Scripts {
    /*
    public class OpponentDamagedState: State {
        AudioData data;
        Animator animator;
        OpponentStatsOld opponentData;
        State waitState;
        AbstractStateController stateMachine;
        Collider2D objectCollider;

        float timeLeft;
        bool entered = false;
        private void Awake() {
            GetAllComponents();
        }
        public override void Enter(AbstractStateController machine) {
            stateMachine = machine;
            timeLeft = opponentData.invincibilityTime;
            objectCollider = GetDamageCollider(gameObject);
            objectCollider.enabled = false;

            animator.Play("Damage");
            SoundManager.PlaySound(data.damage);
            entered = true;
        }

        private void GetAllComponents() {
            data = FindObjectOfType<AudioData>();
            animator = GetComponent<Animator>();
            opponentData = gameObject.GetComponent<OpponentStatsOld>();
            //waitState = gameObject.GetComponent<WaitingState>();
        }

        public override void HandleLogic() {
            if (!entered)
                return;

            timeLeft -= Time.deltaTime;
            if (timeLeft < 0) {
                stateMachine.ChangeState(waitState);
                return;
            }
        }

        public override void Exit() {
            objectCollider.enabled = true;
        }

        private Collider2D GetDamageCollider(GameObject gameObject) {
            Collider2D[] coliders = gameObject.GetComponents<BoxCollider2D>();
            for (int i = 0; i < coliders.Length; ++i) {
                if (coliders[i].isTrigger == true) {
                    return coliders[i];
                }
            }
            return null;
        }
    }
    */
    public class OpponentDamagedState : AbstractDamagedState {
        [Inject] protected OpponentStatsNames _paramNames;

        protected OpponentStats _stats;

        protected string frontAnimationName = "Damage";
        protected string backAnimationName = "Damage back";

        protected override void Awake() {
            base.Awake();
            _nextState = GetComponent<AbstractMovementState>();
            _stats = GetComponent<OpponentStats>();
        }

        public override void EnterState(AbstractStateController machine) {
            base.EnterState(machine);
            _invincibilityTime = _stats.GetValue<float>(_paramNames.paramInvincibilityTime);
        }

        public override void Enter(AbstractStateController machine) {
            _machine = machine;
            _objectCollider.isTrigger = false;
            _animator.Play(_animator.GetBool("direction") ? backAnimationName : frontAnimationName);
        }
    }
}
