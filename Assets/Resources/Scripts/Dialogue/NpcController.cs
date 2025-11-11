using System.Collections;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    [SerializeField] private Transform visitorStartPosition;
    [SerializeField] private Transform visitorEndPosition;
    [SerializeField] private Transform thiefStartPosition;
    [SerializeField] private Transform thiefEndPosition;
    [SerializeField] private float timeBetweenPosition = 3f;

    private GameObject visitorGameObject;

    void Awake()
    {
        visitorGameObject = this.gameObject;
        visitorGameObject.SetActive(false);
    }

    public IEnumerator Apear(TextData dialogue)
    {
        visitorGameObject.SetActive(true);
        float elapsedTime = 0f;
        Vector2 startPos;
        Vector2 endPos;

        if (dialogue.Customer.Contains("Bandit"))
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

    public IEnumerator Disapear(TextData dialogue)
    {
        float elapsedTime = 0f;
        Vector2 startPos;
        Vector2 endPos;

        if (dialogue.Customer.Contains("Bandit"))
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
    }
}
