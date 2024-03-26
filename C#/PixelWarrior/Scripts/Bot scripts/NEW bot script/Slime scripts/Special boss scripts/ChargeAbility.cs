using UnityEngine;

/*
 * Способность босса "рывок", босс разгоняется, а затем с огромной скоростью 
 * устремляется по направлению к игроку
 */

public class ChargeAbility : MonoBehaviour, IBossAbility
{

    [SerializeField] private ParticleSystem chargeParticles;
    [SerializeField] string targetTag;
    Rigidbody2D _bossRb;

    GameObject _target;
    private IBotCharacteristics _characteristics;
    private TrailRenderer _trailEffect;
    private float _chargePower = 4f;
    private float _preWarmTime = 1.5f;
    public bool _isAbilityCasting { get; set; } = false;

    private void Start()
    {
        HashComponents();
    }

    /*
     * Функция запускает последовательную цепочка функций: 
     * PreWarmStage -> ActiveStage -> EndStage
     */
    public void Activate()
    {
        PreWarmStage();
    }

    private void PreWarmStage()
    {
        chargeParticles.Play();
        chargeParticles.enableEmission = true;
        _isAbilityCasting = true;
        _bossRb.velocity = Vector2.zero;
        Invoke("ActiveStage", _preWarmTime);
    }

    private void ActiveStage()
    {
        chargeParticles.enableEmission = false;
        StartCharge();
        PlayChargeTrailEffect(_trailEffect);
        InvokeRepeating("IsAttackEnded", 0f, 0.1f);
    }

    private void EndStage()
    {
        _isAbilityCasting = false;
        DisableTrailEffect();
    }


    private void IsAttackEnded()
    {
        if (_bossRb.velocity.magnitude < 2*_characteristics.VelocityDecrease)
        {
            CancelInvoke("IsAttackEnded");
            EndStage();
        }
    }


    private void PlayChargeTrailEffect(TrailRenderer trailEffect) // Включает след при совершении боссом рывка
    {
        trailEffect.enabled = true;
    }
    private void DisableTrailEffect() => _trailEffect.enabled = false;


    private void StartCharge() // Атака "рывок"
    {
        _bossRb.velocity = (_target.transform.position -_bossRb.transform.position).normalized * _chargePower;
        PlayChargeTrailEffect(_trailEffect);
    }


    private void HashComponents()
    {
        _characteristics = GetComponent<BotCharactersitics>();
        _bossRb = GetComponent<Rigidbody2D>();
        _trailEffect = GetComponent<TrailRenderer>();
        _target = GameObject.FindGameObjectWithTag(targetTag);
    }
}

