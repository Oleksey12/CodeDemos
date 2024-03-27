using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * ������ 4-�� ������ � ������������, ��������� ���� ���� ������ �����
 * ����� �����, ��������� ��������� � �.�. ��� ����� ����������
 * ��������� ���� � ������������ ����������� ��� ������� �������������
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

   
    public void DestroyHealthBar() // ���������� ������� �������� ����� ����� ��� ������
    {
        Destroy(mask.gameObject);
        Destroy(form.gameObject);
    }
    public void maskUpdate(float curHealth, float maxHealth) // ��������� �� ����� ��� ��������� �� �����
    {
        if (curHealth >= 0.1f)
            mask.fillAmount = curHealth / maxHealth;
        else
            DestroyHealthBar();
    }



    protected override void GetValues(BotScriptableObject settings)
    {
        base.GetValues(settings);
        // �������� ������ �� ��������� ������������� �� ���� ����
        _trailEffect = gameObject.GetComponent<TrailRenderer>();
        // ��������� �������� ���� ������ �� �� �����������
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
        // ���������� ��� ���� �������� 4-�� �������
        _botAnimator.SetInteger("BotType", 4);
        // ������������� ���� ������ �����
        SetSprite(botObj,botSettings.BotType4);
        // ������������� ��������� ��� ����� �������������� �� ������
        SetBossStats(botObj,_botHp,_botSize,_botDamage,_botSpeed);
    }



    protected override void ApplyKnockback(Rigidbody2D rbBot, Rigidbody2D rbPlayer, float knockback) // ��������� �������� ������������ ������ � ����
    {

        // ���� ����� ��������� �����, �� ��� ���������� �����, � ���� ������ � ��������
        int forceDirection = GetPlayerSide(rbPlayer.transform.position, rbBot.transform.position);

        // ���������� ����, ��� ��� ��� ����, �� ������ ���.��������, ����������� ����������� �����
        rbBot.velocity = new Vector2(rbBot.velocity.x + _bossKnockbackResistance * knockback * (1 - 2 * forceDirection),
            rbBot.velocity.y);

        // ���������� ������ � �������� ���������
        rbPlayer.velocity = new Vector2(-knockback * (1 - 2 * forceDirection),
            rbPlayer.velocity.y);

    }
    protected override void HitByPlayer(Rigidbody2D rbBot, float playerKnockback) // ��������� ������������ ��� ��������������
    {

        // ���� ����� ��������� �����,  ���� ������ � ��������
        int forceDirection = GetPlayerSide(_player.transform.position, gameObject.transform.position);

        rbBot.velocity = new Vector2(rbBot.velocity.x + playerKnockback * _bossKnockbackResistance * (1 - 2 * forceDirection),
            rbBot.velocity.y);



    }
    private void UpdateBotStats(GameObject botObj,float currentHealth,float maxHealth) // �������� ��������� ����� � ����������� �� ��� ��
    {
        botObj.transform.localScale = new Vector3(_botSize / 2 + currentHealth / maxHealth * _botSize / 2,
            _botSize / 2 + currentHealth / maxHealth * _botSize / 2,
            _botSize / 2 + currentHealth / maxHealth * _botSize / 2);

        _botSpeed = (2 - currentHealth / maxHealth) * botSettings.Speed/2 + botSettings.Speed/2;
    }

    protected override void OnTriggerEnter2D(Collider2D collision) // �������������� ����� � ���������� ���������
    {
        // ��� ������������ ���� � ��������� �����
        if (collision.CompareTag("damageBox"))
        {
            // ���� ����� �������� �����, �� ��������� ��� ��, ���������, ����������� ������ ������
            maskUpdate(gameObject.GetComponent<HealthScript>().Current_health - playerData.Damage, gameObject.GetComponent<HealthScript>().Max_health);
            UpdateBotStats(gameObject, gameObject.GetComponent<HealthScript>().Current_health - playerData.Damage, gameObject.GetComponent<HealthScript>().Max_health);
            HitByPlayer(_rbBot, playerData.PlayerKnockback);
            gameObject.GetComponent<HealthScript>().ChangeCurrentHealth(-playerData.Damage);
            hitAnimation(_hitParitcles);
        }
        // ���� ��� �� �� ������ ������
        else if (collision.CompareTag("Player"))
        {
            // ������� ������ � ������� ��� ����
            collision.GetComponent<PlayerHealthScript>().ChangeCurrentHealth(-_botDamage);
            // ���������� ������ � ����, ���� ����� ����� �������� ����
            ApplyKnockback(_rbBot, collision.gameObject.GetComponent<Rigidbody2D>(), _knockbackForce);
        }
        else
        {
            if (GetDistance(_player.transform.position, gameObject.transform.position) > _botRange)
            {
                // ���� ��� ���������� � ������������ �� ����� ���������� ������������ - ������ ��� �����������
                _botDirection = 1 - _botDirection;
            }
        }

    }





    protected override void moveBehaviour(float moveSpeed) // ��������� ��������� �����
    {
        // � ������� ��������� ���� ������ ���������� ������
        if (!_isAttackCasting && _player != null)
            _rbBot.transform.position = BotMovement(_player.transform.position, moveSpeed);

        // ����� �������� ����� ���� ������� ����� ����
        if (_attackTimer == _attackTimerMax)
        {
            _isAttackCasting = true;
            CastNewAttack();
            _attackTimer = 0;
        }



        ++_attackTimer;

    }

    private void PlayChargeTrailEffect(TrailRenderer trailEffect) // �������� ���� ��� ���������� ������ �����
    {
        trailEffect.enabled = true;
        // ��������� ������ ����� ����� ��������� �����
        Invoke("DisableTrailEffect", 1.5f);
    }
    private void DisableTrailEffect() => _trailEffect.enabled = false;

    private void PlayTeleportParticles(ParticleSystem teleportParticles) => teleportParticles.Play();
    private void CastNewAttack() // �������� ���� �� ������� �����
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


    private void StartCharge() // ����� "�����"
    {
        chargeParticles.enableEmission = false;
        _rbBot.velocity = BotMovement(_player.transform.position, _chargePower);
        PlayChargeTrailEffect(_trailEffect);
        _isAttackCasting = false;
    }
    private void TeleportToPlayer() // ����� "��������"
    {
        _rbBot.transform.position = _lastPlayerCoords- BotMovement(_player.transform.position, 1f)*0.01f;
        PlayTeleportParticles(teleportParticles);
        _isAttackCasting = false;
    }
}
