using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisitRandomPoint : MonoBehaviour, IVisitRandomPoint
{
    [SerializeField] private GameObject[] points;
    [SerializeField] private float butterFlySpeed = 0.2f;
    private Animator _animator;
    private SpriteRenderer _rend;
    private int _currentPoint = 0;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rend = GetComponent<SpriteRenderer> ();
    }

    public void setNewPoint()
    {
        if (points.Length == 1)
        {
            _currentPoint = 0;
            return;
        }

        int newPoint = Random.Range(0, points.Length);
        while (_currentPoint == newPoint)
            newPoint = Random.Range(0, points.Length);

        _currentPoint = newPoint;
    }
    public bool MoveToCurrentPoint()
    {
        Vector3 botCoords = gameObject.transform.position,
            pointCoords = points[_currentPoint].transform.position;
        
        

        if (botCoords != pointCoords)
        {
            gameObject.transform.position = Vector3.MoveTowards(botCoords, pointCoords, butterFlySpeed * Time.deltaTime);
            ChangeDirection(botCoords, pointCoords);
            PlayFlyAnimation(true);
            return false;
        }
        else
        {
            PlayFlyAnimation(false);
            return true;
        }

        
    }
    private void ChangeDirection(Vector3 botCoords, Vector3 pointCoords)
    {
        if (botCoords.x > pointCoords.x)
            _rend.flipX = true;
        else
            _rend.flipX = false;
    }

    void PlayFlyAnimation(bool param)
    {
        _animator.SetBool("IsFlying", param);
    }



}
