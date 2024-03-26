using UnityEngine;

/*
 * Реализует функции управления движением игрока
 */
public class MoveController : MonoBehaviour, IMoveController
{
    [SerializeField] private Joystick joystick;
    
    private Vector2 _moveVector;
    private float _joyStickTriggerPercent = 0.25f;
    public Vector2 MoveVector { get => _moveVector; }

    private Rigidbody2D playerRigidBody;
    private IPlayerDefaultData playerData;
    private IMoveAnimations moveAnimation;


    private void Start()
    {
        HashComponents();
    }

    // Функция считывает данные с джойстика
    public void MoveVectorCount() 
    {
        // Движение по диагонали
        if (Mathf.Abs(joystick.Vertical) > _joyStickTriggerPercent && Mathf.Abs(joystick.Horizontal) > _joyStickTriggerPercent)
        {
            _moveVector = new Vector2(joystick.Horizontal,joystick.Vertical).normalized * playerData.Speed;
            return;
        }
        
        // Движение в одну сторону
        if (joystick.Horizontal > _joyStickTriggerPercent)
        {
            moveAnimation.SetAnimationToNormal();
            _moveVector.x = playerData.Speed;
        }
        else if (joystick.Horizontal < -_joyStickTriggerPercent)
        {
            moveAnimation.SetAnimationToReverse();
            _moveVector.x = -playerData.Speed;
        }
        else
            _moveVector.x = 0;

        if (joystick.Vertical > _joyStickTriggerPercent)
        {
            _moveVector.y = playerData.Speed;

        }
        else if (joystick.Vertical < - _joyStickTriggerPercent)
        {
            _moveVector.y = -playerData.Speed;
        }
        else
            _moveVector.y = 0;
    }

    // Управляет передвижением игрока
    public void PlayerMovementController() 
    {
        // Перемещаем игрока по данным джостика
        if (_moveVector != Vector2.zero)
        {
            // Управляем направлением спрайта
            if (_moveVector.x > 0)
            {
                moveAnimation.SetAnimationToNormal();
                moveAnimation.SetParticleStystemToNormal();
            }
            else
            {
                moveAnimation.SetAnimationToReverse();
                moveAnimation.SetParticleStystemToReverse();
            }

            // Проигрываем анимацию бега и отображаем эффект бега
            moveAnimation.PlayAnimation();
            moveAnimation.StartEmission();

        }
        else
        {
            moveAnimation.StopAnimation();
            moveAnimation.StopEmission();
        }

        playerRigidBody.velocity = _moveVector;
    }




    private void HashComponents()
    {
        playerData = GetComponent<IPlayerDefaultData>();
        moveAnimation = GetComponent<IMoveAnimations>();
        playerRigidBody = GetComponent<Rigidbody2D>();
    }
}
