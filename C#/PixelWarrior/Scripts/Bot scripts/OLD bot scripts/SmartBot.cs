using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

/*
 * Попытка создания ИИ для бота, скрипт оставлен на доработку до лучших времён
 * скрипт работает, но не учитывается размер самого бота и положение других ботов
 */
public class SmartBot : CustomSlimeBot
{

    protected float _closeDistance;
    protected float _nextWaypointDist = 0.64f;
    protected const float recalculateTime = 0.5f;
    protected float _repeatTime;
    protected float bezieT = 0f;

    protected Pathfinding.Seeker _seekedPath;
    protected Pathfinding.Path _path;
    protected Rigidbody2D _rbPlayer;
    protected Vector2 _moveVector;



    protected override void GetValues(BotScriptableObject settings)
    {

        base.GetValues(settings);
        // Получаем данные об игроке
        _rbPlayer = _player.GetComponent<Rigidbody2D>();


        // Настраиваем вычисление пути до игрока
        _seekedPath = GetComponent<Pathfinding.Seeker>();
        _repeatTime = recalculateTime;
        _closeDistance = _botRange / 2;

    }
    protected virtual void OnPathComplete(Pathfinding.Path p)
    {
        if (!p.error)
        {
            _path = p;
            _path.vectorPath[0] = _rbBot.transform.position;
        }
    }
    protected virtual void UpdatePath()
    {
        if (_seekedPath.IsDone())
        {
            bezieT = 0f;
            _seekedPath.StartPath(_rbBot.position, _player.transform.position, OnPathComplete);

        }
    }


    protected float pathSize(IList<Vector3> waypoints) => Vector3.Distance(waypoints[0], waypoints[waypoints.Count - 1]);
  

    // Алгоритм движения бота по точкам созданного массива пути
    protected virtual void PathFollowing(Rigidbody2D rbBot, Rigidbody2D rbPlayer, float speed)
    {
        if (_path == null)
            return;

        Debug.Log(bezieT);

        bezieT += 1 / (pathSize(_path.vectorPath) / speed);
        // Если мы прошли все точки пути и достигли текущей цели
        if (bezieT >= 1)
        {
            // Просто приследуем игрока до просчёта следующего маршрута
            rbBot.transform.position = BotMovement(rbPlayer.transform.position, _botSpeed * Time.deltaTime);
            return;
        }

        _rbBot.position = BezieCurveMovement(_path.vectorPath, bezieT);


    }

    protected virtual void BotMoveBehavour(Rigidbody2D rbBot, Rigidbody2D rbPlayer, float speed)
    {
        float dist = Vector3.Distance(rbBot.transform.position, rbPlayer.transform.position);
        if (dist > _botRange)
        {
            _rbBot.transform.position = DefaultMovement(rbBot.transform.position, speed * 0.8f);
            bezieT = 1f;
        }
        else
        {
            PathFollowing(rbBot, rbPlayer, speed);
            /*
            if(dist > _closeDistance)
            {
                //Debug.Log("Bot is not close " + dist);
                _repeatTime = recalculateTime;
                PathFollowing(rbBot,rbPlayer, speed);
            }
            else// if(dist > _closeDistance/2)
            {
                //Debug.Log("Bot is close " + dist);
                _repeatTime = recalculateTime/1.5f;
                PathFollowing(rbBot,rbPlayer,speed);
            }
            
            else
            {
                //Debug.Log("Bot is following " + dist);
                _rbBot.transform.position = BotMovement(rbPlayer.transform.position, speed);
            }
            */
        }
    }

    // Апроксимация кривой безье
    protected virtual Vector2 BezieCurveMovement(IList<Vector3> points, float newT)
    {

        float
            // Следующая точка кривой безье
            newX = 0, newY = 0, pointsCount = points.Count;




        for (int i = 0; i < pointsCount; ++i)
        {

            // Вычисляем новое положение бота на кривой безье, включающей в себя точки графа мин.пути
            // x = ((1-t)+t)^n, где каждый член выражения умножен на xi
            // y = ((1-t)+t)^n, где каждый член выражения умножен на yi


            newX += (float)CoffeicentCount((int)pointsCount - 1, i) *
                Mathf.Pow(1 - newT, pointsCount - 1 - i) * Mathf.Pow(newT, i) * points[i].x;
            newY += (float)CoffeicentCount((int)pointsCount - 1, i) *
                Mathf.Pow(1 - newT, pointsCount - 1 - i) * Mathf.Pow(newT, i) * points[i].y;


        }

        return new Vector2(newX, newY);
    }

    // Расчёт коэффицентов в уравнении кривой безье
    protected virtual double CoffeicentCount(int x1, int n)
    {
        double result = 1f;

        // Cn = x1! / ((x1-n)!*n!)

        for (int i = 0; i < n; ++i)
        {
            result *= (x1 - i);
        }

        int nCopy = n;
        while (n > 1)
        {
            result /= (double)n;
            --n;
        }

        return result;
    }

