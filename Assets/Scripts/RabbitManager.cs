using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

// 토끼의 개별 상태를 저장하는 클래스
public class Rabbit : MonoBehaviour
{
    public bool isFlipped = false;          // 토끼가 뒤집힌 상태인지 여부
    public Sprite originalSprite;           // 원본 스프라이트 저장용
}

// 토끼 전체 관리 및 게임 진행 로직을 담당하는 클래스
public class RabbitManager : MonoBehaviour
{
    public GameObject[] rabbitPrefabs;      // 토끼 프리팹 배열 (6가지 색깔)

    public Transform centerXPoint;          // 중앙 위치 기준점
    public Transform leftXPoint;            // 좌측 위치 기준점
    public Transform rightXPoint;           // 우측 위치 기준점

    public List<GameObject> rabbitStack = new List<GameObject>(); // 중앙 열 토끼 목록
    private float[] yPositions = new float[] { -2.5f, -1.5f, -0.5f, 0.5f, 1.5f, 2.5f, 3.5f, 4.5f }; // 중앙 토끼 Y 좌표들

    public float flyDistance = 5f;          // 토끼가 날아갈 거리
    public float flyDuration = 0.5f;        // 날아가는 시간

    public TextMeshProUGUI scoreText;       // 점수 텍스트 UI
    public TextMeshProUGUI comboText;       // 콤보 텍스트 UI
    public Image hpBarImage;                 // HP 바 UI 이미지

    private float score = 0;                 // 현재 점수
    private float combo = 0;                 // 현재 콤보

    private float roundClearScore = 20000;  // 현재 라운드 클리어 점수 기준

    public float maxHpTime = 60f;            // 최대 HP 시간
    private float currentHpTime;             // 현재 HP 시간
    public float penaltyTime = 1f;           // 틀렸을 때 페널티 시간

    public FeverManager feverManager;        // 피버 모드 매니저 참조
    private float feverCombo = 15;           // 피버 시작 콤보 기준
    private float feverEndCombo = 0;         // 피버 종료 시 누적 콤보

    private AudioSource audioSource;         // 오디오 소스 컴포넌트
    public AudioClip correctSound;           // 정답 효과음
    public AudioClip wrongSound;             // 오답 효과음
    public AudioClip gameOverSound;          // 게임오버 효과음
    public AudioClip gameClearSound;         // 게임 클리어 효과음
    public AudioClip feverStartSound;        // 피버 시작 효과음

    public Image clearImage;                 // 클리어 이미지
    public Image gameClearImage;             // 6라운드 클리어 이미지
    public Image gameOverImage;              // 게임 오버 이미지

    private bool isCleared = false;          // 클리어 상태 플래그

    public int currentRound = 1;             // 현재 라운드 번호

    private List<int> chosenColors = new List<int>(); // 이번 라운드에서 선택된 색상 인덱스 리스트
    private List<int> leftColors = new List<int>();   // 좌측 토끼 색상 리스트
    private List<int> rightColors = new List<int>();  // 우측 토끼 색상 리스트

    public GameObject resetButton;           // 리셋 버튼 오브젝트
    public GameObject retryButton;           // 재시도 버튼 오브젝트

    public GameObject panel; // 반투명 패널

