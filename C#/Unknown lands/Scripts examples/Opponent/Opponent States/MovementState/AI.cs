using Assets.Project.Scripts.Opponent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace Assets.Scripts {

    public class AI : MonoBehaviour {
        protected enum modes {
            hybrid,
            chase,
            strafe,
            avoid,
        }

        [Header("Визуализация направления")]
        [SerializeField] protected bool debug = true;

        [Header("Параметры алгоритма карты интересов")]
        [SerializeField] protected int mapResolution;
        [SerializeField] protected float maxDistance;
        [SerializeField] protected int bestVectorsCount;
   
        [Tooltip("Выбор поведения бота")]
        [SerializeField] protected modes mode;

        [Header("Приоритет каждого из наблюдений")]
        [Tooltip("Наказание за смену направления")]
        [SerializeField] protected float interestChangeWeight;

        [Tooltip("Наказания за движение не по направлению цели")]
        [SerializeField] protected float targetFollowWeight;

        [Header("Время между обновлениями карты направлений")]
        [SerializeField] protected float cooldown = 0.1f;

        protected OpponentStats _stats;
        protected Transform _target;

        protected List<Vector2> _directions;
        protected List<float> _interestMap;
        protected Vector2 _previousVector;

        protected float _timeLeft = 0f;
        protected float _colliderSize;

        protected void Awake() {
            GetAllComponents();
        }
        protected void Start() {
            _colliderSize = GetComponent<BoxCollider2D>().size.x / 2f + 0.01f;
        }

        public void InitializeMaps() {
            _previousVector = new Vector2(1, 0);
            _directions = new List<Vector2>(new Vector2[mapResolution]);
            _interestMap = new List<float>(new float[mapResolution]);

            for (int i = 0; i < mapResolution; i++) {
                _directions[i] = new Vector2(Mathf.Cos(2 * Mathf.PI * i / mapResolution), Mathf.Sin(2 * Mathf.PI * i / mapResolution));
            }
        }

        protected void OnDrawGizmos() {
            if (!debug || _interestMap == null) return;

            float arrowSize = 0.25f;
            Vector2 playerPos = gameObject.transform.position;
            float primaryDirection = _interestMap.Max();
            

            for (int i = 0; i < mapResolution; i++) {
                //Debug.Log(offset);
                Gizmos.color = Color.green;
                Vector2 line = _directions[i] * _interestMap[i];
                if (_interestMap[i] == primaryDirection)
                    Gizmos.color = Color.red;
                Gizmos.DrawLine(playerPos, playerPos + line.normalized * arrowSize);
            }
        }

        public Vector2 InterestPathLogic() {
            _timeLeft -= Time.deltaTime;
            if (_timeLeft < 0) {
                _timeLeft = cooldown;
                Vector2 botPosition = gameObject.transform.localPosition;

                LayerMask mask = LayerMask.GetMask("Walls") | LayerMask.GetMask("Player");

                for (int i = 0; i < mapResolution; i++) {
                    _interestMap[i] = EvaluatePossibleDirections(botPosition, mask, i);
                }

                CorrectInterests(botPosition);

                int index = _interestMap.IndexOf(_interestMap.Max());
                _previousVector = _directions[index];
            }
            return _previousVector;
        }

        protected float EvaluatePossibleDirections(Vector2 botPosition, LayerMask mask, int i) {
            Vector2[] colliderVerticies = {new Vector2(1, 1),
                                           new Vector2(1, -1),
                                           new Vector2(-1, -1),
                                           new Vector2(-1, 1)};

            float directionWeight = 1f;
            foreach (var ver in colliderVerticies) {
                RaycastHit2D hit = Physics2D.Raycast(botPosition + ver * _colliderSize,
                                                     _directions[i], maxDistance, mask);
                directionWeight = Mathf.Min(directionWeight, EvaluateDirection(hit, maxDistance));
            }
            return directionWeight;
        }

        protected float EvaluateDirection(RaycastHit2D hit, float maxDistance) {
            return hit.collider != null ? hit.distance / maxDistance : 1f;
        }

        protected void CorrectInterests(Vector2 botPosition) {
            CorrectWeights(_previousVector, _directions, _interestMap, interestChangeWeight);

            Vector2 targetDirection = (Vector2)_target.position - botPosition;

            if (mode == 0) {
                CorrectWeights(targetDirection, _directions, _interestMap, targetFollowWeight);
            }
            else {
                CorrectWeights(-targetDirection, _directions, _interestMap, targetFollowWeight);
            }
        }

        protected void CorrectWeights(Vector2 correctVector, 
                                    List<Vector2> directions, 
                                    List<float> interestMap, 
                                    float weight,
                                    bool IsChasing = true) {
            // Корректирует интересы так, чтобы они не противоречили предыдущему решения бота
            for (int i = 0; i < mapResolution; i++) {
                float dotProduct = Vector2.Dot(directions[i], correctVector);
                if (!IsChasing)
                    dotProduct = Mathf.Sign(dotProduct) * (1 - Mathf.Abs(dotProduct));

                interestMap[i] *= (dotProduct * (1 - weight) + weight);
            }
        }

        protected void GetAllComponents() {
            _target = GameObject.FindWithTag("Player").transform;
            _stats = gameObject.GetComponent<OpponentStats>();
        }

        protected Vector2 ColliderOffset(Vector2 direction, float colliderSize) {
            return direction * colliderSize / Mathf.Max(Mathf.Abs(direction.y), Mathf.Abs(direction.x));
        }
    } 
    

}

