using UnityEngine;

[RequireComponent(typeof(AudioSource))]
// BGM�� ����ϰ� �����ϴ� �̱��� Ŭ����
public class BGMPlayer : MonoBehaviour
{
    private static BGMPlayer instance;      // �̱��� �ν��Ͻ�
    private AudioSource audioSource;        // ����� �ҽ� ������Ʈ

    private static bool isMuted = false;    // ���Ұ� ���� ����

    void Awake()
    {
        if (instance == null)
        {
            instance = this;                            // �ν��Ͻ� �ʱ�ȭ
            DontDestroyOnLoad(gameObject);              // �� ��ȯ �� �ı����� �ʵ��� ����

            audioSource = GetComponent<AudioSource>();  // AudioSource ������Ʈ ��������
            audioSource.loop = true;                    // ���� ��� ����
            audioSource.playOnAwake = false;            // �ڵ� ��� ��Ȱ��ȭ

            audioSource.Play();                         // BGM ��� ����
        }
        else
        {
            Destroy(gameObject); // �ߺ��� BGMPlayer�� ����� ����
        }
    }

    // BGM�� ��� �ӵ��� �����ϴ� �Լ�
    public static void SetPitch(float pitch)
    {
        if (instance != null && instance.audioSource != null)
        {
            instance.audioSource.pitch = pitch;
        }
    }

    // BGM ���Ұ� ���¸� ����ϴ� �Լ�
    public static void ToggleMute()
    {
        isMuted = !isMuted;
        if (instance != null && instance.audioSource != null)
        {
            instance.audioSource.mute = isMuted;
        }
    }

    // ���� ���Ұ� ���¸� ��ȯ�ϴ� �Լ�
    public static bool IsMuted()
    {
        return isMuted;
    }
}