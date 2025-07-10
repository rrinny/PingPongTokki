using UnityEngine;

// ����� �Է¿� ���� �䳢�� �¿�� �̵���Ű�� ��Ʈ�ѷ� Ŭ����
public class RabbitController : MonoBehaviour
{
    public RabbitManager manager;  // RabbitManager�� �����ϱ� ���� ����

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveLeft();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveRight();
        }
    }

    public void MoveLeft()
    {
        manager.SendBottomRabbit(-1);  // ���� �������� �䳢 �̵�
    }

    public void MoveRight()
    {
        manager.SendBottomRabbit(1);  // ������ �������� �䳢 �̵�
    }
}