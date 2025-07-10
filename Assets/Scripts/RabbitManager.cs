using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

// �䳢�� ���� ���¸� �����ϴ� Ŭ����
public class Rabbit : MonoBehaviour
{
    public bool isFlipped = false;          // �䳢�� ������ �������� ����
    public Sprite originalSprite;           // ���� ��������Ʈ �����
}

// �䳢 ��ü ���� �� ���� ���� ������ ����ϴ� Ŭ����
public class RabbitManager : MonoBehaviour
{
    public GameObject[] rabbitPrefabs;      // �䳢 ������ �迭 (6���� ����)

    public Transform centerXPoint;          // �߾� ��ġ ������
    public Transform leftXPoint;            // ���� ��ġ ������
    public Transform rightXPoint;           // ���� ��ġ ������

    public List<GameObject> rabbitStack = new List<GameObject>(); // �߾� �� �䳢 ���
    private float[] yPositions = new float[] { -2.5f, -1.5f, -0.5f, 0.5f, 1.5f, 2.5f, 3.5f, 4.5f }; // �߾� �䳢 Y ��ǥ��

    public float flyDistance = 5f;          // �䳢�� ���ư� �Ÿ�
    public float flyDuration = 0.5f;        // ���ư��� �ð�

    public TextMeshProUGUI scoreText;       // ���� �ؽ�Ʈ UI
    public TextMeshProUGUI comboText;       // �޺� �ؽ�Ʈ UI
    public Image hpBarImage;                 // HP �� UI �̹���

    private float score = 0;                 // ���� ����
    private float combo = 0;                 // ���� �޺�

    private float roundClearScore = 20000;  // ���� ���� Ŭ���� ���� ����

    public float maxHpTime = 60f;            // �ִ� HP �ð�
    private float currentHpTime;             // ���� HP �ð�
    public float penaltyTime = 1f;           // Ʋ���� �� ���Ƽ �ð�

    public FeverManager feverManager;        // �ǹ� ��� �Ŵ��� ����
    private float feverCombo = 15;           // �ǹ� ���� �޺� ����
    private float feverEndCombo = 0;         // �ǹ� ���� �� ���� �޺�

    private AudioSource audioSource;         // ����� �ҽ� ������Ʈ
    public AudioClip correctSound;           // ���� ȿ����
    public AudioClip wrongSound;             // ���� ȿ����
    public AudioClip gameOverSound;          // ���ӿ��� ȿ����
    public AudioClip gameClearSound;         // ���� Ŭ���� ȿ����
    public AudioClip feverStartSound;        // �ǹ� ���� ȿ����

    public Image clearImage;                 // Ŭ���� �̹���
    public Image gameClearImage;             // 6���� Ŭ���� �̹���
    public Image gameOverImage;              // ���� ���� �̹���

    private bool isCleared = false;          // Ŭ���� ���� �÷���

    public int currentRound = 1;             // ���� ���� ��ȣ

    private List<int> chosenColors = new List<int>(); // �̹� ���忡�� ���õ� ���� �ε��� ����Ʈ
    private List<int> leftColors = new List<int>();   // ���� �䳢 ���� ����Ʈ
    private List<int> rightColors = new List<int>();  // ���� �䳢 ���� ����Ʈ

    public GameObject resetButton;           // ���� ��ư ������Ʈ
    public GameObject retryButton;           // ��õ� ��ư ������Ʈ

    public GameObject panel; // ������ �г�

