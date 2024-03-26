using UnityEngine;


namespace Assets.Scripts {
    public class NPCStats : Stats {
        NPCMainController controller;

        public float activateRadius { get; protected set; }
        public float dialogRadius { get; protected set; }
        private void Start() {
            controller = gameObject.GetComponent<NPCMainController>();
            activateRadius = 1.5f;
            dialogRadius = 0.4f;
        }

    }
}
