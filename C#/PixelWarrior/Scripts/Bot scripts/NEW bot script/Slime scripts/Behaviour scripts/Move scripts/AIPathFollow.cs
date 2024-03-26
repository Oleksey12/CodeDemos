using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/*
 * Скрипт реализует приследование игрока,при помощи A* star
 * алгоритма поиска оптимального пути в графе
 */
public class AIPathFollow : MonoBehaviour, IFollowPlayer
{
    [SerializeField] protected string _targetTag;
    protected Transform _targetObj;
    protected float _nextWaypointDist = 0.2f;
    protected const float _recalculateTime = 0.5f;
    protected int _currentWayPoint = 0;

    protected IBotCharacteristics _characteristics;
    protected Rigidbody2D _botRb;
    protected Pathfinding.Seeker _seekedPath;

    protected List<Vector3> _path;

    private void Start()
    {
        HashComponents();
        InvokeRepeating("UpdatePath", 0, _recalculateTime);
    }

    public Vector2 ChasePlayer()
    {
        return PathFollowing(_botRb, _targetObj);

    }

    protected virtual void OnPathComplete(Pathfinding.Path p)
    {
        if (!p.error)
        {
            _path = p.vectorPath;
        }
    }
    protected virtual void UpdatePath()
    {
        if (_seekedPath.IsDone())
        {
            _currentWayPoint = 0;
            _seekedPath.StartPath(_botRb.position, _targetObj.position, OnPathComplete);

        }
    }
    protected virtual Vector2 PathFollowing(Rigidbody2D rbBot, Transform target)
    {
        Vector2 moveVector = Vector2.zero;
        // Бот приследует игрока по просчитанному пути, после завершения он начинает просто приследовать игрока.
        if (_path != null && _currentWayPoint < _path.Count)
        {
            Vector2 direction = ((Vector2)_path[_currentWayPoint] - _botRb.position).normalized;
            moveVector = direction * _characteristics.Speed * Time.deltaTime;
            _botRb.velocity += moveVector;
        }
        else
        {
            moveVector = ((Vector2)_targetObj.position - _botRb.position).normalized
                * _characteristics.Speed 
                * Time.deltaTime;

            _botRb.velocity += moveVector;
            return moveVector;
        }

        // Если мы дошли до текущего узла графа, то двигаемся к следующему
        if (Vector3.Distance(_botRb.position, _path[_currentWayPoint]) < _nextWaypointDist)
            ++_currentWayPoint;

        return moveVector;

    }

    private void HashComponents()
    {
        _seekedPath = GetComponent<Pathfinding.Seeker>();
        _botRb = GetComponent<Rigidbody2D>();
        _targetObj = GameObject.FindGameObjectWithTag(_targetTag).transform;
        _characteristics = GetComponent<IBotCharacteristics>();
    }
}

