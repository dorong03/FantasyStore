using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [SerializeField] private GameObject textPanel;
    [SerializeField] private Text dialogueText;
    
    [SerializeField] private Image visitorImage;
    
    [SerializeField] private Button purchaseButton;
    [SerializeField] private Button cancelButton;
    public bool canBuy = false;

    public TextData dialogue;
    
    [SerializeField] private NpcController npcController;

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

        if (npcController == null)
        {
            Debug.LogError("NpcController 컴포넌트 Visitor 에 설정하기");
        }
        
        if (dialogueText == null)
        {
            Debug.LogError("방문자 대사 채팅 말풍선 게임 인스펙터에 설정하기");
        }

        if (visitorImage == null)
        {
            Debug.LogError("Visitor 이미지 인스펙터에 설정하기");
        }
        
        if (purchaseButton == null)
        {
            Debug.LogError("구매하기 버튼을 인스펙터에 설정해주세요.");
        }
    }

    public void StartDialogueCoroutine(int i)
    {
        StartCoroutine(playDialogueWithID(i));
    }
    
    public IEnumerator playDialogueWithID(int dialogueID)
    {
        canBuy = false;
        purchaseButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(true);
        
        dialogue = DataManager.Instance.GetTextData(dialogueID.ToString());
        
        if (Resources.Load<Sprite>($"Image/Characters/{dialogue.Customer}/Basic") != null)
        {
            visitorImage.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Image/Characters/{dialogue.Customer}/Basic");
        }
        else
        {
            Debug.Log("해당 스프라이트가 없음");
        }

        InitChatBubble();
        yield return StartCoroutine(npcController.Apear(dialogue));
        
        canBuy = true;
        ChatBubble(DataManager.Instance.GetItemData(dialogue.ItemID).BuyText1);

        if (dialogue.selling == false)
        {
            purchaseButton.gameObject.SetActive(true);
        }
    }
    
    private void ChatBubble(string msg)
    {
        textPanel.gameObject.SetActive(true);
        dialogueText.gameObject.SetActive(true);
        dialogueText.text = msg;
    }

    private void InitChatBubble()
    {
        textPanel.gameObject.SetActive(false);
        dialogueText.gameObject.SetActive(false);
        dialogueText.text = "";
    }
    
    private IEnumerator RejectSequence()
    {
        purchaseButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        if (dialogue.rejectSpriteLoaded != null)
        {
            visitorImage.sprite = dialogue.rejectSpriteLoaded;
        }
        ChatBubble(dialogue.rejectDialogue);
        yield return new WaitForSeconds(2f);
        InitChatBubble();
        yield return StartCoroutine(npcController.Disapear(dialogue));
    }

    private IEnumerator AcceptSequence()
    {
        purchaseButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        if (!dialogue.selling)
        {
            if (GameManager.Instance.CheckGold(dialogue.itemPrice))
            {
                GameManager.Instance.RemoveGold(dialogue.itemPrice);
                InventoryManager.Instance.AddItem(dialogue.itemId, dialogue.itemAmount);

                if (dialogue.acceptSpriteLoaded != null)
                {
                    visitorImage.sprite = dialogue.acceptSpriteLoaded;
                }
                ChatBubble(dialogue.acceptDialogue);
            }
            else
            {
                if (dialogue.rejectSpriteLoaded != null)
                {
                    visitorImage.sprite = dialogue.rejectSpriteLoaded;
                }
                ChatBubble(dialogue.rejectDialogue);
            }
        }
        else
        {
            if (dialogue.acceptSpriteLoaded != null)
            {
                visitorImage.sprite = dialogue.acceptSpriteLoaded;
            }
            ChatBubble(dialogue.acceptDialogue);   
        }
        yield return new WaitForSeconds(2f);
        InitChatBubble();
        yield return StartCoroutine(npcController.Disapear(dialogue));

        dialogue = null;
        GameManager.Instance.OnNpcEncounterFinished();
    }
    
    public void OnReject()
    {
        if (dialogue != null)
        {
            StartCoroutine(RejectSequence());
        }
    }
    
    public void OnAccept()
    {
        if (dialogue != null)
        {
            StartCoroutine(AcceptSequence());
        }
    }
    
}
