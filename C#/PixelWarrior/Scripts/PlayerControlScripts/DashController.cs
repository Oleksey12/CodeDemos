using UnityEngine;
/*
 * Способность игрока - рывок
 */
public class DashController : MonoBehaviour, IDashController
{
    [SerializeField] private GameObject echo;
    [SerializeField] private GameObject echoReversed;
    [SerializeField] private Animator animator;

    private IPlayerDefaultData playerData;
    IVelocityController velocityController;
    private Rigidbody2D playerRigidBody;
    private float _curDashCooldown = 0;
    private float _curEchoCooldown = 0;
    private float _TimeBetweenEchoes = 3;
    private bool _isDashing = false;

    public float DashCooldown { get => _curDashCooldown; set => _curDashCooldown = value; }

    public bool DashState { get => _isDashing; }
    
    
    private void Start()
    {
        playerData = GetComponent<IPlayerDefaultData>();
        playerRigidBody = GetComponent<Rigidbody2D>();
        velocityController = GetComponent<IVelocityController>();    
    }
    public void DashButton(Vector3 moveVector) // Запуск рывка
    {
        if (_curDashCooldown <= 0)
        {
            StartDash(moveVector);
        }
    }
    private void StartDash(Vector3 _moveVector)
    {
        /* При использовании рывка, игрок с огромной скоростью
        * устремляется либо по направлению движения, либо
        * в сторону, которую он смотрит.
        */
        _curDashCooldown = playerData.DashCooldown;
        // Начинаем проигрывать анимацию рывка
        _isDashing = true;

        if (_moveVector == Vector3.zero)
        {
            if (gameObject.transform.localScale.x == -1)
            {
                playerRigidBody.velocity = new Vector3(-playerData.DashPower, 0f, 0f);
            }
            else
                playerRigidBody.velocity = new Vector3(playerData.DashPower, 0f, 0f);
        }
        else
            playerRigidBody.velocity = _moveVector.normalized * playerData.DashPower;
    }


    public void RestoreDashCooldown() // Перезарядка способности
    {
        if (_curDashCooldown > 0)
            --_curDashCooldown;
        if (_curEchoCooldown > 0)
            --_curEchoCooldown;
    }
    public void DashEffect() // Управляет анимацией игрока при рывке
    {
        velocityController.VecloityChange(playerData.VelocityReduceAmount);
        if (playerRigidBody.velocity == Vector2.zero)
        {
            _isDashing = false;
            return;
        }
        if (_curEchoCooldown <= 0 && _isDashing == true)
        {
            _curEchoCooldown = _TimeBetweenEchoes;
            if (playerRigidBody.velocity.x < 0)
                SpawnEcho(echoReversed, gameObject.transform.position);
            else
                SpawnEcho(echo, gameObject.transform.position);
        }
        
    }
    private void SpawnEcho(GameObject echo, Vector3 position) // Создаёт копии игрока при рывке
    {
        GameObject echo1 = Instantiate(echo, position, Quaternion.identity);
        Destroy(echo1, 4);
    }
    
}