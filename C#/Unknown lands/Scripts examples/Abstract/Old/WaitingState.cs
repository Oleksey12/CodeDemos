using Assets.Project.Scripts.Abstract.EntityAbstracts.StatesAbstracts;
using Assets.Project.Scripts.Functions;
using Assets.Project.Scripts.Opponent;
using Assets.Scripts.Opponent;
using UnityEngine;
using Zenject;

namespace Assets.Scripts {
    /*
    public class WaitingState: State {
        Transform player;
        AudioData data;
        OpenentStats openentStats; 
        AbstractStateController stateController;
        State ai;
        Transform botPosition;
       
        public override void Enter(AbstractStateController machine) {
            data = FindObjectOfType<AudioData>();
            player = GameObject.FindWithTag("Player").transform;
            openentStats = gameObject.GetComponent<OpenentStats>();
            ai = gameObject.GetComponent<AI>();

            stateController = machine;
            botPosition = gameObject.transform;
        }

        public override void HandlePhysics() {
            if ((player.position - botPosition.position).magnitude < openentStats.triggerDistance) {
                stateController.ChangeState(ai);
            }
        }

        public override void Exit() {
            SoundManager.PlaySound(data.wakeUp);
        }

    }
    */
    
}
