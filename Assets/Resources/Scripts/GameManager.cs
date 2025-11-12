using System;
using UnityEngine;
using UnityEngine.UI; // Text 컴포넌트를 사용하기 위해 추가
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [SerializeField] private int day = 1;
    [SerializeField] private int npcsPerDay = 5;
    private int npcEncountersToday = 0;
    
    [SerializeField] private float minRandomSpawnDelay = 3f; 
    [SerializeField] private float maxRandomSpawnDelay = 6f;

    // 이벤트 관련 필드
    public EventData CurrentEventData { get; private set; } = null;
    [SerializeField] private EventUI eventUI; 
    
    // 💡 하루 전환 연출 관련 필드 추가
    [Header("Day Transition Settings")]
    [SerializeField] private GameObject changeDayPanel; // 패널 GameObject (Canvas Group 필수)
    [SerializeField] private Text dayTextComponent; // Day # 텍스트 컴포넌트
    
    public int CurrentGold { get; private set; }

    public static event Action<int> OnGoldChanged;
    public static event Action<int> OnDayChanged;

    private bool isDayChanging = false;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (changeDayPanel != null && changeDayPanel.GetComponent<CanvasGroup>() == null)
        {
            Debug.LogError("ChangeDayPanel에는 페이드 인/아웃을 위해 CanvasGroup 컴포넌트가 필요합니다.");
        }
        if (changeDayPanel != null)
        {
            changeDayPanel.SetActive(false); // 시작 시 비활성화
        }
    }

    void Start()
    {
        InitGold(2000);
        StartDay();
    }

    private void StartDay()
    {
        npcEncountersToday = 0;
        isDayChanging = false;
        OnDayChanged?.Invoke(day);
        
        CurrentEventData = null; 
        
        // 1. 첫 번째 날 이벤트 고정 로직
        if (day == 1)
        {
            var eventDictionary = DataManager.Instance.EventDictionary;
            if (eventDictionary.ContainsKey("100"))
            {
                CurrentEventData = eventDictionary["100"];
                if (eventUI != null)
                {
                    eventUI.ActivateEvent(CurrentEventData.Text);
                    return;
                }
            }
        }
        
        // 2. 랜덤 이벤트 70% 확률
        if (Random.Range(0f, 1f) <= 0.7f)
        {
            var eventDictionary = DataManager.Instance.EventDictionary;
            if (eventDictionary.Count > 0)
            {
                List<EventData> allEvents = new List<EventData>(eventDictionary.Values);
                CurrentEventData = allEvents[Random.Range(0, allEvents.Count)];
                
                if (eventUI != null)
                {
                    eventUI.ActivateEvent(CurrentEventData.Text);
                    return;
                }
            }
        }
        
        StartCoroutine(SpawnNextNpcCoroutine());
    }
    
    public void OnEventFinished()
    {
        StartCoroutine(SpawnNextNpcCoroutine());
    }
    
    public void OnNpcEncounterFinished()
    {
        npcEncountersToday++;

        if (npcEncountersToday >= npcsPerDay)
        {
            if (!isDayChanging)
            {
                StartCoroutine(GoToNextDayCoroutine());
            }
        }
        else
        {
            StartCoroutine(SpawnNextNpcCoroutine());
        }
    }
    
    private IEnumerator SpawnNextNpcCoroutine()
    {
        float delay = Random.Range(minRandomSpawnDelay, maxRandomSpawnDelay);
        yield return new WaitForSeconds(delay);

        // 1일차 고정 대화 로직 유지
        if (day == 1)
        {
            if (npcEncountersToday == 0)
            {
                DialogueManager.Instance.StartDialogueCoroutine(9);
            }
            else if (npcEncountersToday == 1)
            {
                DialogueManager.Instance.StartDialogueCoroutine(7);
            }
            else
            {
                int nextDialogueID = Random.Range(0, DataManager.Instance.TextDictionary.Count);
                DialogueManager.Instance.StartDialogueCoroutine(nextDialogueID);
            }
        }
        else
        {
            int nextDialogueID = Random.Range(0, DataManager.Instance.TextDictionary.Count);
            DialogueManager.Instance.StartDialogueCoroutine(nextDialogueID);
        }
    }

    private IEnumerator GoToNextDayCoroutine()
    {
        if (isDayChanging) yield break;
        isDayChanging = true;

        // 💡 새로운 연출 로직 실행
        yield return StartCoroutine(FadeAndAnimateDayText(day + 1));
        
        day++;
        StartDay();
    }
    
    // 💡 새로운 하루 전환 연출 코루틴 (Day 텍스트 애니메이션 추가)
    private IEnumerator FadeAndAnimateDayText(int nextDay)
    {
        const float FADE_TIME = 0.5f;
        const float DISPLAY_TIME = 1.0f; // Day # 표시 시간
        
        if (changeDayPanel == null || dayTextComponent == null)
        {
            yield return new WaitForSeconds(1f); 
            yield break;
        }

        CanvasGroup canvasGroup = changeDayPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            yield return new WaitForSeconds(1f); 
            yield break;
        }

        changeDayPanel.SetActive(true);
        float elapsedTime = 0f;

        // 1. 페이드 인 (0.5초 동안 Alpha 0 -> 1)
        while (elapsedTime < FADE_TIME)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / FADE_TIME);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;
        
        // 2. 텍스트 연출 (타자기 효과)
        dayTextComponent.text = "";
        string fullText = $"D A Y {nextDay}";
        elapsedTime = 0f;

        // 1초 동안 타자기 효과로 텍스트를 채움
        while (elapsedTime < DISPLAY_TIME)
        {
            int charCount = Mathf.RoundToInt((elapsedTime / DISPLAY_TIME) * fullText.Length);
            dayTextComponent.text = fullText.Substring(0, charCount);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        dayTextComponent.text = fullText; // 텍스트를 완전히 채움
        
        // 3. 1초 대기
        yield return new WaitForSeconds(1f);

        // 4. 페이드 아웃 (0.5초 동안 Alpha 1 -> 0)
        elapsedTime = 0f;
        while (elapsedTime < FADE_TIME)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / FADE_TIME);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;
        changeDayPanel.SetActive(false);
    }

    // 골드 관련 메서드 유지
    private void InitGold(int amount)
    {
        SetGold(amount);
    }

    private void SetGold(int amount)
    {
        CurrentGold = amount;
        OnGoldChanged?.Invoke(CurrentGold);
    }

    public void AddGold(int amount)
    {
        CurrentGold += amount;
        OnGoldChanged?.Invoke(CurrentGold);
    }

    public bool CheckGold(int amount)
    {
        return CurrentGold >= amount;
    }

    public void RemoveGold(int amount)
    {
        CurrentGold -= amount;
        OnGoldChanged?.Invoke(CurrentGold);
    }
}