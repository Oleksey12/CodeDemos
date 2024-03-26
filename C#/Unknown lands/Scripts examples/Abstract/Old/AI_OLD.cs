using Assets.Scripts.Opponent;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace Assets.Scripts {

    public class AI_OLD : State {
        Transform target;
        [SerializeField] int mapResolution;
        [SerializeField] float maxDistance;
        [SerializeField] int bestVectorsCount;

        [SerializeField] bool chase = true;

        [SerializeField] float interestChangeWeight = 0.8f;
        [SerializeField] float targetFollowWeight = 0.5f;
        [SerializeField] float ColliderOverlap = 0.6f;

        [SerializeField] float cooldown = 0.25f;

        
        List<Vector2> directions;
        List<int> preferedDirections;
        List<float> interestMap;
        Vector2 previousVector;

        Animator animator;
        SpriteRenderer spriteRenderer;
        Rigidbody2D rb;
        BoxCollider2D col;
        OpponentStatsOld opponentStats;

        float walkSpeed;
        float timeLeft = 0f;
        bool haveEntered = false;
        private void Awake() {
            GetAllComponents();
            InitializeMaps();
        }
        public override void Enter(AbstractStateController machine) {
            animator.SetBool("active", true);
            walkSpeed = opponentStats.walkSpeed;
        }


        public override void HandlePhysics() {
            // При помощи анимации определяем перешёл ли бот в стадию преследования или нет
            if (!haveEntered && animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Run" &&
                animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Run back") {
                return;
            }
            else {
                haveEntered = true;
            }
            animator.SetBool("direction", previousVector.y < 0 ? true : false);
            InterestPathLogic();
            rb.MovePosition(rb.position + previousVector * walkSpeed * Time.deltaTime);
            AnimationScripts.HandleDirection(spriteRenderer, previousVector.x);
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            GameObject entity = collision.gameObject;
            Stats opponentStatsClass = collision.gameObject.GetComponent<Stats>();
            if (entity.tag == "Player" && opponentStatsClass != null) {
                opponentStatsClass.TakeDamage(opponentStats.damage);
                /*
                Rigidbody2D enemyRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                    enemyRb.velocity = (entity.transform.localPosition -
                        gameObject.transform.localPosition) * opponentStats.attackKnockback;
                */
            }
        }
   


        private void InitializeMaps() {
            previousVector = new Vector2(1, 0);
            directions = new List<Vector2>(new Vector2[mapResolution]);
            interestMap = new List<float>(new float[mapResolution]);
            preferedDirections = new List<int>(new int[bestVectorsCount]);

            for (int i = 0; i < mapResolution; i++) {
                directions[i] = new Vector2(Mathf.Cos(2 * Mathf.PI * i / mapResolution), Mathf.Sin(2 * Mathf.PI * i / mapResolution));
            }
        }
        /*
        private void OnDrawGizmos() {
            Vector2 playerPos = gameObject.transform.position;

            Gizmos.color = UnityEngine.Color.green;

            for (int i = 0; i < mapResolution; i++) {
                Vector2 line = directions[i] * interestMap[i];
                Gizmos.DrawLine(playerPos, playerPos + line);
            }
        }
        */
        
        private void InterestPathLogic() {
            timeLeft -= Time.deltaTime;
            if (timeLeft < 0) {
                timeLeft = cooldown;
                Vector2 botPosition = gameObject.transform.localPosition;
                for (int i = 0; i < mapResolution; i++) {
                    RaycastHit2D[] hit = Physics2D.RaycastAll(botPosition, directions[i], maxDistance);
                    foreach (RaycastHit2D hit2d in hit) {             
                        if (hit2d.collider.gameObject.CompareTag("Player")) {
                            interestMap[i] = 0.85f;
                            break;
                        }
                        else if (hit2d.collider.gameObject.name == "Walls") {
                            interestMap[i] = hit2d.distance / maxDistance;
                        }
                    }

                    CorrectInterests(botPosition);

                    int index = interestMap.IndexOf(interestMap.Max());
                    previousVector = directions[index];
                }
            }
        }

        public override void Exit() {
            animator.SetBool("direction", previousVector.y < 0 ? true : false);
        }
        private void CorrectInterests(Vector2 botPosition) {
            CorrectWeights(previousVector, directions, interestMap, interestChangeWeight);

            Vector2 targetPos = target.position;
            CorrectWeights(targetPos - botPosition, directions, interestMap, targetFollowWeight);

            // Выбираем лучшие векторы для определения финального пути
            //preferedDirections = GetNBestVectors(bestVectorsCount, directions, interestMap);
            //SimulateMovement(col, preferedDirections, directions, interestMap);
        }

        private void CorrectWeights(Vector2 correctVector, List<Vector2> directions, List<float> interestMap, float weight, bool IsChasing = true) {
            // Корректирует интересы так, чтобы они не противоречили предыдущему решения бота
            for (int i = 0; i < mapResolution; i++) {
                float dotProduct = Vector2.Dot(directions[i], correctVector);
                if (!IsChasing)
                    dotProduct = Mathf.Sign(dotProduct) * (1 - Mathf.Abs(dotProduct));

                interestMap[i] *= (dotProduct * (1 - weight) + weight);
            }
        }

        private List<int> GetNBestVectors(int nbest, List<Vector2> directions, List<float> interestMap) {
            int[] indexes = new int[mapResolution];
            float[] sortedInterestMaps = interestMap.ToArray();
            for (int i = 0; i < mapResolution; ++i) {
                indexes[i] = i;
            }
            Array.Sort(sortedInterestMaps, indexes);

            return new List<int>(indexes).GetRange(0, nbest);
        }

        private void SimulateMovement(BoxCollider2D hitBox, List<int> indexes, List<Vector2> directions, List<float> interestMap) {
            List<Collider2D> overlapItems = new List<Collider2D>();
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.useTriggers = false;

            float maxDist = Mathf.Max(hitBox.size.x, hitBox.size.y);
            //float minDist = Mathf.Min(hitBox.size.x, hitBox.size.y);

            foreach (int index in indexes) {
                Vector3 simulationPoint = rb.position + directions[index] * walkSpeed * cooldown;
                Physics2D.OverlapCircle(simulationPoint, maxDist, contactFilter, overlapItems);
                foreach (var item in overlapItems) {
                    if (item.gameObject.name == "Walls") {
                        interestMap[index] *= ColliderOverlap;
                    }
                }
            }

        }

        private void GetAllComponents() {
            target = GameObject.FindWithTag("Player").transform;
            animator = gameObject.GetComponent<Animator>();
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            rb = gameObject.GetComponent<Rigidbody2D>();
            col = gameObject.GetComponent<BoxCollider2D>();
            opponentStats = gameObject.GetComponent<OpponentStatsOld>();
        }

    } 

}

