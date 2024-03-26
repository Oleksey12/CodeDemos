using UnityEngine;
/*
 * Скрипт 1-го уровня в наследовании, бот
 * способный только двигаться и преследовать игрока
 */
public abstract class DummyBot : MonoBehaviour
{
    /* ScriptableObject бота в котором хранятся в виде констант
     * данные о его базовых свойствах и параметрах 
     * (скорости, радиусе реагирования на игрока, уроне, отдачи получении урона и т.д)
    */
    [SerializeField] protected BotScriptableObject botSettings;

    protected Rigidbody2D _rbBot;
    protected GameObject _player;
    protected float _botRange;
    protected float _botSpeed;
    protected bool _movement;

    // Управляет направлением бездумного движения бота, 0 - вправо, 1 влево
    protected float _botDirection; 
    protected Vector2 _moveDirection;

    protected virtual float GetDistance(Vector3 playerCords, Vector3 botCoords)
    {
        float result;
        // Вычисляем расстояние между точками с координатами бота и игрока
        result = Mathf.Sqrt((playerCords.x - botCoords.x) * (playerCords.x - botCoords.x) + (playerCords.y - botCoords.y) * (playerCords.y - botCoords.y));

        return result;
    } // Возвращает расстояние между ботом и координатами

    protected virtual void GetValues(BotScriptableObject settings)
    {
        _rbBot = gameObject.GetComponent<Rigidbody2D>();
        _player = GameObject.FindGameObjectWithTag("Player");

        // Получаем данные из объекта данных
        _movement = settings.Movement;
        _botRange = settings.Range;
        _botSpeed = settings.Speed;
        _botDirection = Random.Range(0, 2);
        

        

       

    }// Извлечение данных бота из его "карты"

    protected virtual Vector2 BotMovement(Vector3 playerCoords, float moveSpeed) // Управляет приследованием игрока
    {

        // Вычисляем коллиниарный вектор по которому совершит движение бот, пройдя ровно указанное расстояние

        // Вектор на который сместится бот
        float newVectorX, newVectorY;

        // Точка, где сейчас находится бот
        float x0 = gameObject.transform.position.x,
            y0 = gameObject.transform.position.y;

        // Точка игрока
        float x1 = playerCoords.x,
            y1 = playerCoords.y;

        // Вектор расстояния, коллиниарным которому будет итоговый
        float colliniarVectorX = x1 - x0,
            colliniarVectorY = y1 - y0;

        float k = colliniarVectorX / colliniarVectorY;

        /*
         Решение уравнения вида:
         
         { newVectorX/newVectorZ = k
         { newVectorX^2 + newVectorZ^2 = _botspeed
         
        */

        // Решение уравнения при помощи выражения X и подстановки его во второе уравнение
        if (colliniarVectorY > 0)
            newVectorY = Mathf.Sqrt(moveSpeed * moveSpeed / (1 + k * k));
        else
        {
            newVectorY = -Mathf.Sqrt(moveSpeed * moveSpeed / (1 + k * k));
        }
        newVectorX = k * newVectorY;

        // Новые координаты бота

        // Сохраняем вектор движения бота в текущий момент
        _moveDirection = new Vector2(newVectorX, newVectorY);

        // Вычисляем новые координаты бота
        newVectorX = x0 + newVectorX;
        newVectorY = y0 + newVectorY;

        return new Vector2(newVectorX, newVectorY);

    }
    protected virtual Vector3 DefaultMovement(Vector3 curentBotPosition,float moveSpeed) // Управляет движением в отсутсвии игрока
    {
        switch (_botDirection)
        {
            case 0: // Бот двигается вправо
                {

                    return new Vector3(curentBotPosition.x +
                            moveSpeed / 1.5f, curentBotPosition.y, 0);

                }
            case 1: // Бот движется влево
                {

                    return new Vector3(curentBotPosition.x -
                            moveSpeed / 1.5f, curentBotPosition.y, 0);

                }
        }
        return curentBotPosition;
    }
    protected virtual void moveBehaviour(float moveSpeed) // Функция описывающаая движение объекта
    {
        // Узнаём текущее положение игрока
        Vector3 playerCoords = _player.transform.position;
        Vector3 curentBotPosition = _rbBot.position;


        // Если игрок в зоне видимости бота
        if (GetDistance(playerCoords,curentBotPosition) < _botRange)
        {
            _rbBot.transform.position = BotMovement(playerCoords, moveSpeed);
        }
        else // Если игрок далеко от бота
        {
            _rbBot.transform.position = DefaultMovement(curentBotPosition, moveSpeed);
        }
    }

    protected virtual void Start()
    {

        // Определяем направление движения бота: он идёт направо или налево
        _botDirection = Random.Range(0, 2);

        // В начале извлекаем все данные из объекта данных бота
        GetValues(botSettings);


    }


    protected virtual void FixedUpdate()
    {

        if (_movement && _player != null) // Если объект может двигаться и игрок загрузился
            moveBehaviour(_botSpeed*Time.deltaTime);
       
    }


    
}
