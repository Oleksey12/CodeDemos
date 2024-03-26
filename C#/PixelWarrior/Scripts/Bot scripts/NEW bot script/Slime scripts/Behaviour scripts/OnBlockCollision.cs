using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class OnBlockCollision : MonoBehaviour, IOnBlockCollision
{
    IBotCharacteristics _characteristics;
    private void Start()
    {
        _characteristics = GetComponent<IBotCharacteristics>(); 
    }

    public void RegisterCollision()
    {
        _characteristics.Direction = -_characteristics.Direction;
    }
}

