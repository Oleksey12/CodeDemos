using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Скрипт 4-го уровня в наследовании, добавляет боту спец породу босса
 * супер удары, изменённое поведение и т.д. Для босса существует
 * отдельный файл с константными параметрами его базовых характеристик
 */
public sealed class BossScript : CustomSlimeBot
{

    [SerializeField] private Image mask;
    [SerializeField] private Image form;
    [SerializeField] private ParticleSystem chargeParticles;
    [SerializeField] private ParticleSystem teleportParticles;
    private TrailRenderer _trailEffect;
    private int _attackTypesCount = 2;
    private int _attackTimer = 0, _attackTimerMax = 450;
    private int _attackType = 0;
    private float _bossKnockbackResistance = 0.1f;
    private float _chargePower = 5f;
    private Vector2 _lastPlayerCoords;
    private bool _isAttackCasting = false;

   
    public void DestroyHealthBar() // Уничтожает полоску здоровья босса после его смерти
    {
        Destroy(mask.gameObject);
        Destroy(form.gameObject);
    }
    public void maskUpdate(float curHealth, float maxHealth) // Обновляет хп босса при получении им урона
    {
        if (curHealth >= 0.1f)
            mask.fillAmount = curHealth / maxHealth;
        else
            DestroyHealthBar();
    }



    protected override void GetValues(BotScriptableObject settings)
    {
        base.GetValues(settings);
        // Получаем ссылку на компонент ответственный за след бота
        _trailEffect = gameObject.GetComponent<TrailRenderer>();
        // Выключаем анимации всех частиц до их отображения
        chargeParticles.enableEmission = false;
        teleportParticles.Stop();
    }
    private void SetBossStats(GameObject botObj,float health,float size,float damage,float speed)
    {
        _botDamage = damage * _difficultyLevel;
        _botSpeed = speed * _difficultyLevel;

        botObj.GetComponent<HealthScript>().setHealth
            (health * _difficultyLevel,
            health * _difficultyLevel);

        botObj.transform.localScale = new Vector3(size, size, size);  

    }
    protected override void SetBotBreed(GameObject botObj)
    {
        // Использует для бота анимации 4-го спрайта
        _botAnimator.SetInteger("BotType", 4);
        // Устанавливает боту спрайт босса
        SetSprite(botObj,botSettings.BotType4);
        // Устанавливает параметры для босса внезависимости от породы
        SetBossStats(botObj,_botHp,_botSize,_botDamage,_botSpeed);
    }



    protected override void ApplyKnockback(Rigidbody2D rbBot, Rigidbody2D rbPlayer, float knockback) // Управляет эффектом столкновения игрока и бота
    {

        // Если игрок находится слева, то его откидывает влево, а бота вправо и наоборот
        int forceDirection = GetPlayerSide(rbPlayer.transform.position, rbBot.transform.position);

        // Откидываем бота, так как это босс, то вводим доп.параметр, уменьшающий откдиывание босса
        rbBot.velocity = new Vector2(rbBot.velocity.x + _bossKnockbackResistance * knockback * (1 - 2 * forceDirection),
            rbBot.velocity.y);

        // Откидываем игрока в обратное положение
        rbPlayer.velocity = new Vector2(-knockback * (1 - 2 * forceDirection),
            rbPlayer.velocity.y);

    }
    protected override void HitByPlayer(Rigidbody2D rbBot, float playerKnockback) // Управляет откидыванием при взаимодействии
    {

        // Если игрок находится слева,  бота вправо и наоборот
        int forceDirection = GetPlayerSide(_player.transform.position, gameObject.transform.position);

        rbBot.velocity = new Vector2(rbBot.velocity.x + playerKnockback * _bossKnockbackResistance * (1 - 2 * forceDirection),
            rbBot.velocity.y);



    }
    private void UpdateBotStats(GameObject botObj,float currentHealth,float maxHealth) // Изменяет параметры босса в зависимости от его хп
    {
        botObj.transform.localScale = new Vector3(_botSize / 2 + currentHealth / maxHealth * _botSize / 2,
            _botSize / 2 + currentHealth / maxHealth * _botSize / 2,
            _botSize / 2 + currentHealth / maxHealth * _botSize / 2);

        _botSpeed = (2 - currentHealth / maxHealth) * botSettings.Speed/2 + botSettings.Speed/2;
    }

