using UnityEngine;


public class PlayerDataController : MonoBehaviour {
    protected enum modes { 
        hardcoded,
        scriptableObject,
        none
    }

    [Header("Режим загрузки данных")]
    [SerializeField]
    [Tooltip("Лучше не трогать, если не знаешь")] protected modes mode;

    [Header("Статистика")]
    [SerializeField] protected int killsCount;
    [SerializeField] protected float timeAlive;

    [Header("Внутриигровые параметры")]
    [SerializeField] protected int healthDelimeter;
    [SerializeField] protected float invincibilityTime;

    [Header("Начальные значения")]
    [SerializeField] protected int maxHealth;
    [SerializeField] protected float speed;
    [SerializeField] protected int currentHealth;
    [SerializeField] protected int currentLevel;
    [SerializeField] protected int pointsLeft;


    [Header("Параметры атаки")]
    [SerializeField] protected string attackName;
    [SerializeField] protected float attackDamageBoxDist;
    [SerializeField] protected float attackSpearDist;
    [SerializeField] protected float attackDamage;
    [SerializeField] protected float attackTime;
    [SerializeField] protected float attackCooldown;
    [SerializeField] protected float attackSpeed;
    [SerializeField] protected float attackKnockback;


    [Header("Параметры рывка")]
    [SerializeField] protected string chargeName;
    [SerializeField] protected float chargeTime;
    [SerializeField] protected float chargeCooldown;
    [SerializeField] protected float chargeSpeed;
    [SerializeField] protected float chargeEchoCooldown;

    [Header("Уровни")]
    [SerializeField] protected int healthLevel;
    [SerializeField] protected int attackLevel;
    [SerializeField] protected int chargeLevel;


    public PlayerData Initialize() {
        if (mode == modes.hardcoded) {
            killsCount = 0;
            timeAlive = 0f;

            invincibilityTime = 0.25f;
            healthDelimeter = 15;

            currentHealth = 3;
            maxHealth = 3;
            speed = 1.8f;
            currentLevel = 0;
            pointsLeft = 0;

            attackName = "Attack";
            attackTime = 0.22f;
            attackCooldown = 1.3f;
            attackDamageBoxDist = 0.11f;
            attackSpearDist = 0.2f;
            attackSpeed = 0.5f;
            attackDamage = 15f;
            attackKnockback = 0.3f;

            chargeName = "Charge";
            //chargeCooldown = 4.5f;
            chargeCooldown = 0.1f;
            chargeTime = 0.93f;
            chargeSpeed = 2.5f;
            chargeEchoCooldown = 0.075f;

            healthLevel = 0;
            attackLevel = 0;
            chargeLevel = 0;
        }

        return InitializePlayerData();
        
    }

    protected PlayerData InitializePlayerData() {
        PlayerData data = new PlayerData();
        InitializeStatistics(data.statistics);
        InitializeGameSettings(data.gameSettings);
        InitializeGameParams(data.gameParams);
        InitializeAttackSettings(data.attackSettings);
        InitializeChargeSettings(data.chargeSettings);
        InitializeLevels(data.levels);
        return data;
    }
    private void InitializeLevels(Levels levels) {
        levels.healthLevel = new Levels.HealthLevel();
        levels.attackLevel = new Levels.AttackLevel();
        levels.chargeLevel = new Levels.ChargeLevel();

        levels.healthLevel.ChangeValue(healthLevel);
        levels.attackLevel.ChangeValue(attackLevel);
        levels.chargeLevel.ChangeValue(chargeLevel);
    }
    private void InitializeChargeSettings(ChargeSettings chargeSettings) {
        chargeSettings.chargeName = new ChargeSettings.ChargeName();
        chargeSettings.chargeTime = new ChargeSettings.ChargeTime();
        chargeSettings.chargeCooldown = new ChargeSettings.ChargeCooldown();
        chargeSettings.chargeSpeed = new ChargeSettings.ChargeSpeed();
        chargeSettings.chargeEchoCooldown = new ChargeSettings.ChargeEchoCooldown();

        chargeSettings.chargeName.ChangeValue(chargeName);
        chargeSettings.chargeTime.ChangeValue(chargeTime);
        chargeSettings.chargeCooldown.ChangeValue(chargeCooldown);
        chargeSettings.chargeSpeed.ChangeValue(chargeSpeed);
        chargeSettings.chargeEchoCooldown.ChangeValue(chargeEchoCooldown);
    }
    private void InitializeAttackSettings(AttackSettings attackSettings) {
        attackSettings.attackName = new AttackSettings.AttackName();
        attackSettings.attackDamageBoxDist = new AttackSettings.AttackDamageBoxDist();
        attackSettings.attackSpearDist = new AttackSettings.AttackSpearDist();
        attackSettings.attackDamage = new AttackSettings.AttackDamage();
        attackSettings.attackTime = new AttackSettings.AttackTime();
        attackSettings.attackCooldown = new AttackSettings.AttackCooldown();
        attackSettings.attackSpeed = new AttackSettings.AttackSpeed();
        attackSettings.attackKnockback = new AttackSettings.AttackKnockback();

        attackSettings.attackName.ChangeValue(attackName);
        attackSettings.attackDamageBoxDist.ChangeValue(attackDamageBoxDist);
        attackSettings.attackSpearDist.ChangeValue(attackSpearDist);
        attackSettings.attackDamage.ChangeValue(attackDamage);
        attackSettings.attackTime.ChangeValue(attackTime);
        attackSettings.attackCooldown.ChangeValue(attackCooldown);
        attackSettings.attackSpeed.ChangeValue(attackSpeed);
        attackSettings.attackKnockback.ChangeValue(attackKnockback);
    }
    private void InitializeGameParams(GameParams gameParams) {
        gameParams.maxHealth = new GameParams.MaxHealth();
        gameParams.currentHealth = new GameParams.CurrentHealth();
        gameParams.speed = new GameParams.Speed();
        gameParams.currentLevel = new GameParams.CurrentLevel();
        gameParams.pointsLeft = new GameParams.PointsLeft();

        gameParams.maxHealth.ChangeValue(maxHealth);
        gameParams.currentHealth.ChangeValue(currentHealth);
        gameParams.speed.ChangeValue(speed);
        gameParams.currentLevel.ChangeValue(currentLevel);
        gameParams.pointsLeft.ChangeValue(pointsLeft);
    }

    private void InitializeStatistics(Statistics statistics) {
        statistics.killsCount = new Statistics.KillsCount();
        statistics.timeAlive = new Statistics.TimeAlive();

        statistics.killsCount.ChangeValue(killsCount);
        statistics.timeAlive.ChangeValue(timeAlive);
    }

    private void InitializeGameSettings(GameSettings gameSettings) {
        gameSettings.invincibilityTime = new GameSettings.InvincibilityTime();
        gameSettings.healthDelimeter = new GameSettings.HealthDelimeter();

        gameSettings.invincibilityTime.ChangeValue(invincibilityTime);
        gameSettings.healthDelimeter.ChangeValue(healthDelimeter);
    }

    /*
   
    private void Start() {
        playerController = gameObject.GetComponent<MainController>();
    }

    public override void TakeDamage(float damage) {
        currentHealth -= damage / healthDelimeter;
        playerController.ReactOnDamage();
        if (currentHealth <= 0) {
            Die();
            currentHealth = 0;
        }
    }
    protected override void Die() {
        // Проигрываем анимацию и переходим в состояние смерти
        playerController.Die();
    }
    
    public void lvlUp() {
        ++currentLevel;
        ++pointsLeft;
    }
    */
}

