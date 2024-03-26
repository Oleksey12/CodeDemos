using UnityEngine;

namespace Assets.Scripts.Player {
    public class MyMath {

        public static Vector3 CalculateKoefficients(float minPoint, float maxPoint) {
            // Создаёт функцию зависимости скорости игрока от силы нажатия кнопки 
            // В данном случае используем параболу
            // y = ax^2 + bx + c
            Vector2 point1 = new Vector2(minPoint, 0);
            Vector2 point2 = new Vector2(maxPoint, 1);

            // Вершина параболы будет находится в точке point2 
            // y' = 2ax + b
            // 0 = 2ax + b
            float bKoefIna = -2 * point2.x;
            // y = ax^2 + kax + c
            // Подставим первую точку
            float cKoefInA = -point1.x * point1.x - bKoefIna * point1.x;
            // y = ax^2 + kax + ma
            // Подставим вторую точку
            float a = 1 / (point2.x * point2.x + bKoefIna * point2.x + cKoefInA);

            return new Vector3(a, a * bKoefIna, a * cKoefInA);
        }

        public static float ParabolaFunc(float val, Vector2 valueRange, Vector3 koefs) {
            val = Mathf.Abs(val);
            if (val > valueRange.y)
                return 1;

            if (val < valueRange.x)
                return 0;

            return koefs.x * val * val + koefs.y * val + koefs.z;
        }

        public static Quaternion VectorToQuaternion(Vector3 direction) {
            float angle = Mathf.Acos(Vector2.Dot(direction.normalized, new Vector2(1, 0)));
            if (direction.normalized.y < 0) {
                angle = -angle;
            }
            if (direction.normalized.x < 0) {
                angle = Mathf.PI + angle;
            }
            return Quaternion.Euler(0, 0, 180 / Mathf.PI * angle);
        }
    }
}