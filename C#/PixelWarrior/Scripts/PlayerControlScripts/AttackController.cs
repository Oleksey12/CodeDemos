using UnityEngine;
/*
 * Скрипт управляет атакой игрока
 */
public class AttackController : MonoBehaviour, IAttackController
{
    [SerializeField] private GameObject damageBox;
    [SerializeField] private Animator animator;
    private GameObject _iDamageBox;
    private IPlayerDefaultData playerData;



    private float _curAttackCooldown = 0;
    public float AttackCooldown { get => _curAttackCooldown; set => _curAttackCooldown = value; }

    private void Start()
    {
        playerData = GetComponent<IPlayerDefaultData>();
    }


    public void AttackButton() // Кнопка удара
    {
        // Если пришла команда начать атаку и игрок может аттаковать
        if (_curAttackCooldown == 0 && !animator.GetCurrentAnimatorStateInfo(0).IsName("Damage"))
            StartAttack();
    }

    private void StartAttack() 
    {
        // Проигрываем анимацию удара
        PlayAnimation();

        // Создаём хитбокс удара, наносящий урон контактирующим объектам
        _iDamageBox = Instantiate(damageBox, HitBoxPosition(), Quaternion.identity);
        _iDamageBox.transform.SetParent(gameObject.transform);

        _curAttackCooldown = playerData.AttackCooldown;
    }



    public void RestoreAttackCooldown() // Пассивная перезарядка атаки
    {
        if (_curAttackCooldown > 0)
        {
            --_curAttackCooldown;
            // После четырёх тиков удаляем хитбокс удара
            if (_curAttackCooldown < playerData.AttackCooldown-3)
            {
                if (_iDamageBox != null)
                    Destroy(_iDamageBox);
            }
        }
    }
    private Vector3 HitBoxPosition() // Рассчитывает положение хитбокса удара, взависимости от направления игрока
    {
        Vector3 playerPosition = gameObject.transform.position;
        playerPosition.x = gameObject.transform.localScale.x == -1 ? 
            playerPosition.x -= 0.05f
            : playerPosition.x += 0.05f;

        playerPosition.y -= 0.1f;
        return playerPosition;
    }
    private void PlayAnimation() // Проигрывает анимацию, взависимости от расположения игрока
    {
        animator.Play("Attack");
    }

}