    void Start()
    {
        chosenColors.Clear();   // ���� ����Ʈ �ʱ�ȭ
        rabbitStack.Clear();    // �䳢 ���� �ʱ�ȭ

        audioSource = GetComponent<AudioSource>(); // ����� �ҽ� ������Ʈ �Ҵ�

        roundClearScore = GetRoundClearScoreByRound(currentRound); // ���庰 Ŭ���� ���� ����

        int pairCount = GetPairCountByRound(currentRound); // ���庰 �� ���� ����

        penaltyTime = Mathf.Lerp(1f, 2f, (currentRound - 1) / 5f); // ���Ƽ �ð� ���� (���� ���� �� ����)

        bool unlocked = PlayerPrefs.GetInt("PingPongUnlocked", 0) == 1;

        if (SceneManager.GetActiveScene().name == "PingPong" && unlocked)
        {
            currentRound = 6;
            maxHpTime = 120f;
            roundClearScore = float.MaxValue;
        }

        currentHpTime = maxHpTime;

        float[] yPositionsForSideRabbits = new float[] { -0.2f, 1.8f, 3.8f }; // �¿� �䳢 Y ��ġ �迭

        // �ߺ� ���� �䳢 ���� �ε��� �̱�
        chosenColors.Clear();
        leftColors.Clear();
        rightColors.Clear();

        while (chosenColors.Count < pairCount * 2)
        {
            int randomColor = Random.Range(0, rabbitPrefabs.Length);
            if (!chosenColors.Contains(randomColor))
                chosenColors.Add(randomColor);
        }

        // ���� �䳢 ���� �� ��ġ ����
        for (int i = 0; i < pairCount; i++)
        {
            int leftColorIndex = chosenColors[i];
            leftColors.Add(leftColorIndex);

            Vector3 leftPos = new Vector3(leftXPoint.position.x, yPositionsForSideRabbits[i], leftXPoint.position.z);
            GameObject rabbit = Instantiate(rabbitPrefabs[leftColorIndex], leftPos, Quaternion.identity);
            rabbit.name = rabbitPrefabs[leftColorIndex].name;
        }

        // ���� �䳢 ���� �� ��ġ ����
        for (int i = 0; i < pairCount; i++)
        {
            int rightColorIndex = chosenColors[pairCount + i];
            rightColors.Add(rightColorIndex);

            Vector3 rightPos = new Vector3(rightXPoint.position.x, yPositionsForSideRabbits[i], rightXPoint.position.z);
            GameObject rabbit = Instantiate(rabbitPrefabs[rightColorIndex], rightPos, Quaternion.identity);
            rabbit.name = rabbitPrefabs[rightColorIndex].name;
        }

        // �߾� �� �䳢 8���� ����
        for (int i = 0; i < 8; i++)
        {
            AddNewRabbitAtTop();
        }

        UpdatePositions();     // �䳢 ��ġ ������Ʈ
    }

    void Update()
    {
        if (isCleared) return;  // Ŭ���� �� ���� ����

        float decreaseRate = GetHpDecreaseRateByRound(currentRound);  // ���庰 HP ���� �ӵ�
        currentHpTime -= Time.deltaTime * decreaseRate;               // HP ����
        currentHpTime = Mathf.Clamp(currentHpTime, 0, maxHpTime);     // HP ���� ����
        UpdateHpBar();                                                // HP �� ����

        if (currentHpTime <= 0)
        {
            if (SceneManager.GetActiveScene().name == "PingPong")
            {
                StartCoroutine(ShowClearAndNextScene());
            }
            else
            {
                // ���� ���� ó��
                GameOver();
            }
        }
    }

    // ���庰 �¿� �䳢 �� ���� ��ȯ
    private int GetPairCountByRound(int round)
    {
        if (round == 1) return 1;
        else if (round >= 2 && round <= 3) return 2;
        else if (round >= 4 && round <= 6) return 3;
        else return 1;
    }

    // ���庰 HP ���� �ӵ� ��ȯ
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

    // ���庰 Ŭ���� ���� ���� ��ȯ
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

    // HP �� UI ������Ʈ
    private void UpdateHpBar()
    {
        if (hpBarImage != null)
        {
            hpBarImage.fillAmount = currentHpTime / maxHpTime;
        }
    }

    // ���� ���� ó��
    private void GameOver()
    {
        isCleared = true;  // ���� ���� ���� ����

        if (feverManager != null && feverManager.isFeverMode)
        {
            feverManager.EndFeverMode(rabbitStack);  // �ǹ� ��� ����
        }

        PlaySound(gameOverSound, 0.5f); // ���� ���� ȿ���� ���

        panel.SetActive(true); // ������ �г� Ȱ��ȭ

        if (gameOverImage != null)
        {
            gameOverImage.gameObject.SetActive(true);  // ���� ���� �̹��� ǥ��
            retryButton.SetActive(true);                // ��õ� ��ư Ȱ��ȭ
        }
    }

