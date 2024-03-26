using System.Collections.Generic;
using UnityEngine;

public class BezieInterpolation : MonoBehaviour
{
    // Апроксимация кривой безье
    public virtual Vector2 BezieCurveMovement(IList<Vector3> points, int waypointNum, int waypointCount,  float curT)
    {
        float
            // Следующая точка кривой безье
            newX = 0, newY = 0;

        for (int i = waypointNum; i < waypointNum + waypointCount; ++i)
        {
            // Вычисляем новое положение бота на кривой безье, включающей в себя точки графа мин.пути
            // x = ((1-t)+t)^n, где каждый член выражения умножен на xi
            // y = ((1-t)+t)^n, где каждый член выражения умножен на yi

            newX += (float)CoffeicentCount((int)waypointCount - 1, i) *
                Mathf.Pow(1 - curT, waypointCount - 1 - i) * Mathf.Pow(curT, i) * points[i].x;
            newY += (float)CoffeicentCount((int)waypointCount - 1, i) *
                Mathf.Pow(1 - curT, waypointCount - 1 - i) * Mathf.Pow(curT, i) * points[i].y;
        }
        return new Vector2(newX, newY);
    }

    // Расчёт коэффицентов в уравнении кривой безье
    protected virtual float CoffeicentCount(int x1, int n)
    {
        float result = 1f;

        // Cn = x1! / ((x1-n)!*n!)

        for (int i = 0; i < n; ++i)
        {
            result *= (x1 - i);
        }

        int nCopy = n;
        while (n > 1)
        {
            result /= (float)n;
            --n;
        }

        return result;
    }
}