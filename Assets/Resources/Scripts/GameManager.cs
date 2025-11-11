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

    // GameManager.cs

    private void StartDay()
    {
        npcEncountersToday = 0;
        OnDayChanged?.Invoke(day);
        
        CurrentEventData = null; 
        
        // 💡 1. 첫 번째 날 이벤트 고정 로직 (확률 무시)
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
        
        // 💡 2. 첫날이 아니거나 고정 이벤트가 없을 경우: 70% 확률 이벤트 체크
        if (Random.Range(0f, 1f) <= 0.7f)
        {
            var eventDictionary = DataManager.Instance.EventDictionary;
            if (eventDictionary.Count > 0)
            {
                // 랜덤 이벤트 선택
                List<EventData> allEvents = new List<EventData>(eventDictionary.Values);
                CurrentEventData = allEvents[Random.Range(0, allEvents.Count)];
                
                // 이벤트 UI 활성화 (NPC 스폰 대기)
                if (eventUI != null)
                {
                    eventUI.ActivateEvent(CurrentEventData.Text);
                    return; // NPC 스폰 코루틴을 실행하지 않고 대기
                }
            }
        }
        
        // 이벤트가 발생하지 않았거나 EventUI가 없을 경우 바로 NPC 스폰 시작
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
            StartCoroutine(GoToNextDayCoroutine());
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