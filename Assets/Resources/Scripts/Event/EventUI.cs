using UnityEngine;
using UnityEngine.UI;

public class EventUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject eventPanel; 
    [SerializeField] private Button eventPanelButton; 
    [SerializeField] private Text eventText; 
    [SerializeField] private Button closeButton;

    void Awake()
    {
        if (eventPanel != null) eventPanel.SetActive(false);
        if (eventPanelButton != null) eventPanelButton.gameObject.SetActive(false);

        // 버튼 리스너 연결
        if (eventPanelButton != null)
        {
            eventPanelButton.onClick.RemoveAllListeners();
            eventPanelButton.onClick.AddListener(OnClickEventButton);
        }
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(OnClickCloseButton);
        }
    }

    // GameManager.StartDay()에서 호출됨
    public void ActivateEvent(string eventMessage)
    {
        if (eventPanelButton != null)
        {
            eventPanelButton.gameObject.SetActive(true);
        }
        if (eventText != null)
        {
            eventText.text = eventMessage;
        }
    }

    private void OnClickEventButton()
    {
        // 이벤트 버튼 클릭 -> 패널 열기
        if (eventPanel != null)
        {
            eventPanel.SetActive(true);
        }
    }

    private void OnClickCloseButton()
    {
        // 1. 패널 닫기
        if (eventPanel != null)
        {
            eventPanel.SetActive(false);
        }

        // 2. 버튼 비활성화
        if (eventPanelButton != null)
        {
            eventPanelButton.gameObject.SetActive(false);
        }

        // 3. GameManager에 알림 -> NPC 스폰 시작
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnEventFinished();
        }
    }
}