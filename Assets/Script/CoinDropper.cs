using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CoinDropper : MonoBehaviour
{
    public GameObject coinPrefab;
    public int totalCoins = 10;
    public float dropInterval = 0.3f;
    public float interactionDuration = 3f;
    public float dropRadius = 1.5f;

    private Slider progressBar;
    private float currentProgress = 0f;
    private bool isPlayerInRange = false;
    private bool isDropping = false;
    private Coroutine dropCoroutine;
    private GameObject player;

    void Awake()
    {
        var sliders = FindObjectsOfType<Slider>(true);
        foreach (var s in sliders)
        {
            if (s.name == "Slider")
            {
                progressBar = s;
                break;
            }
        }

        if (progressBar != null)
            progressBar.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isPlayerInRange && !isDropping && Input.GetKey(KeyCode.Space))
        {
            if (dropCoroutine == null)
                dropCoroutine = StartCoroutine(HandleInteraction());
        }

        if ((!isPlayerInRange || !Input.GetKey(KeyCode.Space)) && dropCoroutine != null)
        {
            StopCoroutine(dropCoroutine);
            dropCoroutine = null;

            isDropping = false;

            if (progressBar != null)
                progressBar.gameObject.SetActive(false);

            currentProgress = 0f;
        }
    }

    IEnumerator HandleInteraction()
    {
        isDropping = true;

        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(true);
            progressBar.value = currentProgress / interactionDuration;
        }

        while (currentProgress < interactionDuration)
        {
            if (!isPlayerInRange || !Input.GetKey(KeyCode.Space))
                break;

            currentProgress += Time.deltaTime;

            if (progressBar != null)
                progressBar.value = currentProgress / interactionDuration;

            yield return null;
        }

        if (currentProgress >= interactionDuration)
        {
            StartCoroutine(DropCoins());
        }

        isDropping = false;
        if (progressBar != null)
            progressBar.gameObject.SetActive(false);

        currentProgress = 0f;
    }

    IEnumerator DropCoins()
    {
        while (totalCoins > 0)
        {
            Vector2 randomOffset = Random.insideUnitCircle * dropRadius; 
            Vector3 dropPosition = transform.position + (Vector3)randomOffset;

            Instantiate(coinPrefab, dropPosition, Quaternion.identity);
            totalCoins--;
            yield return new WaitForSeconds(dropInterval);
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            player = other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            player = null;
        }
    }
}