    void Start()
    {
        chosenColors.Clear();   // 색상 리스트 초기화
        rabbitStack.Clear();    // 토끼 스택 초기화

        audioSource = GetComponent<AudioSource>(); // 오디오 소스 컴포넌트 할당

        roundClearScore = GetRoundClearScoreByRound(currentRound); // 라운드별 클리어 점수 설정

        int pairCount = GetPairCountByRound(currentRound); // 라운드별 쌍 개수 설정

        penaltyTime = Mathf.Lerp(1f, 2f, (currentRound - 1) / 5f); // 페널티 시간 조절 (라운드 증가 시 증가)

        bool unlocked = PlayerPrefs.GetInt("PingPongUnlocked", 0) == 1;

        if (SceneManager.GetActiveScene().name == "PingPong" && unlocked)
        {
            currentRound = 6;
            maxHpTime = 120f;
            roundClearScore = float.MaxValue;
        }

        currentHpTime = maxHpTime;

        float[] yPositionsForSideRabbits = new float[] { -0.2f, 1.8f, 3.8f }; // 좌우 토끼 Y 위치 배열

        // 중복 없이 토끼 색상 인덱스 뽑기
        chosenColors.Clear();
        leftColors.Clear();
        rightColors.Clear();

        while (chosenColors.Count < pairCount * 2)
        {
            int randomColor = Random.Range(0, rabbitPrefabs.Length);
            if (!chosenColors.Contains(randomColor))
                chosenColors.Add(randomColor);
        }

        // 좌측 토끼 생성 및 위치 지정
        for (int i = 0; i < pairCount; i++)
        {
            int leftColorIndex = chosenColors[i];
            leftColors.Add(leftColorIndex);

            Vector3 leftPos = new Vector3(leftXPoint.position.x, yPositionsForSideRabbits[i], leftXPoint.position.z);
            GameObject rabbit = Instantiate(rabbitPrefabs[leftColorIndex], leftPos, Quaternion.identity);
            rabbit.name = rabbitPrefabs[leftColorIndex].name;
        }

        // 우측 토끼 생성 및 위치 지정
        for (int i = 0; i < pairCount; i++)
        {
            int rightColorIndex = chosenColors[pairCount + i];
            rightColors.Add(rightColorIndex);

            Vector3 rightPos = new Vector3(rightXPoint.position.x, yPositionsForSideRabbits[i], rightXPoint.position.z);
            GameObject rabbit = Instantiate(rabbitPrefabs[rightColorIndex], rightPos, Quaternion.identity);
            rabbit.name = rabbitPrefabs[rightColorIndex].name;
        }

        // 중앙 열 토끼 8마리 생성
        for (int i = 0; i < 8; i++)
        {
            AddNewRabbitAtTop();
        }

        UpdatePositions();     // 토끼 위치 업데이트
    }

    void Update()
    {
        if (isCleared) return;  // 클리어 시 동작 중지

        float decreaseRate = GetHpDecreaseRateByRound(currentRound);  // 라운드별 HP 감소 속도
        currentHpTime -= Time.deltaTime * decreaseRate;               // HP 감소
        currentHpTime = Mathf.Clamp(currentHpTime, 0, maxHpTime);     // HP 범위 제한
        UpdateHpBar();                                                // HP 바 갱신

        if (currentHpTime <= 0)
        {
            if (SceneManager.GetActiveScene().name == "PingPong")
            {
                StartCoroutine(ShowClearAndNextScene());
            }
            else
            {
                // 게임 오버 처리
                GameOver();
            }
        }
    }

    // 라운드별 좌우 토끼 쌍 개수 반환
    private int GetPairCountByRound(int round)
    {
        if (round == 1) return 1;
        else if (round >= 2 && round <= 3) return 2;
        else if (round >= 4 && round <= 6) return 3;
        else return 1;
    }

    // 라운드별 HP 감소 속도 반환
    private float GetHpDecreaseRateByRound(int round)
    {
        if (round == 1) return 1.0f;
        else if (round == 2) return 1.1f;
        else if (round == 3) return 1.2f;
        else if (round == 4) return 1.3f;
        else if (round == 5) return 1.4f;
        else if (round == 6) return 1.5f;
        else return 1.0f;
    }

    // 라운드별 클리어 점수 기준 반환
    private int GetRoundClearScoreByRound(int round)
    {
        if (round == 1) return 20000;
        else if (round == 2) return 50000;
        else if (round == 3) return 70000;
        else if (round == 4) return 100000;
        else if (round == 5) return 120000;
        else if (round == 6) return 150000;
        else return 20000;
    }

    // HP 바 UI 업데이트
    private void UpdateHpBar()
    {
        if (hpBarImage != null)
        {
            hpBarImage.fillAmount = currentHpTime / maxHpTime;
        }
    }

    // 게임 오버 처리
    private void GameOver()
    {
        isCleared = true;  // 게임 종료 상태 설정

        if (feverManager != null && feverManager.isFeverMode)
        {
            feverManager.EndFeverMode(rabbitStack);  // 피버 모드 종료
        }

        PlaySound(gameOverSound, 0.5f); // 게임 오버 효과음 재생

        panel.SetActive(true); // 반투명 패널 활성화

        if (gameOverImage != null)
        {
            gameOverImage.gameObject.SetActive(true);  // 게임 오버 이미지 표시
            retryButton.SetActive(true);                // 재시도 버튼 활성화
        }
    }

