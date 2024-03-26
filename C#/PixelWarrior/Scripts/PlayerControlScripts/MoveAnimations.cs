using UnityEngine;
/*
 * Скрипт управляет эффектами и анимациями при передвижении
 */
internal class MoveAnimations : MonoBehaviour, IMoveAnimations
{
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem particles;
    private Vector3 _defaultParticleRotation;
    private const string runningStateString = "IsRunning";


    private void Start() { _defaultParticleRotation = particles.shape.rotation; }



    // Изменение направления только после проигрывания всех основных анимаций
    public void SetAnimationToReverse()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            gameObject.transform.localScale = new Vector3(-1,1,1); 
    }

    public void SetAnimationToNormal()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            gameObject.transform.localScale = new Vector3(1, 1, 1);
    }


    // Анимация ходьбы
    public void PlayAnimation() => animator.SetBool(runningStateString, true);
    public void StopAnimation() => animator.SetBool(runningStateString, false);
    // Направление частиц при движении
    public void SetParticleStystemToNormal() 
    {
        var shapeModule = particles.shape;
        shapeModule.rotation = _defaultParticleRotation;
    }
    public void SetParticleStystemToReverse()
    {
        var shapeModule = particles.shape;
        shapeModule.rotation = -_defaultParticleRotation;
    }

    // Включить/ выключить частицы
    public void StartEmission() => particles.enableEmission = true;
    public void StopEmission() => particles.enableEmission = false;

}
