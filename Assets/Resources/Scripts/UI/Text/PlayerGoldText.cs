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
    }

    void OnDisable()
    {
        GameManager.OnGoldChanged += UpdateText;
    }

    private void UpdateText(int changedGoldAmount)
    {
        goldText.text = changedGoldAmount.ToString();
    }
}
