using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �ǹ� ����� ���۰� ���Ḧ �����ϴ� Ŭ����
public class FeverManager : MonoBehaviour
{
    public RabbitManager rabbitManager;           // �޺� �� ������ �����ϴ� �Ŵ���

    public GameObject feverImage;                 // �ǹ� ��� �� ������ �̹���
    public Sprite feverRabbitSprite;              // �ǹ� ��忡�� �䳢�� ������ ��������Ʈ
    public float feverDuration = 2f;              // �ǹ� ��� ���� �ð�

    [HideInInspector]
    public bool isFeverMode = false;              // ���� �ǹ� ��� ����
    private Coroutine feverCoroutine;             // �ǹ� ��� Ÿ�̸� �ڷ�ƾ ����

    private List<GameObject> lastRabbitStack;     // ������ �ǹ� ��� ������ �䳢 ����Ʈ ����

    public void StartFeverMode(List<GameObject> rabbitStack)
    {
        if (isFeverMode) return;  // �̹� �ǹ� ��� ���̸� ����

        isFeverMode = true;
        lastRabbitStack = new List<GameObject>(rabbitStack);  // ���� �䳢 ���� ����

        if (feverImage != null)
            feverImage.SetActive(true);  // �ǹ� �̹��� ǥ��

        // �� �䳢�� ��������Ʈ�� �ǹ������� ����
        foreach (var rabbit in rabbitStack)
        {
            var sr = rabbit.GetComponent<SpriteRenderer>();
            var rabbitComp = rabbit.GetComponent<Rabbit>();
            if (sr != null && rabbitComp != null)
            {
                if (rabbitComp.originalSprite == null)
                    rabbitComp.originalSprite = sr.sprite;  // ���� ��������Ʈ ����

                sr.sprite = feverRabbitSprite;  // �ǹ� ��������Ʈ ����
            }
        }

        // ���� �ǹ� �ڷ�ƾ�� ������ �ߴ�
        if (feverCoroutine != null)
            StopCoroutine(feverCoroutine);

        // ���ο� �ǹ� Ÿ�̸� ����
        feverCoroutine = StartCoroutine(FeverTimer(rabbitStack));
    }

    // �ǹ� ��� ���� �ð��� ��ٸ��� �ڷ�ƾ
    private IEnumerator FeverTimer(List<GameObject> rabbitStack)
    {
        yield return new WaitForSeconds(feverDuration);
        EndFeverMode(rabbitStack);  // �ð� ���� �� �ǹ� ��� ����
    }

    // ���� �ı��� �� �ǹ� ��尡 ���� �ִٸ� ���� ó��
    private void OnDestroy()
    {
        if (isFeverMode && lastRabbitStack != null)
            EndFeverMode(lastRabbitStack);
    }

    // �ǹ� ��带 �����ϴ� �Լ�
    public void EndFeverMode(List<GameObject> rabbitStack)
    {
        isFeverMode = false;

        if (feverImage != null)
            feverImage.SetActive(false);  // �ǹ� �̹��� �����

        // �� �䳢�� ��������Ʈ�� ������� ����
        foreach (var rabbit in rabbitStack)
        {
            if (rabbit == null) continue;

            var sr = rabbit.GetComponent<SpriteRenderer>();
            var rabbitComp = rabbit.GetComponent<Rabbit>();
            if (sr != null && rabbitComp != null && rabbitComp.originalSprite != null)
            {
                sr.sprite = rabbitComp.originalSprite;       // ���� ��������Ʈ�� ����
                rabbitComp.originalSprite = null;            // ���� ���� �ʱ�ȭ
            }
        }

        // �ǹ� Ÿ�̸� ����
        if (feverCoroutine != null)
        {
            StopCoroutine(feverCoroutine);
            feverCoroutine = null;
        }

        // �ǹ� ���� �� �޺� ó��
        if (rabbitManager != null)
            rabbitManager.OnFeverEnd();
    }
}
