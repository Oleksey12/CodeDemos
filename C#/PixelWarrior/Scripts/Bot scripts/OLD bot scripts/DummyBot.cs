using UnityEngine;
/*
 * ������ 1-�� ������ � ������������, ���
 * ��������� ������ ��������� � ������������ ������
 */
public abstract class DummyBot : MonoBehaviour
{
    /* ScriptableObject ���� � ������� �������� � ���� ��������
     * ������ � ��� ������� ��������� � ���������� 
     * (��������, ������� ������������ �� ������, �����, ������ ��������� ����� � �.�)
    */
    [SerializeField] protected BotScriptableObject botSettings;

    protected Rigidbody2D _rbBot;
    protected GameObject _player;
    protected float _botRange;
    protected float _botSpeed;
    protected bool _movement;

    // ��������� ������������ ���������� �������� ����, 0 - ������, 1 �����
    protected float _botDirection; 
    protected Vector2 _moveDirection;

    protected virtual float GetDistance(Vector3 playerCords, Vector3 botCoords)
    {
        float result;
        // ��������� ���������� ����� ������� � ������������ ���� � ������
        result = Mathf.Sqrt((playerCords.x - botCoords.x) * (playerCords.x - botCoords.x) + (playerCords.y - botCoords.y) * (playerCords.y - botCoords.y));

        return result;
    } // ���������� ���������� ����� ����� � ������������

    protected virtual void GetValues(BotScriptableObject settings)
    {
        _rbBot = gameObject.GetComponent<Rigidbody2D>();
        _player = GameObject.FindGameObjectWithTag("Player");

        // �������� ������ �� ������� ������
        _movement = settings.Movement;
        _botRange = settings.Range;
        _botSpeed = settings.Speed;
        _botDirection = Random.Range(0, 2);
        

        

       

    }// ���������� ������ ���� �� ��� "�����"

    protected virtual Vector2 BotMovement(Vector3 playerCoords, float moveSpeed) // ��������� �������������� ������
    {

        // ��������� ������������ ������ �� �������� �������� �������� ���, ������ ����� ��������� ����������

        // ������ �� ������� ��������� ���
        float newVectorX, newVectorY;

        // �����, ��� ������ ��������� ���
        float x0 = gameObject.transform.position.x,
            y0 = gameObject.transform.position.y;

        // ����� ������
        float x1 = playerCoords.x,
            y1 = playerCoords.y;

        // ������ ����������, ������������ �������� ����� ��������
        float colliniarVectorX = x1 - x0,
            colliniarVectorY = y1 - y0;

        float k = colliniarVectorX / colliniarVectorY;

        /*
         ������� ��������� ����:
         
         { newVectorX/newVectorZ = k
         { newVectorX^2 + newVectorZ^2 = _botspeed
         
        */

        // ������� ��������� ��� ������ ��������� X � ����������� ��� �� ������ ���������
        if (colliniarVectorY > 0)
            newVectorY = Mathf.Sqrt(moveSpeed * moveSpeed / (1 + k * k));
        else
        {
            newVectorY = -Mathf.Sqrt(moveSpeed * moveSpeed / (1 + k * k));
        }
        newVectorX = k * newVectorY;

        // ����� ���������� ����

        // ��������� ������ �������� ���� � ������� ������
        _moveDirection = new Vector2(newVectorX, newVectorY);

        // ��������� ����� ���������� ����
        newVectorX = x0 + newVectorX;
        newVectorY = y0 + newVectorY;

        return new Vector2(newVectorX, newVectorY);

    }
    protected virtual Vector3 DefaultMovement(Vector3 curentBotPosition,float moveSpeed) // ��������� ��������� � ��������� ������
    {
        switch (_botDirection)
        {
            case 0: // ��� ��������� ������
                {

                    return new Vector3(curentBotPosition.x +
                            moveSpeed / 1.5f, curentBotPosition.y, 0);

                }
            case 1: // ��� �������� �����
                {

                    return new Vector3(curentBotPosition.x -
                            moveSpeed / 1.5f, curentBotPosition.y, 0);

                }
        }
        return curentBotPosition;
    }
    protected virtual void moveBehaviour(float moveSpeed) // ������� ������������ �������� �������
    {
        // ����� ������� ��������� ������
        Vector3 playerCoords = _player.transform.position;
        Vector3 curentBotPosition = _rbBot.position;


        // ���� ����� � ���� ��������� ����
        if (GetDistance(playerCoords,curentBotPosition) < _botRange)
        {
            _rbBot.transform.position = BotMovement(playerCoords, moveSpeed);
        }
        else // ���� ����� ������ �� ����
        {
            _rbBot.transform.position = DefaultMovement(curentBotPosition, moveSpeed);
        }
    }

    protected virtual void Start()
    {

        // ���������� ����������� �������� ����: �� ��� ������� ��� ������
        _botDirection = Random.Range(0, 2);

        // � ������ ��������� ��� ������ �� ������� ������ ����
        GetValues(botSettings);


    }


    protected virtual void FixedUpdate()
    {

        if (_movement && _player != null) // ���� ������ ����� ��������� � ����� ����������
            moveBehaviour(_botSpeed*Time.deltaTime);
       
    }


    
}
