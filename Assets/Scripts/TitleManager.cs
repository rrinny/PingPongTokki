using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// Ÿ��Ʋ ȭ���� UI�� �����ϴ� Ŭ����
public class TitleManager : MonoBehaviour
{
    public Button startButton;           // ���� ���� ��ư
    public Button guideButton;           // ���̵� ���� ��ư
    public Button closeGuideButton;      // ���̵� �ݱ� ��ư
    public Button tokkiButton;           // ���� ��� ���� ��ư

    public GameObject guideBackground;   // ���̵� ��� �̹���
    public TextMeshProUGUI guideText;    // ���̵� ���� �ؽ�Ʈ

    void Start()
    {
        if (startButton != null)
            startButton.onClick.AddListener(OnStartButtonClicked);  // ���� ��ư Ŭ�� �� �Լ� ����

        if (guideButton != null)
            guideButton.onClick.AddListener(OnGuideButtonClicked);  // ���̵� ��ư Ŭ�� �� �Լ� ����

        if (closeGuideButton != null)
            closeGuideButton.onClick.AddListener(OnCloseGuideClicked);  // ���̵� �ݱ� ��ư Ŭ�� �� �Լ� ����
        
        if (tokkiButton != null)
            tokkiButton.onClick.AddListener(OnTokkiButtonClicked);  // �䳢 ��ư Ŭ�� �� �Լ� ����

        SetGuideUIActive(false);  // �ʱ⿡�� ���̵� UI ��Ȱ��ȭ
    }

    // ���� ��ư Ŭ�� �� Round1 ������ �̵�
    void OnStartButtonClicked()
    {
        SceneManager.LoadScene("Round1");
    }

    // ���̵� ��ư Ŭ�� �� ���̵� UI Ȱ��ȭ
    void OnGuideButtonClicked()
    {
        SetGuideUIActive(true);
    }

    // ���̵� �ݱ� ��ư Ŭ�� �� ���̵� UI ��Ȱ��ȭ
    void OnCloseGuideClicked()
    {
        SetGuideUIActive(false);
    }

    // �䳢 ��ư Ŭ�� �� PingPong ������ �̵�
    void OnTokkiButtonClicked()
    {
        if (PlayerPrefs.GetInt("PingPongUnlocked", 0) == 1)
        {
            SceneManager.LoadScene("PingPong");
        }
    }

    // ���̵� UI�� Ȱ��ȭ ���θ� �����ϴ� �Լ�
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