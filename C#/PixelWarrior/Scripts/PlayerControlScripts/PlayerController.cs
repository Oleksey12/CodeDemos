using UnityEngine;

/*
* Главный скрипт, связывает логику управления персонажем
*/
public class PlayerController : MonoBehaviour, IAttackButton, IDashButton
{

    // Компонент регулирующий перемещение игрока
    private IMoveController moveController;
    // Компонент управляющий способностью "атака"
    private IAttackController attackController;
    // Компонент управляющий способностью "рывок"
    private IDashController dashController;
    // Компонент управляющий эффектом отдачи при получении урона
    private IKnockbackController knockbackEffect;


    // Для сохранения данных о последнем перемещении игрока
    private Vector2 moveVector;


    private void Start()
    {
        HashComponents();
    }



    // Обрабатывает нажатие способностей игрока
    public void OnAttackButtonClick() => attackController.AttackButton();

    public void OnDashButtonClick() => dashController.DashButton(moveVector);


    private void FixedUpdate() // Обновление состояния персонажа
    {
        // Перезарядка способностей
        attackController.RestoreAttackCooldown();
        dashController.RestoreDashCooldown();


        // Сохраняем направление текущего перемещения игрока
        moveVector = moveController.MoveVector;

        // Обрабатываем перемещение игрока, если он не находится в рывке или не получил урон от бота
        if (!dashController.DashState && !knockbackEffect.isDamaged)
            moveController.PlayerMovementController();

        // Обрабатываем эффект создания клонов при рывке
        dashController.DashEffect();

        // Обрабатываем изменение скорости при отдаче
        if(knockbackEffect.isDamaged)
            knockbackEffect.SlowDownEffect();
    }

    private void Update() => moveController.MoveVectorCount();  // Непрерывно обрабатываем ввод с джойстика



    private void HashComponents()
    {
        moveController = GetComponent<IMoveController>();
        attackController = GetComponent<IAttackController>();
        dashController = GetComponent<IDashController>();
        knockbackEffect = GetComponent<IKnockbackController>();
    }
}
