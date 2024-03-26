using UnityEngine;
using Random = UnityEngine.Random;

/*
 * Скрипт 2-го уровня в наследовании, управляет реакцией бота на посторонние объекты
 * и его взаимодействие с игроком
 */
public class SlimeBot : DummyBot
{
    [SerializeField] protected ParticleSystem _hitParitcles;
    protected IPlayerDefaultData playerData;
    protected float _botHp;
    protected float _botDamage;
    protected float _knockbackForce;
    protected float _bouncePower;
    protected float _velocityReduce;


   
    protected override void GetValues(BotScriptableObject settings)
    {
        base.GetValues(settings);
        // Получаем данные из объекта данных
        _botDamage = settings.Damage;
        _botHp = settings.Hp;
        _knockbackForce = settings.Knockback;
        _velocityReduce = settings.VelocityDecrease;
        _hitParitcles.Stop();

        _bouncePower = _knockbackForce / 3;

        playerData = FindObjectOfType<PlayerData>();
        // Устанавливаем хп в соотвествии с объектом данных для бота
        gameObject.GetComponent<HealthScript>().setHealth(settings.Hp, settings.Hp);
        gameObject.transform.localScale = new Vector3(settings.Size, settings.Size, settings.Size);
    }
    protected virtual int GetPlayerSide(Vector3 playerCoords, Vector3 botCoords) // Игрок находится слева - 0, справа - 1
    {
        if (playerCoords.x < botCoords.x)
            return 0;
        else
            return 1;
    }

    protected override void Start()
    {
        // Определяем направление движения бота: он идёт направо или налево
        _botDirection = Random.Range(0, 2);

        // В начале извлекаем все данные из объекта данных бота
        GetValues(botSettings);


    }


    protected virtual void VelocityController(Rigidbody2D _rb, float velocityReduce)// Для достижения эффекты "отдачи" после удара
    {

        if (Mathf.Abs(_rb.velocity.x) <= velocityReduce && Mathf.Abs(_rb.velocity.y) <= velocityReduce)
        {
            _rb.velocity = Vector2.zero;
        }
        else
        {
            _rb.velocity = _rb.velocity - _rb.velocity.normalized * velocityReduce;
        }

    }
    protected virtual void ApplyKnockback(Rigidbody2D rbBot, Rigidbody2D rbPlayer, float knockback)
    {

        // Если игрок находится слева, то его откидывает влево, а бота вправо и наоборот
        int forceDirection = GetPlayerSide(rbPlayer.transform.position, rbBot.transform.position);

        // Откидываем бота
        rbBot.velocity = new Vector2(knockback * (1 - 2 * forceDirection),
            rbBot.velocity.y);

        // Откидываем игрока в обратное положение
        rbPlayer.velocity = new Vector2(-knockback * (1 - 2 * forceDirection),
            rbPlayer.velocity.y);

    }
    protected virtual void HitByPlayer(Rigidbody2D rbBot, float playerKnockBack) // Управляет откидыванием при взаимодействии
    {
        
        // Если игрок находится слева,  бота вправо и наоборот
        int forceDirection = GetPlayerSide(_player.transform.position, gameObject.transform.position);

        rbBot.velocity = new Vector2(playerKnockBack * (1 - 2 * forceDirection),
            rbBot.velocity.y);

    }


    protected virtual void BotBounce(Rigidbody2D rbBot,float bouncePower,float PlayerSide) // Симулирует отскок бота от препятствия назад
    {
        rbBot.velocity = new Vector2(rbBot.velocity.x + bouncePower*(1-2*PlayerSide), 0);
    }



    protected override void FixedUpdate()
    {
        if (_rbBot.velocity != Vector2.zero)
            VelocityController(_rbBot, _velocityReduce);
        else if (_movement && _player != null) // Если объект может двигаться
            moveBehaviour(_botSpeed * Time.deltaTime);
    }

    protected virtual void hitAnimation(ParticleSystem hitParitcles)
    {
        hitParitcles.Play();
    }
    protected virtual void OnTriggerEnter2D(Collider2D collision) // При столкновении бота с каким-либо из объектов
    {
        // При столкновении бота с каким-либо объектом
        if (collision.CompareTag("damageBox"))
        {
            HitByPlayer(_rbBot, playerData.PlayerKnockback);
            gameObject.GetComponent<HealthScript>().ChangeCurrentHealth(-playerData.Damage);
            hitAnimation(_hitParitcles);
        }
        else if (collision.CompareTag("Player"))
        {
            // Находим игрока и наносим ему урон
            collision.GetComponent<PlayerHealthScript>().ChangeCurrentHealth(-_botDamage);
            // Откидываем игрока и бота, если игрок может получать урон
            ApplyKnockback(_rbBot, collision.gameObject.GetComponent<Rigidbody2D>(), _knockbackForce);
        }
        else
        {
            if(GetDistance(_player.transform.position, gameObject.transform.position) > _botRange)
            {
                // Если бот столкнулся с препятствием во время бездумного передвижения - меняем его направление
                _botDirection =  1-_botDirection;
            }
            else
            {
                BotBounce(_rbBot, _bouncePower, GetPlayerSide(collision.gameObject.GetComponent<Rigidbody2D>().position, _rbBot.position));
            }
        }
       
    }

    


}
