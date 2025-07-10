using UnityEngine;

[RequireComponent(typeof(AudioSource))]
// BGM을 재생하고 유지하는 싱글톤 클래스
public class BGMPlayer : MonoBehaviour
{
    private static BGMPlayer instance;      // 싱글톤 인스턴스
    private AudioSource audioSource;        // 오디오 소스 컴포넌트

    private static bool isMuted = false;    // 음소거 상태 저장

    void Awake()
    {
        if (instance == null)
        {
            instance = this;                            // 인스턴스 초기화
            DontDestroyOnLoad(gameObject);              // 씬 전환 시 파괴되지 않도록 설정

            audioSource = GetComponent<AudioSource>();  // AudioSource 컴포넌트 가져오기
            audioSource.loop = true;                    // 루프 재생 설정
            audioSource.playOnAwake = false;            // 자동 재생 비활성화

            audioSource.Play();                         // BGM 재생 시작
        }
        else
        {
            Destroy(gameObject); // 중복된 BGMPlayer가 생기면 제거
        }
    }

    // BGM의 재생 속도를 변경하는 함수
    public static void SetPitch(float pitch)
    {
        if (instance != null && instance.audioSource != null)
        {
            instance.audioSource.pitch = pitch;
        }
    }

    // BGM 음소거 상태를 토글하는 함수
    public static void ToggleMute()
    {
        isMuted = !isMuted;
        if (instance != null && instance.audioSource != null)
        {
            instance.audioSource.mute = isMuted;
        }
    }

    // 현재 음소거 상태를 반환하는 함수
    public static bool IsMuted()
    {
        return isMuted;
    }
}