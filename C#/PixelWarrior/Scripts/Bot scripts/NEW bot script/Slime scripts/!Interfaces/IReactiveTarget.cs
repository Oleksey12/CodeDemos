using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
/*
 * Скрипт определяет реакцию объекта на получение урона
 */
public interface IReactiveTarget
{
    public void React(Vector3 senderPosition);
}
