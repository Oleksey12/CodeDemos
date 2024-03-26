using Assets.Project.Scripts.Abstract;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;



public abstract class Stats : MonoBehaviour {
    [SerializeField] public float maxHealth { get; set; } = 0;
    [SerializeField] public float currentHealth { get; set; } = 0;
    [SerializeField] public float speed { get; protected set; } = 0;

    public virtual void ResetHealth() {
        currentHealth = maxHealth;
    }

    public virtual void SetSpeed(float newSpeed) {
        speed = newSpeed;
    }

    public virtual void TakeDamage(float damage) {
        currentHealth -= damage;
    }


    protected virtual void Die() {

    }


}

