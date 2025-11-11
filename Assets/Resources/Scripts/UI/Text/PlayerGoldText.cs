using UnityEngine;
using UnityEngine.UI;

public class PlayerGoldText : MonoBehaviour
{
    private Text goldText;

    void Awake()
    {
        goldText = GetComponent<Text>();
    }

    void OnEnable()
    {
        GameManager.OnGoldChanged += UpdateText;
        // 시작할 때 현재 골드 값으로 한 번 업데이트
        if (GameManager.Instance != null)
        {
            UpdateText(GameManager.Instance.CurrentGold);
        }
    }

    void OnDisable()
    {
        // 수정: += (중복 구독) -> -= (구독 해제)
        GameManager.OnGoldChanged -= UpdateText;
    }

    private void UpdateText(int changedGoldAmount)
    {
        goldText.text = changedGoldAmount.ToString("N0");
    }
}