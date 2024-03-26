using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Project.Scripts {
    public interface IBullet {
        public void Initialize(Action bulletCallback, 
                               Vector2 direction,
                               List<string> damagedTags = null,
                               List<string> ignoredTags = null,
                               GameObject parent = null);

        public void Move(Rigidbody2D rb, Vector2 direction, float speed);
        public void CheckCollisions(Collider2D collider, List<GameObject> damagedObjects, float damage);
        public void Disappear(GameObject thisObject);

    }
}
