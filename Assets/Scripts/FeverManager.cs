using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 피버 모드의 시작과 종료를 관리하는 클래스
public class FeverManager : MonoBehaviour
{
    public RabbitManager rabbitManager;           // 콤보 및 래빗을 관리하는 매니저

    public GameObject feverImage;                 // 피버 모드 시 보여줄 이미지
    public Sprite feverRabbitSprite;              // 피버 모드에서 토끼에 적용할 스프라이트
    public float feverDuration = 2f;              // 피버 모드 지속 시간

    [HideInInspector]
    public bool isFeverMode = false;              // 현재 피버 모드 상태
    private Coroutine feverCoroutine;             // 피버 모드 타이머 코루틴 참조

    private List<GameObject> lastRabbitStack;     // 마지막 피버 모드 시점의 토끼 리스트 저장

    public void StartFeverMode(List<GameObject> rabbitStack)
    {
        if (isFeverMode) return;  // 이미 피버 모드 중이면 무시

        isFeverMode = true;
        lastRabbitStack = new List<GameObject>(rabbitStack);  // 현재 토끼 스택 저장

        if (feverImage != null)
            feverImage.SetActive(true);  // 피버 이미지 표시

        // 각 토끼의 스프라이트를 피버용으로 변경
        foreach (var rabbit in rabbitStack)
        {
            var sr = rabbit.GetComponent<SpriteRenderer>();
            var rabbitComp = rabbit.GetComponent<Rabbit>();
            if (sr != null && rabbitComp != null)
            {
                if (rabbitComp.originalSprite == null)
                    rabbitComp.originalSprite = sr.sprite;  // 원래 스프라이트 저장

                sr.sprite = feverRabbitSprite;  // 피버 스프라이트 적용
            }
        }

        // 기존 피버 코루틴이 있으면 중단
        if (feverCoroutine != null)
            StopCoroutine(feverCoroutine);

        // 새로운 피버 타이머 시작
        feverCoroutine = StartCoroutine(FeverTimer(rabbitStack));
    }

    // 피버 모드 지속 시간을 기다리는 코루틴
    private IEnumerator FeverTimer(List<GameObject> rabbitStack)
    {
        yield return new WaitForSeconds(feverDuration);
        EndFeverMode(rabbitStack);  // 시간 종료 후 피버 모드 해제
    }

    // 씬이 파괴될 때 피버 모드가 남아 있다면 종료 처리
    private void OnDestroy()
    {
        if (isFeverMode && lastRabbitStack != null)
            EndFeverMode(lastRabbitStack);
    }

    // 피버 모드를 종료하는 함수
    public void EndFeverMode(List<GameObject> rabbitStack)
    {
        isFeverMode = false;

        if (feverImage != null)
            feverImage.SetActive(false);  // 피버 이미지 숨기기

        // 각 토끼의 스프라이트를 원래대로 복구
        foreach (var rabbit in rabbitStack)
        {
            if (rabbit == null) continue;

            var sr = rabbit.GetComponent<SpriteRenderer>();
            var rabbitComp = rabbit.GetComponent<Rabbit>();
            if (sr != null && rabbitComp != null && rabbitComp.originalSprite != null)
            {
                sr.sprite = rabbitComp.originalSprite;       // 원래 스프라이트로 복구
                rabbitComp.originalSprite = null;            // 원본 정보 초기화
            }
        }

        // 피버 타이머 정리
        if (feverCoroutine != null)
        {
            StopCoroutine(feverCoroutine);
            feverCoroutine = null;
        }

        // 피버 종료 시 콤보 처리
        if (rabbitManager != null)
            rabbitManager.OnFeverEnd();
    }
}
