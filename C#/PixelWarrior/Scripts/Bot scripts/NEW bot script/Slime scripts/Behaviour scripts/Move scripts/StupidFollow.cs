using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.UIElements;

public class StupidFollow : MonoBehaviour, IFollowPlayer
{
    [SerializeField] protected string _targetTag;
    protected IBotCharacteristics _characteristics;
    protected Rigidbody2D _botRb;
    protected Transform _targetObj;
    private void Start()
    {
        _characteristics = GetComponent<IBotCharacteristics>();
        _botRb = GetComponent<Rigidbody2D>();
        _targetObj = GameObject.FindGameObjectWithTag(_targetTag).transform;
    }

    public Vector2 ChasePlayer()
    {
        Vector2 moveVector = ((Vector2)_targetObj.transform.position - _botRb.position).normalized * _characteristics.Speed * Time.deltaTime;
        _botRb.velocity += moveVector;

        return moveVector;
    }
}

