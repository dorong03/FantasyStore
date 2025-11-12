using System;
using UnityEngine;
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
    
    public int CurrentGold { get; private set; }

    public static event Action<int> OnGoldChanged;
    public static event Action<int> OnDayChanged;

    // ✅ 하루 전환 중복 방지용 플래그
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
    }

    void Start()
    {
        InitGold(2000);
        StartDay();
    }

    private void StartDay()
    {
        npcEncountersToday = 0;
        isDayChanging = false; // 새로운 하루 시작 시 초기화
        OnDayChanged?.Invoke(day);
        
        CurrentEventData = null; 
        
        // 💡 1. 첫 번째 날 이벤트 고정 로직
        if (day == 1)
        {
            var eventDictionary = DataManager.Instance.EventDictionary;
            if (eventDictionary.ContainsKey("100"))
            {
                CurrentEventData = eventDictionary["100"];
                if (eventUI != null)
                {
                    eventUI.ActivateEvent(CurrentEventData.Text);
                    return; // NPC 스폰 대기
                }
            }
        }
        
        // 💡 2. 랜덤 이벤트 70% 확률
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

        // ✅ 하루에 지정된 NPC 수를 모두 만났을 때만 다음날로 이동
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
        if (isDayChanging) yield break; // ✅ 이미 하루 이동 중이면 중단
        isDayChanging = true;

        yield return new WaitForSeconds(10f); 
        
        day++;
        StartDay();
    }
    
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
