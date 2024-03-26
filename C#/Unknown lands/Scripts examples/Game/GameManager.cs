using Assets.Project.Scripts.Abstract.EntityAbstracts;
using UnityEngine;


namespace Assets.Project.Scripts.Game {
    public class GameManager : MonoBehaviour, IPauseManager {
        protected IPausable[] _dynamicObjects;

        protected bool _isPaused = false;

        protected void Awake() {
            _dynamicObjects = GetComponents<IPausable>();
        }

        public void PauseDynamicObjects() {
            foreach (var obj in _dynamicObjects) {
                obj.Pause();
            }
        }
        public bool IsPaused() => _isPaused;
        public void Pause() {
            _isPaused = true;
            PauseDynamicObjects();
        }
        public void Unpause() {
            _isPaused = false;
        }
    }
}
