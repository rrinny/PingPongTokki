using UnityEngine;

// 사용자 입력에 따라 토끼를 좌우로 이동시키는 컨트롤러 클래스
public class RabbitController : MonoBehaviour
{
    public RabbitManager manager;  // RabbitManager에 접근하기 위한 참조

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
        manager.SendBottomRabbit(-1);  // 왼쪽 방향으로 토끼 이동
    }

    public void MoveRight()
    {
        manager.SendBottomRabbit(1);  // 오른쪽 방향으로 토끼 이동
    }
}