using UnityEngine;
using UnityEngine.UI;

// Mute ��ư�� ���¸� �����ϴ� Ŭ����
public class MuteButtonManager : MonoBehaviour
{
    public Sprite MuteSprite;         // ���Ұ� ������ �� ǥ���� ��������Ʈ
    public Sprite UnmuteSprite;       // ���Ұ� ���� ������ �� ǥ���� ��������Ʈ
    public Image buttonImage;         // ��ư�� ǥ�õ� �̹���

    void Start()
    {
        UpdateButtonImage();  // ���� �� ��ư �̹��� �ʱ�ȭ
    }

    // Mute ��ư�� Ŭ���Ǿ��� �� ȣ��Ǵ� �Լ�
    public void OnClickMuteButton()
    {
        BGMPlayer.ToggleMute();  // BGM ���Ұ� ���� ���
        UpdateButtonImage();     // ��ư �̹��� ����
    }

    // ���� ���Ұ� ���¿� ���� ��ư �̹����� �����ϴ� �Լ�
    private void UpdateButtonImage()
    {
        bool isMuted = BGMPlayer.IsMuted();  // ���� ���Ұ� ���� Ȯ��
        buttonImage.sprite = isMuted ? MuteSprite : UnmuteSprite;  // �̹��� ����
    }
}