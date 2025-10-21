using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [SerializeField] private int day = 1;
    [SerializeField] private int npcsPerDay = 5;
    private int npcEncountersToday = 0;
    
    [SerializeField] private float minRandomSpawnDelay = 3f; 
    [SerializeField] private float maxRandomSpawnDelay = 6f;
    
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

    private void StartDay()
    {
        npcEncountersToday = 0;
        OnDayChanged?.Invoke(day);
        StartCoroutine(SpawnNextNpcCoroutine());
    }
    
    public void OnNpcEncounterFinished()
    {
        npcEncountersToday++;
        Debug.Log($"오늘 {npcEncountersToday}/{npcsPerDay} 번째 손님 퇴장");

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
        float randomDelay = Random.Range(minRandomSpawnDelay, maxRandomSpawnDelay);
        yield return new WaitForSeconds(randomDelay);

        if (day == 1)
        {
            // 1일차 첫번째, 두번째 손님 고정 (9번,7번 대화)
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
                int nextDialogueID = Random.Range(0, DataManager.Instance.DialogueDataBase.Count);
                DialogueManager.Instance.StartDialogueCoroutine(nextDialogueID);
            }
        }
        else
        {
            int nextDialogueID = Random.Range(0, DataManager.Instance.DialogueDataBase.Count);
            DialogueManager.Instance.StartDialogueCoroutine(nextDialogueID);
        }
        
    }

    private IEnumerator GoToNextDayCoroutine()
    {
        Debug.Log("오늘 장사 끝");
        yield return new WaitForSeconds(10f); 
        
        day++;
        StartDay();
    }
    
    private void InitGold()
    {
        SetGold(0);
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