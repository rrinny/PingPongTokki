using UnityEngine;
using UnityEngine.SceneManagement;

// Replay ��ư�� ���¸� �����ϴ� Ŭ����
public class ReplayButtonManager : MonoBehaviour
{
    public void OnReplayButtonClick()
    {
        Scene currentScene = SceneManager.GetActiveScene(); // ���� Ȱ��ȭ�� ���� ������ ����
        SceneManager.LoadScene(currentScene.name); // ���� ���� �ٽ� �ε�
    }
}