    // 하단에 있는 토끼를 좌(-1) 또는 우(1) 방향으로 보내는 함수
    public void SendBottomRabbit(int direction)
    {
        if (isCleared) return;  // 클리어 상태면 무시

        if (rabbitStack.Count == 0) return; // 토끼가 없으면 무시

        GameObject bottomRabbit = rabbitStack[0]; // 맨 아래 토끼 선택

        int bottomRabbitIndex = GetPrefabIndex(bottomRabbit); // 해당 토끼의 색상 인덱스 가져오기

        // 토끼에 붙은 Rabbit 컴포넌트에서 뒤집힘 여부 확인
        Rabbit rabbitComp = bottomRabbit.GetComponent<Rabbit>();
        bool isFlipped = rabbitComp != null && rabbitComp.isFlipped;

        bool isCorrectDirection; // 토끼가 맞게 보내졌는지 여부 판별 변수


        if (feverManager != null && feverManager.isFeverMode)
        {
            // 피버 모드일 경우 방향 관계없이 항상 정답으로 처리
            isCorrectDirection = true;
        }
        else if (isFlipped)
        {
            // 토끼가 좌우가 뒤집혀 있는 상태일 때
            // 왼쪽 방향이면 원래 오른쪽 색상 그룹에 속해야 정답
            // 오른쪽 방향이면 원래 왼쪽 색상 그룹에 속해야 정답
            isCorrectDirection = (direction == -1 && rightColors.Contains(bottomRabbitIndex)) || // ← 오른쪽
                                 (direction == 1 && leftColors.Contains(bottomRabbitIndex));     // → 왼쪽
        }
        else
        {
            // 토끼가 기본 상태(뒤집히지 않은 상태)일 때
            // 왼쪽 방향이면 왼쪽 색상 그룹에 속해야 정답
            // 오른쪽 방향이면 오른쪽 색상 그룹에 속해야 정답
            isCorrectDirection = (direction == -1 && leftColors.Contains(bottomRabbitIndex)) ||  // ← 왼쪽
                                 (direction == 1 && rightColors.Contains(bottomRabbitIndex));    // → 오른쪽
        }

        if (isCorrectDirection)
        {
            PlaySound(correctSound, 1.45f); // 정답 사운드 재생
            IncreaseScoreAndCombo();        // 점수 및 콤보 증가
        }
        else
        {
            PlaySound(wrongSound, 1.75f);   // 오답 사운드 재생

            combo = 0;                     // 콤보 초기화
            feverEndCombo = 0;             // 피버 관련 콤보도 초기화
            UpdateComboText();             // 콤보 UI 갱신

            currentHpTime -= penaltyTime;  // 체력(시간) 감소
            currentHpTime = Mathf.Clamp(currentHpTime, 0, maxHpTime); // 체력 최소 0, 최대값 제한
            UpdateHpBar();                 // 체력 바 UI 갱신
        }

        rabbitStack.RemoveAt(0);   // 스택에서 하단 토끼 제거
        AddNewRabbitAtTop();       // 새 토끼를 스택 맨 위에 추가
        UpdatePositions();         // 토끼들 위치 갱신

        StartCoroutine(FlyAwayAndDestroy(bottomRabbit, direction)); // 토끼가 날아가면서 파괴되는 코루틴 실행
    }

    // 전달받은 토끼 게임 오브젝트의 프리팹 배열 내 인덱스를 반환하는 함수
    // rabbitPrefabs 배열에서 토끼 이름과 일치하는 프리팹을 찾아 인덱스를 리턴
    private int GetPrefabIndex(GameObject rabbit)
    {
        for (int i = 0; i < rabbitPrefabs.Length; i++)
        {
            // "(Clone)" 제거하고 공백도 제거한 이름과 프리팹 이름 비교
            if (rabbitPrefabs[i].name == rabbit.name.Replace("(Clone)", "").Trim())
            {
                return i;  // 일치하는 프리팹 인덱스 반환
            }
        }
        return -1;  // 못 찾으면 -1 반환
    }