    protected override void OnTriggerEnter2D(Collider2D collision) // Взаимодействие босса с остальными объектами
    {
        // При столкновении бота с хитбоксом атаки
        if (collision.CompareTag("damageBox"))
        {
            // Если игрок атаковал босса, то обновляем его хп, параметры, проигрываем эффект отдачи
            maskUpdate(gameObject.GetComponent<HealthScript>().Current_health - playerData.Damage, gameObject.GetComponent<HealthScript>().Max_health);
            UpdateBotStats(gameObject, gameObject.GetComponent<HealthScript>().Current_health - playerData.Damage, gameObject.GetComponent<HealthScript>().Max_health);
            HitByPlayer(_rbBot, playerData.PlayerKnockback);
            gameObject.GetComponent<HealthScript>().ChangeCurrentHealth(-playerData.Damage);
            hitAnimation(_hitParitcles);
        }
        // Если бот всё же достал игрока
        else if (collision.CompareTag("Player"))
        {
            // Находим игрока и наносим ему урон
            collision.GetComponent<PlayerHealthScript>().ChangeCurrentHealth(-_botDamage);
            // Откидываем игрока и бота, если игрок может получать урон
            ApplyKnockback(_rbBot, collision.gameObject.GetComponent<Rigidbody2D>(), _knockbackForce);
        }
        else
        {
            if (GetDistance(_player.transform.position, gameObject.transform.position) > _botRange)
            {
                // Если бот столкнулся с препятствием во время бездумного передвижения - меняем его направление
                _botDirection = 1 - _botDirection;
            }
        }

    }





    protected override void moveBehaviour(float moveSpeed) // Управляет движением босса
    {
        // В обычном состоянии босс просто преследует игрока
        if (!_isAttackCasting && _player != null)
            _rbBot.transform.position = BotMovement(_player.transform.position, moveSpeed);

        // Когда приходит время босс наносит супер удар
        if (_attackTimer == _attackTimerMax)
        {
            _isAttackCasting = true;
            CastNewAttack();
            _attackTimer = 0;
        }



        ++_attackTimer;

    }

    private void PlayChargeTrailEffect(TrailRenderer trailEffect) // Включает след при совершении боссом рывка
    {
        trailEffect.enabled = true;
        // Отключаем эффект следа после окончания рывка
        Invoke("DisableTrailEffect", 1.5f);
    }
    private void DisableTrailEffect() => _trailEffect.enabled = false;

    private void PlayTeleportParticles(ParticleSystem teleportParticles) => teleportParticles.Play();
    private void CastNewAttack() // Вызывает одну из функций атаки
    {
        _attackType = Random.Range(0, _attackTypesCount);
        switch (_attackType)
        {
            case 0:
                {
                    chargeParticles.enableEmission = true;
                    Invoke("StartCharge", 1.5f/_difficultyLevel);
                    break;
                }
            case 1:
                {
                    _lastPlayerCoords = _player.transform.position;
                    Invoke("TeleportToPlayer", 0.5f/_difficultyLevel);
                    break;
                }
        }
    }


    private void StartCharge() // Атака "рывок"
    {
        chargeParticles.enableEmission = false;
        _rbBot.velocity = BotMovement(_player.transform.position, _chargePower);
        PlayChargeTrailEffect(_trailEffect);
        _isAttackCasting = false;
    }
    private void TeleportToPlayer() // Атака "телепорт"
    {
        _rbBot.transform.position = _lastPlayerCoords- BotMovement(_player.transform.position, 1f)*0.01f;
        PlayTeleportParticles(teleportParticles);
        _isAttackCasting = false;
    }
}
