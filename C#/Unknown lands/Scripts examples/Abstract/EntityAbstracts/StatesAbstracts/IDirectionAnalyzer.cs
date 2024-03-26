using UnityEngine;
namespace Assets.Project.Scripts.Abstract.EntityAbstracts.StatesAbstracts {

    public interface IDirectionAnalyzer {
        public Vector2 CalculateMoveDirection(Vector2 inputVector);
    }
}
