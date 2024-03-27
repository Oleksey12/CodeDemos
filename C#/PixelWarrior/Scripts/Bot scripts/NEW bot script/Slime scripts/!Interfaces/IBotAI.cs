using UnityEngine;


/*
 * ��������� ���������� ���������� ����.
 * 
 * ��� ����������� ������:
 * 1) ������������� ��������� �� ������ Scriptable �������
 * 2) ��������� ������� ��������� (���������/����������/�������)
 * 3) ����� ���� ��������
 * 4) ������������ ������������ (� �������/ ��������� / ������� ������)
 */
public abstract class IBotAI : MonoBehaviour
{
    protected IBotCharacteristics _characteristics;
    protected IHealthScript _botHealth;


    protected virtual void HashAllSubscripts()
    {
        _characteristics = GetComponent<IBotCharacteristics>();
        _botHealth = GetComponent<IHealthScript>();
    }

    protected virtual void Start()
    {
        HashAllSubscripts();
    }


    protected virtual void SetStats()
    {

    }

    protected virtual void FixedUpdate()
    {

    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

}
