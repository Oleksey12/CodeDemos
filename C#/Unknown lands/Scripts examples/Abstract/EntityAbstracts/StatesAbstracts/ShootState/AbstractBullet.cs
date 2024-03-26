using Assets.Project.Scripts.Abstract.EntityAbstracts;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Project.Scripts {
    public class AbstractBullet : MonoBehaviour, IBullet {
        protected Rigidbody2D _rb;
        protected Collider2D _collider;

        protected float _damage;
        protected float _speed;
        protected float _liveTime;

        Action _bulletCallback;
        protected Vector2 _direction;

        protected List<string> _damagedTags;
        protected List<string> _ignoredTags;
        protected List<GameObject> _damagedObjects;

        protected virtual void Awake() {
            _damagedObjects = new List<GameObject>();
        }

        public virtual void Initialize(Action bulletCallback, 
                                       Vector2 direction,
                                       List<string> damagedTags = null,
                                       List<string> ignoredTags = null,
                                       GameObject parent = null) {
            _bulletCallback = bulletCallback;

            _direction = direction;

            _damagedTags = damagedTags;
            _ignoredTags = ignoredTags;

            _damagedObjects.Add(parent);
        }



        public virtual void CheckCollisions(Collider2D collider, List<GameObject> damagedObjects, float damage) {
            List<Collider2D> overlapItems = new List<Collider2D>();
            ContactFilter2D filter = new ContactFilter2D();
            filter.useTriggers = true;
            collider.OverlapCollider(filter, overlapItems);
            foreach (Collider2D item in overlapItems) {
                var entity = item.transform.gameObject;
                IDamagable damageClass = entity.GetComponent<IDamagable>();
                if (damageClass != null && IsNotDamaged(entity, _damagedObjects) && CheckTag(_damagedTags, _ignoredTags, entity.tag)) {
                    damageClass.ApplyDamage(damage);
                    damagedObjects.Add(entity);
                }
            }
        }
        protected virtual bool IsNotDamaged(GameObject entity, List<GameObject> damagedObjects) {
            return !damagedObjects.Contains(entity);
        }
        protected virtual bool CheckTag(List<string> damagedTags, List<string> ignoredTags, string tag) {
            return (damagedTags == null || damagedTags.Contains(tag)) && 
                (ignoredTags == null || !ignoredTags.Contains(tag));
        }
        public virtual void Disappear(GameObject thisObject) {
            _bulletCallback();
            thisObject.SetActive(false);
            Destroy(thisObject, 3f);
        }
        
        public virtual void Move(Rigidbody2D rb, Vector2 direction, float speed) {
            rb.MovePosition(rb.position + direction * speed * Time.deltaTime);
        }

        public class Factory : PlaceholderFactory<AbstractBullet> {

        }
    }

}
