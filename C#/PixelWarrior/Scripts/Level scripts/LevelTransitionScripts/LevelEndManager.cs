using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Скрипт проверяет окончания уровня игроком
 */
public class LevelEndManager: MonoBehaviour, ILeavelEndManager
{
    [SerializeField] protected GameObject _gates;
    [SerializeField] protected string _awaitedTag = "Slime";

    protected virtual void Start()
    {
        InvokeRepeating("IsLevelEnded",0, 0.5f);
    }

    public void IsLevelEnded() 
    {
        if (GameObject.FindGameObjectWithTag(_awaitedTag) == null)
        {
            LevelEndFunction();
            CancelInvoke("IsLevelEnded");
        }
    }


    protected virtual void LevelEndFunction() 
    {
        RemoveGates();
    }

    protected virtual void RemoveGates() 
    {
        if (_gates != null)
            Destroy(_gates);
    }

}
