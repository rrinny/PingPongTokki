using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// 타이틀 화면의 UI를 관리하는 클래스
public class TitleManager : MonoBehaviour
{
    public Button startButton;           // 게임 시작 버튼
    public Button guideButton;           // 가이드 보기 버튼
    public Button closeGuideButton;      // 가이드 닫기 버튼
    public Button tokkiButton;           // 핑퐁 모드 진입 버튼

    public GameObject guideBackground;   // 가이드 배경 이미지
    public TextMeshProUGUI guideText;    // 가이드 설명 텍스트

    void Start()
    {
        if (startButton != null)
            startButton.onClick.AddListener(OnStartButtonClicked);  // 시작 버튼 클릭 시 함수 연결

        if (guideButton != null)
            guideButton.onClick.AddListener(OnGuideButtonClicked);  // 가이드 버튼 클릭 시 함수 연결

        if (closeGuideButton != null)
            closeGuideButton.onClick.AddListener(OnCloseGuideClicked);  // 가이드 닫기 버튼 클릭 시 함수 연결
        
        if (tokkiButton != null)
            tokkiButton.onClick.AddListener(OnTokkiButtonClicked);  // 토끼 버튼 클릭 시 함수 연결

        SetGuideUIActive(false);  // 초기에는 가이드 UI 비활성화
    }

    // 시작 버튼 클릭 시 Round1 씬으로 이동
    void OnStartButtonClicked()
    {
        SceneManager.LoadScene("Round1");
    }

    // 가이드 버튼 클릭 시 가이드 UI 활성화
    void OnGuideButtonClicked()
    {
        SetGuideUIActive(true);
    }

    // 가이드 닫기 버튼 클릭 시 가이드 UI 비활성화
    void OnCloseGuideClicked()
    {
        SetGuideUIActive(false);
    }

    // 토끼 버튼 클릭 시 PingPong 씬으로 이동
    void OnTokkiButtonClicked()
    {
        if (PlayerPrefs.GetInt("PingPongUnlocked", 0) == 1)
        {
            SceneManager.LoadScene("PingPong");
        }
    }

    // 가이드 UI의 활성화 여부를 설정하는 함수
    void SetGuideUIActive(bool isActive)
    {
        if (guideBackground != null)
            guideBackground.SetActive(isActive);

        if (guideText != null)
            guideText.gameObject.SetActive(isActive);

        if (closeGuideButton != null)
            closeGuideButton.gameObject.SetActive(isActive);
    }
}