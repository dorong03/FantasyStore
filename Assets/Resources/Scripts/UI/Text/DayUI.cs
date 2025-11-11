using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class DayUI : MonoBehaviour
{
    private Text dayText;

    void Awake()
    {
        dayText = GetComponent<Text>();
    }

    void OnEnable()
    {
        GameManager.OnDayChanged += UpdateText;
        dayText.text = "1"; 
    }

    void OnDisable()
    {
        GameManager.OnDayChanged -= UpdateText;
    }
    
    private void UpdateText(int newDay)
    {
        dayText.text = $"{newDay}";
    }
}