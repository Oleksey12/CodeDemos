using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 * Скрипт определяет поведение перехода на следующий уровень
 */

public interface ILeavelEndManager
{
    // Проверяем прошёл игрок уровень или нет
    public void IsLevelEnded();
}