    // �ϴܿ� �ִ� �䳢�� ��(-1) �Ǵ� ��(1) �������� ������ �Լ�
    public void SendBottomRabbit(int direction)
    {
        if (isCleared) return;  // Ŭ���� ���¸� ����

        if (rabbitStack.Count == 0) return; // �䳢�� ������ ����

        GameObject bottomRabbit = rabbitStack[0]; // �� �Ʒ� �䳢 ����

        int bottomRabbitIndex = GetPrefabIndex(bottomRabbit); // �ش� �䳢�� ���� �ε��� ��������

        // �䳢�� ���� Rabbit ������Ʈ���� ������ ���� Ȯ��
        Rabbit rabbitComp = bottomRabbit.GetComponent<Rabbit>();
        bool isFlipped = rabbitComp != null && rabbitComp.isFlipped;

        bool isCorrectDirection; // �䳢�� �°� ���������� ���� �Ǻ� ����


        if (feverManager != null && feverManager.isFeverMode)
        {
            // �ǹ� ����� ��� ���� ������� �׻� �������� ó��
            isCorrectDirection = true;
        }
        else if (isFlipped)
        {
            // �䳢�� �¿찡 ������ �ִ� ������ ��
            // ���� �����̸� ���� ������ ���� �׷쿡 ���ؾ� ����
            // ������ �����̸� ���� ���� ���� �׷쿡 ���ؾ� ����
            isCorrectDirection = (direction == -1 && rightColors.Contains(bottomRabbitIndex)) || // �� ������
                                 (direction == 1 && leftColors.Contains(bottomRabbitIndex));     // �� ����
        }
        else
        {
            // �䳢�� �⺻ ����(�������� ���� ����)�� ��
            // ���� �����̸� ���� ���� �׷쿡 ���ؾ� ����
            // ������ �����̸� ������ ���� �׷쿡 ���ؾ� ����
            isCorrectDirection = (direction == -1 && leftColors.Contains(bottomRabbitIndex)) ||  // �� ����
                                 (direction == 1 && rightColors.Contains(bottomRabbitIndex));    // �� ������
        }

        if (isCorrectDirection)
        {
            PlaySound(correctSound, 1.45f); // ���� ���� ���
            IncreaseScoreAndCombo();        // ���� �� �޺� ����
        }
        else
        {
            PlaySound(wrongSound, 1.75f);   // ���� ���� ���

            combo = 0;                     // �޺� �ʱ�ȭ
            feverEndCombo = 0;             // �ǹ� ���� �޺��� �ʱ�ȭ
            UpdateComboText();             // �޺� UI ����

            currentHpTime -= penaltyTime;  // ü��(�ð�) ����
            currentHpTime = Mathf.Clamp(currentHpTime, 0, maxHpTime); // ü�� �ּ� 0, �ִ밪 ����
            UpdateHpBar();                 // ü�� �� UI ����
        }

        rabbitStack.RemoveAt(0);   // ���ÿ��� �ϴ� �䳢 ����
        AddNewRabbitAtTop();       // �� �䳢�� ���� �� ���� �߰�
        UpdatePositions();         // �䳢�� ��ġ ����

        StartCoroutine(FlyAwayAndDestroy(bottomRabbit, direction)); // �䳢�� ���ư��鼭 �ı��Ǵ� �ڷ�ƾ ����
    }

    // ���޹��� �䳢 ���� ������Ʈ�� ������ �迭 �� �ε����� ��ȯ�ϴ� �Լ�
    // rabbitPrefabs �迭���� �䳢 �̸��� ��ġ�ϴ� �������� ã�� �ε����� ����
    private int GetPrefabIndex(GameObject rabbit)
    {
        for (int i = 0; i < rabbitPrefabs.Length; i++)
        {
            // "(Clone)" �����ϰ� ���鵵 ������ �̸��� ������ �̸� ��
            if (rabbitPrefabs[i].name == rabbit.name.Replace("(Clone)", "").Trim())
            {
                return i;  // ��ġ�ϴ� ������ �ε��� ��ȯ
            }
        }
        return -1;  // �� ã���� -1 ��ȯ
    }

    // �䳢�� �¿� �������� ���� �����鼭 ���� �ð� �� �ı��ϴ� �ڷ�ƾ
    private IEnumerator FlyAwayAndDestroy(GameObject rabbit, int direction)
    {
        float elapsed = 0f;                       // ��� �ð� �ʱ�ȭ
        Vector3 startPos = rabbit.transform.position;  // ���� ��ġ ����
        Vector3 targetPos = startPos + new Vector3(flyDistance * direction, 0, 0);  // ��ǥ ��ġ ��� (��/�� �������� �̵�)

        // flyDuration �ð� ���� ��ġ�� ���� �����ؼ� �ε巴�� �̵�
        while (elapsed < flyDuration)
        {
            // ���� ��ġ�� ���۰� ��ǥ ��ġ ���̿��� ������ �°� ����Ͽ� �̵�
            rabbit.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / flyDuration);
            elapsed += Time.deltaTime;  // �ð� ����
            yield return null;          // �� ������ ���
        }

