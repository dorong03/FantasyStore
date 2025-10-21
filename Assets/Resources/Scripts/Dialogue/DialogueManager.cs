using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    
    [Tooltip("손님 출발 지점")]
    [SerializeField] Transform visitorStartPosition;
    [Tooltip("손님 도착 지점")]
    [SerializeField] Transform visitorEndPosition;
    [Tooltip("도둑 출발 지점")]
    [SerializeField] Transform thiefStartPosition;
    [Tooltip("도둑 도착 지점")]
    [SerializeField] Transform thiefEndPosition;
    [Tooltip("도착까지 걸리는 시간")]
    [SerializeField] private float timeBetweenPosition = 3f;

    [SerializeField] private GameObject textPanel;
    [SerializeField] private Text dialogueText;
    [SerializeField] private GameObject visitorGameObject;
    [SerializeField] private Image visitorImagae;
    
    [SerializeField] private Button purchaseButton;
    [SerializeField] private Button cancelButton;
    public bool canBuy = false;

    public DialogueData dialogue;

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
        if (visitorGameObject == null)
        {
            Debug.LogError("Visitor 게임 인스펙터에 설정하기");
        }
        
        if (dialogueText == null)
        {
            Debug.LogError("방문자 대사 채팅 말풍선 게임 인스펙터에 설정하기");
        }

        if (visitorImagae == null)
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
        
        dialogue = DataManager.Instance.GetDialogue(dialogueID);
        if (dialogue.basicSpriteLoaded != null)
        {
            visitorGameObject.GetComponent<Image>().sprite = dialogue.basicSpriteLoaded;
        }

        InitChatBubble();
        yield return StartCoroutine(VisitorApear());
        canBuy = true;
        ChatBubble(dialogue.dialogue);

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
    
    private IEnumerator VisitorApear()
    {
        visitorGameObject.SetActive(true);
        float elapsedTime = 0f;
        Vector2 startPos;
        Vector2 endPos;
        if (dialogue != null && dialogue.basicSprite.Contains("thief"))
        {
            startPos = thiefStartPosition.position;
            endPos = thiefEndPosition.position;

        }
        else
        {
            startPos = visitorStartPosition.position;
            endPos = visitorEndPosition.position;
        }
        while (elapsedTime < timeBetweenPosition)
        {
            visitorGameObject.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / timeBetweenPosition);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        visitorGameObject.transform.position = endPos;
        yield return new WaitForSeconds(0.3f);
    }

    private IEnumerator VisitorDisapear()
    {

        InitChatBubble();
        float elapsedTime = 0f;
        
        Vector2 startPos;
        Vector2 endPos;
        if (dialogue != null && dialogue.basicSprite.Contains("thief"))
        {
             endPos = thiefStartPosition.position;
             startPos = thiefEndPosition.position;

        }
        else
        {
            endPos = visitorStartPosition.position;
            startPos = visitorEndPosition.position;
        }

        while (elapsedTime < timeBetweenPosition)
        {
            visitorGameObject.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / timeBetweenPosition);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        visitorGameObject.transform.position = endPos;
        visitorGameObject.SetActive(false);
        dialogue = null;

        GameManager.Instance.OnNpcEncounterFinished();
    }
    
    private IEnumerator RejectSequence()
    {
        purchaseButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        if (dialogue.rejectSpriteLoaded != null)
        {
            visitorImagae.sprite = dialogue.rejectSpriteLoaded;
        }
        ChatBubble(dialogue.rejectDialogue);
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(VisitorDisapear());
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
                    visitorImagae.sprite = dialogue.acceptSpriteLoaded;
                }
                ChatBubble(dialogue.acceptDialogue);
            }
            else
            {
                if (dialogue.rejectSpriteLoaded != null)
                {
                    visitorImagae.sprite = dialogue.rejectSpriteLoaded;
                }
                ChatBubble(dialogue.rejectDialogue);
            }
        }
        else
        {
            if (dialogue.acceptSpriteLoaded != null)
            {
                visitorImagae.sprite = dialogue.acceptSpriteLoaded;
            }
            ChatBubble(dialogue.acceptDialogue);   
        }
        yield return new WaitForSeconds(2f);
        
        yield return StartCoroutine(VisitorDisapear());
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
