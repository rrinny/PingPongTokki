using UnityEngine;
using UnityEngine.UI;

// Mute 버튼의 상태를 관리하는 클래스
public class MuteButtonManager : MonoBehaviour
{
    public Sprite MuteSprite;         // 음소거 상태일 때 표시할 스프라이트
    public Sprite UnmuteSprite;       // 음소거 해제 상태일 때 표시할 스프라이트
    public Image buttonImage;         // 버튼에 표시될 이미지

    void Start()
    {
        UpdateButtonImage();  // 시작 시 버튼 이미지 초기화
    }

    // Mute 버튼이 클릭되었을 때 호출되는 함수
    public void OnClickMuteButton()
    {
        BGMPlayer.ToggleMute();  // BGM 음소거 상태 토글
        UpdateButtonImage();     // 버튼 이미지 갱신
    }

    // 현재 음소거 상태에 따라 버튼 이미지를 변경하는 함수
    private void UpdateButtonImage()
    {
        bool isMuted = BGMPlayer.IsMuted();  // 현재 음소거 상태 확인
        buttonImage.sprite = isMuted ? MuteSprite : UnmuteSprite;  // 이미지 설정
    }
}