    protected virtual void BezieCurveDraw(IList<Vector3> points)
    {


        float pointsCount = points.Count;
        float
            // Следующая точка кривой безье
            newX = 0, newY = 0,
            // Первая точка кривой безье находится в первой точке графа пути
            curX = points[0].x, curY = points[0].y;


        for (float curT = 0.01f; curT <= 1f; curT += 0.01f)
        {
            for (int i = 0; i < pointsCount; ++i)
            {

                // Вычисляем новое положение бота на кривой безье, включающей в себя точки графа мин.пути
                // x = ((1-t)+t)^n, где каждый член выражения умножен на xi
                // y = ((1-t)+t)^n, где каждый член выражения умножен на yi


                newX += (float)CoffeicentCount((int)pointsCount - 1, i) *
                    Mathf.Pow(1 - curT, pointsCount - 1 - i) * Mathf.Pow(curT, i) * points[i].x;
                newY += (float)CoffeicentCount((int)pointsCount - 1, i) *
                    Mathf.Pow(1 - curT, pointsCount - 1 - i) * Mathf.Pow(curT, i) * points[i].y;


            }
            Gizmos.DrawLine(new Vector3(curX, curY), new Vector3(newX, newY));
            curX = newX;
            curY = newY;
            newX = 0f;
            newY = 0f;
        }
    }
    // Апроксимация полиномом лагранжа
    protected virtual float ExpressionCounter(IList<Vector3> points, float pointsCount, float x)
    {
        float curExpression = 1f, result = 0;
        for (int i = 0; i < pointsCount; ++i)
        {
            // Вычисляем значение i-го члена интерполяционного полинома Лагранжа
            for (int j = 0; j < pointsCount; ++j)
            {
                if (j != i)
                    curExpression *= (x - points[j].x) / (points[i].x - points[j].x);
            }
            curExpression *= points[i].y;

            result += curExpression;
            curExpression = 1f;
        }
        return result;
    }
    protected virtual void LagranshInterpolationDraw(IList<Vector3> points)
    {

        float step = 100;


        float pointsCount = points.Count;
        float
            // Следующая точка кривой безье
            newY = 0,
            // Первая точка кривой безье находится в первой точке графа пути
            curX = points[0].x, curY = points[0].y;

        if (points[0].x < points[points.Count - 1].x)
        {
            for (float x = points[0].x; x <= points[points.Count - 1].x; x += (points[points.Count - 1].x - points[0].x) / step)
            {
                newY = ExpressionCounter(points, pointsCount, x);

                if (x != points[0].x)
                {
                    // Проводим линию между новой и предыдущей точкой
                    Gizmos.DrawLine(new Vector3(curX, curY), new Vector3(x, newY));
                }
                curX = x;
                curY = newY;
            }
        }
        else
        {
            for (float x = points[0].x; x >= points[points.Count - 1].x; x += (points[points.Count - 1].x - points[0].x) / step)
            {
                newY = ExpressionCounter(points, pointsCount, x);

                if (x != points[0].x)
                {
                    // Проводим линию между новой и предыдущей точкой
                    Gizmos.DrawLine(new Vector3(curX, curY), new Vector3(x, newY));
                }
                curX = x;
                curY = newY;
            }
        }

    }

    protected override void Start()
    {
        base.Start();
        // В начале извлекаем все данные из объекта данных бота
        GetValues(botSettings);

        //InvokeRepeating("UpdatePath", 0, _repeatTime);

    }

    protected override void FixedUpdate()
    {
        if (_rbBot.velocity != Vector2.zero)
            VelocityController(_rbBot, _velocityReduce);
        else
        {
            BotMoveBehavour(_rbBot, _rbPlayer, _botSpeed * Time.deltaTime);
            //PathFollowing();
        }


    }
    protected override void VelocityController(Rigidbody2D _rb, float velocityReduce)
    {
        if (Mathf.Abs(_rb.velocity.x) <= velocityReduce)
        {
            bezieT = 1f;
            _rb.velocity = Vector2.zero;
        }
        else
        {
            if (_rb.velocity.x > 0)
                _rb.velocity = new Vector2(_rb.velocity.x - velocityReduce, _rb.velocity.y);
            if (_rb.velocity.x < 0)
                _rb.velocity = new Vector2(_rb.velocity.x + velocityReduce, _rb.velocity.y);
        }
    }


    // Рисование вычисленного пути движения бота
    private void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        BezieCurveDraw(_path.vectorPath);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_rbBot.transform.position, _botRange);


        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(_rbBot.transform.position, _closeDistance);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(_rbBot.transform.position, _closeDistance / 2);

        Gizmos.color = Color.magenta;
        LagranshInterpolationDraw(_path.vectorPath);
    }

    





    
}
