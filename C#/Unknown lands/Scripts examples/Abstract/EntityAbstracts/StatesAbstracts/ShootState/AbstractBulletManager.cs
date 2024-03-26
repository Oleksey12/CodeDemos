

using Assets.Scripts.Player;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Project.Scripts {
    public class AbstractBulletManager : MonoBehaviour, IBulletManager {
        [SerializeField] protected List<string> _damagedTags;
        [SerializeField] protected List<string> _ignoredTags;


        [Inject]
        protected AbstractBullet.Factory _factory;

        public float bulletsAlive = 0;

        public void ReduceBulletCount() {
            --bulletsAlive;
        }

        public virtual void SetBulletSettings(GameObject bullet, Vector2 direction) {
            bullet.GetComponent<AbstractBullet>().Initialize(ReduceBulletCount,
                                                             direction,
                                                             _damagedTags,
                                                             _ignoredTags,
                                                             gameObject);
        }

        public virtual void Shoot(GameObject bullet, Vector2 poistion, Vector2 direction) {
            ++bulletsAlive;
            AbstractBullet projectile = _factory.Create();
            GameObject buletObject = projectile.gameObject;
            SetUpProjectileTransform(buletObject.transform, poistion, direction);

            SetBulletSettings(buletObject, direction);
        }

        protected void SetUpProjectileTransform(Transform projectile,Vector2 poistion, Vector2 direction) {
            projectile.parent = gameObject.transform;
            projectile.localPosition = poistion;
            projectile.rotation = MyMath.VectorToQuaternion(direction);
        }
    }
}