    // 토끼를 좌우 방향으로 날려 보내면서 일정 시간 후 파괴하는 코루틴
    private IEnumerator FlyAwayAndDestroy(GameObject rabbit, int direction)
    {
        float elapsed = 0f;                       // 경과 시간 초기화
        Vector3 startPos = rabbit.transform.position;  // 시작 위치 저장
        Vector3 targetPos = startPos + new Vector3(flyDistance * direction, 0, 0);  // 목표 위치 계산 (좌/우 방향으로 이동)

        // flyDuration 시간 동안 위치를 선형 보간해서 부드럽게 이동
        while (elapsed < flyDuration)
        {
            // 현재 위치를 시작과 목표 위치 사이에서 비율에 맞게 계산하여 이동
            rabbit.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / flyDuration);
            elapsed += Time.deltaTime;  // 시간 누적
            yield return null;          // 한 프레임 대기
        }

        // 최종 위치 보정 (정확하게 목표 위치로 이동)
        rabbit.transform.position = targetPos;

        // 토끼 오브젝트 파괴 (씬에서 제거)
        Destroy(rabbit);
    }

    // 스택 맨 위에 새로운 토끼를 생성하여 추가하는 함수
    private void AddNewRabbitAtTop()
    {
        // chosenColors 리스트에서 랜덤으로 하나 선택 (토끼 색상 인덱스)
        int chosenIndex = chosenColors[Random.Range(0, chosenColors.Count)];

        // 선택한 인덱스에 해당하는 프리팹을 가져와서 인스턴스 생성
        GameObject prefab = rabbitPrefabs[chosenIndex];
        GameObject newRabbit = Instantiate(prefab, Vector3.zero, Quaternion.identity);

        // 생성된 토끼의 스프라이트 컴포넌트 가져오기
        SpriteRenderer sr = newRabbit.GetComponent<SpriteRenderer>();

        // Rabbit 컴포넌트 가져오기 (없으면 새로 추가)
        Rabbit rabbitComp = newRabbit.GetComponent<Rabbit>();

        if (rabbitComp == null)
            rabbitComp = newRabbit.AddComponent<Rabbit>();

        if (sr != null && rabbitComp != null)
        {
            rabbitComp.originalSprite = sr.sprite; // 원래 스프라이트 저장

            // 피버 모드일 때는 토끼 스프라이트를 피버 전용 스프라이트로 교체
            if (feverManager != null && feverManager.isFeverMode)
            {
                sr.sprite = feverManager.feverRabbitSprite;
            }
        }

        // 현재 라운드에 따라 뒤집힘 확률을 5%에서 15%까지 선형 증가
        float flipProbability = Mathf.Lerp(0.05f, 0.15f, (currentRound - 1) / 5f);

        // 피버 모드가 아닐 때만 뒤집힘 여부 결정
        if (feverManager == null || !feverManager.isFeverMode)
        {
            if (Random.value < flipProbability)
            {
                newRabbit.transform.rotation = Quaternion.Euler(0, 0, 180);  // 180도 회전
                rabbitComp.isFlipped = true;  // 뒤집힘 상태 표시
            }
        }

        newRabbit.tag = "Rabbit";  // 태그 지정

        rabbitStack.Add(newRabbit);  // 토끼 스택에 새 토끼 추가
    }

    // 토끼 스택에 있는 토끼들의 위치를 업데이트하는 함수
    private void UpdatePositions()
    {
        for (int i = 0; i < rabbitStack.Count && i < yPositions.Length; i++)
        {
            // 중앙 x 좌표와 미리 정한 yPositions 배열의 y 좌표를 이용해 위치 지정
            Vector3 newPos = new Vector3(centerXPoint.position.x, yPositions[i], 0);
            rabbitStack[i].transform.position = newPos;

            SpriteRenderer sr = rabbitStack[i].GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                // 아래에 있는 토끼일수록 높은 sortingOrder를 부여해서 앞에 보이도록 설정
                sr.sortingOrder = rabbitStack.Count - i;
            }
        }
    }

    // 점수와 콤보를 증가시키는 함수
    private void IncreaseScoreAndCombo()
    {
        combo += 1;

        // 기본 점수 계산 (라운드에 따라 증가)
        float baseScore = 1500 + (currentRound - 1) * 300;
        float addedScore = 0;

        // 피버 모드일 경우 기본 점수에 1.2배 적용
        if (feverManager != null && feverManager.isFeverMode)
        {
            baseScore *= 1.2f;
        }

        score += baseScore;  // 기본 점수 추가

        // 콤보 보너스 점수 계산
        addedScore = combo * 50f;

        // 피버 모드일 경우 콤보 점수도 1.2배
        if (feverManager != null && feverManager.isFeverMode)
        {
            addedScore *= 1.2f;
        }

        score += addedScore;  // 보너스 점수 추가

        // 점수, 콤보 UI 업데이트
        UpdateScoreText();
        UpdateComboText();

        // 라운드 클리어 조건 체크
        CheckRoundClear();

        // 피버 모드가 아닌 상태에서 콤보가 일정 수치 이상이면 피버 모드 시작
        if (feverManager != null && !feverManager.isFeverMode && combo >= feverCombo)
        {
            PlaySound(feverStartSound, 0.7f);  // 피버 시작 효과음 재생
            feverManager.StartFeverMode(rabbitStack);  // 피버 모드 시작

            BGMPlayer.SetPitch(1.25f);  // BGM 속도 증가
        }
    }

    // 콤보를 초기화하는 함수
    public void ResetCombo()
    {
        combo = 0;             // 콤보 리셋
        feverEndCombo = 0;     // 피버 종료 후 콤보 누적 값도 초기화
        UpdateComboText();     // 콤보 텍스트 UI 업데이트
    }

    // 피버 모드가 끝났을 때 호출되는 함수
    public void OnFeverEnd()
    {
        feverEndCombo += combo;  // 현재 콤보를 피버 종료 누적 콤보에 추가
        combo = 0;               // 콤보 리셋
        UpdateComboText();       // 콤보 텍스트 UI 업데이트

        BGMPlayer.SetPitch(1f);  // BGM 속도를 원래 속도로 복구
    }

    // 라운드 클리어 조건을 확인하는 함수
    private void CheckRoundClear()
    {
        if (score >= roundClearScore)  // 현재 점수가 라운드 클리어 점수 이상이면
        {
            StartCoroutine(ShowClearAndNextScene());  // 클리어 연출 및 다음 씬으로 이동
        }
    }

    // 클리어 연출 후 다음 씬으로 넘어가는 코루틴
    private IEnumerator ShowClearAndNextScene()
    {
        isCleared = true;  // 클리어 상태로 설정

        if (feverManager != null && feverManager.isFeverMode)
        {
            feverManager.EndFeverMode(rabbitStack);  // 피버 모드가 켜져 있으면 종료
        }

        PlaySound(gameClearSound, 0.7f);  // 클리어 사운드 재생

        panel.SetActive(true); // 반투명 패널 활성화

        if (currentRound == 6)  // 마지막 라운드일 경우
        {
            PlayerPrefs.SetInt("PingPongUnlocked", 1);
            PlayerPrefs.Save();

            if (gameClearImage != null)
            {
                gameClearImage.gameObject.SetActive(true);  // 게임 클리어 이미지 표시
                resetButton.SetActive(true);                // 리셋 버튼 표시
            }
        }
        else  // 마지막 라운드가 아닌 경우
        {
            if (clearImage != null)
            {
                clearImage.gameObject.SetActive(true);  // 클리어 이미지 표시
            }
        }

        yield return new WaitForSeconds(3f);  // 3초 대기

        if (currentRound < 6)  // 마지막 라운드가 아닌 경우
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);  // 다음 씬으로 이동
        }
    }

    // 점수 텍스트 UI를 갱신하는 함수
    private void UpdateScoreText()
    {
        scoreText.text = score.ToString("N0");  // 1,000 단위로 콤마 표시
    }

    // 콤보 텍스트 UI를 갱신하는 함수
    private void UpdateComboText()
    {
        comboText.text = (feverEndCombo + combo).ToString();  // 피버 종료 누적 콤보 + 현재 콤보 표시
    }

    // 효과음을 재생하는 함수
    private void PlaySound(AudioClip clip, float volume = 1.0f)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip, volume);  // 지정한 볼륨으로 오디오 클립 재생
        }
    }

    // Reset 버튼 클릭 시 호출되는 함수 (타이틀 화면으로 이동)
    public void OnResetButtonClicked()
    {
        SceneManager.LoadScene("Title");
    }

    // Retry 버튼 클릭 시 호출되는 함수 (현재 씬 다시 로드)
    public void OnRetryButtonClicked()
    {
        Scene currentScene = SceneManager.GetActiveScene();  // 현재 씬 정보 가져오기
        SceneManager.LoadScene(currentScene.name);           // 현재 씬 다시 로드
    }
}
