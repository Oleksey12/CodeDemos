using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButterflyController : MonoBehaviour
{
    IVisitRandomPoint _moveController;
    private float _sittingTime = 250f;
    private float _curTick = 0f;

    private void Start()
    {
        _moveController = GetComponent<IVisitRandomPoint>();
        _moveController.setNewPoint();
    }
    private void FixedUpdate()
    {

        
        if (_moveController.MoveToCurrentPoint())
        {
            if (_curTick <= _sittingTime)
                ++_curTick;
            else
            {
                _curTick = 0f;
                _moveController.setNewPoint();
            }
        }
        


    }
}
