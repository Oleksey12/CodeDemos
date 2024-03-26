using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
/*
 * Способность "телепорт", спустя некоторое время телепортирует босса 
 * в место, где был игрок минуту назад.
 */
public class TeleportAbility : MonoBehaviour, IBossAbility
{

    [SerializeField] private ParticleSystem teleportParticles;
    [SerializeField] private string targetTag;
    private Rigidbody2D _bossRb;

    Vector3 _lastPlayerCoodrs;
    private GameObject _target;
    private float _preWarmTime = 0.75f;

    private void Start()
    {
        _bossRb = GetComponent<Rigidbody2D>();
        _target = GameObject.FindGameObjectWithTag(targetTag);
    }
    public bool _isAbilityCasting { get; set; } = false;


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
        _isAbilityCasting = true;
        _lastPlayerCoodrs = _target.transform.position; 
        _bossRb.velocity = Vector2.zero;
        Invoke("ActiveStage", _preWarmTime);
    }
    private void ActiveStage()
    {
        TeleportToPlayer(_lastPlayerCoodrs);
        PlayTeleportParticles(teleportParticles);
        EndStage();
        
    }
    private void EndStage()
    {
        _isAbilityCasting = false;
    }






    // Атака "телепорт"
    private void TeleportToPlayer(Vector3 playerCoords) => _bossRb.transform.position = 
        Vector3.MoveTowards(_bossRb.transform.position, playerCoords,
        Vector3.Distance(_bossRb.transform.position, playerCoords) -0.1f);

    private void PlayTeleportParticles(ParticleSystem teleportParticles) => teleportParticles.Play();
}

