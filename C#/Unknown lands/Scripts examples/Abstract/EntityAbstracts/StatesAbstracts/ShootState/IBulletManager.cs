
using UnityEngine;

namespace Assets.Project.Scripts {
    public interface IBulletManager {

        public void ReduceBulletCount();

        public void SetBulletSettings(GameObject bullet, Vector2 direction);
        public void Shoot(GameObject bullet, Vector2 poistion, Vector2 direction);
    }
}
