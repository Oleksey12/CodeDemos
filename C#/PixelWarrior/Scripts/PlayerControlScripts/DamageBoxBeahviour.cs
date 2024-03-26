using UnityEngine;

/*
 * Cкрипт управляет поведением хитбокса удара
 */
public class DamageBoxBeahviour : MonoBehaviour , IDamageBoxBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Наносим урон только 1 раз
        if(collision.isTrigger)
            AffectTarget(collision);
       
    }



    public void AffectTarget(Collider2D target)
    {
        if (target.CompareTag("Slime"))
        {
            target.GetComponent<IReactiveTarget>().React(gameObject.transform.position);
        }
    }
} 
    