        // ���� ��ġ ���� (��Ȯ�ϰ� ��ǥ ��ġ�� �̵�)
        rabbit.transform.position = targetPos;

        // �䳢 ������Ʈ �ı� (������ ����)
        Destroy(rabbit);
    }

    // ���� �� ���� ���ο� �䳢�� �����Ͽ� �߰��ϴ� �Լ�
    private void AddNewRabbitAtTop()
    {
        // chosenColors ����Ʈ���� �������� �ϳ� ���� (�䳢 ���� �ε���)
        int chosenIndex = chosenColors[Random.Range(0, chosenColors.Count)];

        // ������ �ε����� �ش��ϴ� �������� �����ͼ� �ν��Ͻ� ����
        GameObject prefab = rabbitPrefabs[chosenIndex];
        GameObject newRabbit = Instantiate(prefab, Vector3.zero, Quaternion.identity);

        // ������ �䳢�� ��������Ʈ ������Ʈ ��������
        SpriteRenderer sr = newRabbit.GetComponent<SpriteRenderer>();

        // Rabbit ������Ʈ �������� (������ ���� �߰�)
        Rabbit rabbitComp = newRabbit.GetComponent<Rabbit>();

        if (rabbitComp == null)
            rabbitComp = newRabbit.AddComponent<Rabbit>();

        if (sr != null && rabbitComp != null)
        {
            rabbitComp.originalSprite = sr.sprite; // ���� ��������Ʈ ����

            // �ǹ� ����� ���� �䳢 ��������Ʈ�� �ǹ� ���� ��������Ʈ�� ��ü
            if (feverManager != null && feverManager.isFeverMode)
            {
                sr.sprite = feverManager.feverRabbitSprite;
            }
        }

        // ���� ���忡 ���� ������ Ȯ���� 5%���� 15%���� ���� ����
        float flipProbability = Mathf.Lerp(0.05f, 0.15f, (currentRound - 1) / 5f);

        // �ǹ� ��尡 �ƴ� ���� ������ ���� ����
        if (feverManager == null || !feverManager.isFeverMode)
        {
            if (Random.value < flipProbability)
            {
                newRabbit.transform.rotation = Quaternion.Euler(0, 0, 180);  // 180�� ȸ��
                rabbitComp.isFlipped = true;  // ������ ���� ǥ��
            }
        }

        newRabbit.tag = "Rabbit";  // �±� ����

        rabbitStack.Add(newRabbit);  // �䳢 ���ÿ� �� �䳢 �߰�
    }

    // �䳢 ���ÿ� �ִ� �䳢���� ��ġ�� ������Ʈ�ϴ� �Լ�
    private void UpdatePositions()
    {
        for (int i = 0; i < rabbitStack.Count && i < yPositions.Length; i++)
        {
            // �߾� x ��ǥ�� �̸� ���� yPositions �迭�� y ��ǥ�� �̿��� ��ġ ����
            Vector3 newPos = new Vector3(centerXPoint.position.x, yPositions[i], 0);
            rabbitStack[i].transform.position = newPos;

            SpriteRenderer sr = rabbitStack[i].GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                // �Ʒ��� �ִ� �䳢�ϼ��� ���� sortingOrder�� �ο��ؼ� �տ� ���̵��� ����
                sr.sortingOrder = rabbitStack.Count - i;
            }
        }
    }

    // ������ �޺��� ������Ű�� �Լ�
    private void IncreaseScoreAndCombo()
    {
        combo += 1;

        // �⺻ ���� ��� (���忡 ���� ����)
        float baseScore = 1500 + (currentRound - 1) * 300;
        float addedScore = 0;

        // �ǹ� ����� ��� �⺻ ������ 1.2�� ����
        if (feverManager != null && feverManager.isFeverMode)
        {
            baseScore *= 1.2f;
        }

        score += baseScore;  // �⺻ ���� �߰�

        // �޺� ���ʽ� ���� ���
        addedScore = combo * 50f;

        // �ǹ� ����� ��� �޺� ������ 1.2��
        if (feverManager != null && feverManager.isFeverMode)
        {
            addedScore *= 1.2f;
        }

        score += addedScore;  // ���ʽ� ���� �߰�

        // ����, �޺� UI ������Ʈ
        UpdateScoreText();
        UpdateComboText();

        // ���� Ŭ���� ���� üũ
        CheckRoundClear();

        // �ǹ� ��尡 �ƴ� ���¿��� �޺��� ���� ��ġ �̻��̸� �ǹ� ��� ����
        if (feverManager != null && !feverManager.isFeverMode && combo >= feverCombo)
        {
            PlaySound(feverStartSound, 0.7f);  // �ǹ� ���� ȿ���� ���
            feverManager.StartFeverMode(rabbitStack);  // �ǹ� ��� ����

            BGMPlayer.SetPitch(1.25f);  // BGM �ӵ� ����
        }
    }

    // �޺��� �ʱ�ȭ�ϴ� �Լ�
    public void ResetCombo()
    {
        combo = 0;             // �޺� ����
        feverEndCombo = 0;     // �ǹ� ���� �� �޺� ���� ���� �ʱ�ȭ
        UpdateComboText();     // �޺� �ؽ�Ʈ UI ������Ʈ
    }

    // �ǹ� ��尡 ������ �� ȣ��Ǵ� �Լ�
    public void OnFeverEnd()
    {
        feverEndCombo += combo;  // ���� �޺��� �ǹ� ���� ���� �޺��� �߰�
        combo = 0;               // �޺� ����
        UpdateComboText();       // �޺� �ؽ�Ʈ UI ������Ʈ

        BGMPlayer.SetPitch(1f);  // BGM �ӵ��� ���� �ӵ��� ����
    }

    // ���� Ŭ���� ������ Ȯ���ϴ� �Լ�
    private void CheckRoundClear()
    {
        if (score >= roundClearScore)  // ���� ������ ���� Ŭ���� ���� �̻��̸�
        {
            StartCoroutine(ShowClearAndNextScene());  // Ŭ���� ���� �� ���� ������ �̵�
        }
    }

    // Ŭ���� ���� �� ���� ������ �Ѿ�� �ڷ�ƾ
    private IEnumerator ShowClearAndNextScene()
    {
        isCleared = true;  // Ŭ���� ���·� ����

        if (feverManager != null && feverManager.isFeverMode)
        {
            feverManager.EndFeverMode(rabbitStack);  // �ǹ� ��尡 ���� ������ ����
        }

        PlaySound(gameClearSound, 0.7f);  // Ŭ���� ���� ���

        panel.SetActive(true); // ������ �г� Ȱ��ȭ

        if (currentRound == 6)  // ������ ������ ���
        {
            PlayerPrefs.SetInt("PingPongUnlocked", 1);
            PlayerPrefs.Save();

            if (gameClearImage != null)
            {
                gameClearImage.gameObject.SetActive(true);  // ���� Ŭ���� �̹��� ǥ��
                resetButton.SetActive(true);                // ���� ��ư ǥ��
            }
        }
        else  // ������ ���尡 �ƴ� ���
        {
            if (clearImage != null)
            {
                clearImage.gameObject.SetActive(true);  // Ŭ���� �̹��� ǥ��
            }
        }

        yield return new WaitForSeconds(3f);  // 3�� ���

        if (currentRound < 6)  // ������ ���尡 �ƴ� ���
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);  // ���� ������ �̵�
        }
    }

    // ���� �ؽ�Ʈ UI�� �����ϴ� �Լ�
    private void UpdateScoreText()
    {
        scoreText.text = score.ToString("N0");  // 1,000 ������ �޸� ǥ��
    }

    // �޺� �ؽ�Ʈ UI�� �����ϴ� �Լ�
    private void UpdateComboText()
    {
        comboText.text = (feverEndCombo + combo).ToString();  // �ǹ� ���� ���� �޺� + ���� �޺� ǥ��
    }

    // ȿ������ ����ϴ� �Լ�
    private void PlaySound(AudioClip clip, float volume = 1.0f)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip, volume);  // ������ �������� ����� Ŭ�� ���
        }
    }

    // Reset ��ư Ŭ�� �� ȣ��Ǵ� �Լ� (Ÿ��Ʋ ȭ������ �̵�)
    public void OnResetButtonClicked()
    {
        SceneManager.LoadScene("Title");
    }

    // Retry ��ư Ŭ�� �� ȣ��Ǵ� �Լ� (���� �� �ٽ� �ε�)
    public void OnRetryButtonClicked()
    {
        Scene currentScene = SceneManager.GetActiveScene();  // ���� �� ���� ��������
        SceneManager.LoadScene(currentScene.name);           // ���� �� �ٽ� �ε�
    }
}
