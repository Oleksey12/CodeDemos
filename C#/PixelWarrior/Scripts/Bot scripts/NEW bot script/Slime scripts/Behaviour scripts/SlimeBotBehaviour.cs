using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/*
 * Скрипт реализует поведение бота, его передвижения, анимации, реакцию на удар
 */
public class SlimeBotBehaviour : MonoBehaviour, IBotBehaviour
{ 
    [SerializeField] protected string _targetTag;
    protected IMoveAnimation _botMoveAnimation;
    protected IBotCharacteristics _characteristics;
    protected IDefaultMovement _defaultMovement;
    protected IFollowPlayer _followPlayer;
    protected GameObject _targetObj;
    protected float _dist;


    protected virtual void Start()
    {
        // Сохраняем ссылки на используемые классы
        HashComponents();

        _targetObj = GameObject.FindGameObjectWithTag(_targetTag);

        InvokeRepeating("ChangeDirection", 0f, 0.15f);
    }


    protected float Distance(GameObject obj1, GameObject obj2)
    {
        return Vector3.Distance(obj1.transform.position, obj2.transform.position);
    }

    public virtual void Behave()
    {
        _dist = Distance(gameObject, _targetObj);

        if(_dist > _characteristics.Range)
        {
            _defaultMovement.Move();
        }
        else if(_dist <= _characteristics.Range)
        {
            _followPlayer.ChasePlayer();
        }

    }
    public void ChangeDirection()
    {
        _botMoveAnimation.SetMoveDirection();
    }
    protected virtual void HashComponents()
    {
        _characteristics = GetComponent<IBotCharacteristics>();
        _defaultMovement = GetComponent<IDefaultMovement>();
        _followPlayer = GetComponent<IFollowPlayer>();
        _botMoveAnimation = GetComponent<IMoveAnimation>();
    }


}


