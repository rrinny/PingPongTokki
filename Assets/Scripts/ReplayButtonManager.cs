using UnityEngine;
using UnityEngine.SceneManagement;

// Replay 버튼의 상태를 관리하는 클래스
public class ReplayButtonManager : MonoBehaviour
{
    public void OnReplayButtonClick()
    {
        Scene currentScene = SceneManager.GetActiveScene(); // 현재 활성화된 씬의 정보를 저장
        SceneManager.LoadScene(currentScene.name); // 현재 씬을 다시 로드
    }
}