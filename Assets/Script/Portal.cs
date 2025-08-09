using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Portal : MonoBehaviour
{
    public float interactionDuration = 3f;  
    public Slider progressBar;              

#if UNITY_EDITOR
    public SceneAsset targetScene; 
#endif

    [SerializeField] private string targetSceneName;

    private float currentProgress = 0f;
    private bool isPlayerInRange = false;
    private bool isInteracting = false;
    private Coroutine interactionCoroutine;

    public float healRate = 20f;

    private Transform player;

    public int currentLevel;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (isPlayerInRange)
        {
            player.GetComponent<HealthController>()?.Heal(healRate * Time.deltaTime);
        }

        if (isPlayerInRange && !isInteracting && Input.GetKey(KeyCode.Space))
        {
            if (interactionCoroutine == null)
                interactionCoroutine = StartCoroutine(HandleInteraction());
        }

        if ((!isPlayerInRange || !Input.GetKey(KeyCode.Space)) && interactionCoroutine != null)
        {
            StopCoroutine(interactionCoroutine);
            interactionCoroutine = null;

            isInteracting = false;
            currentProgress = 0f;

            if (progressBar != null)
            {
                progressBar.value = 0f;
                progressBar.gameObject.SetActive(false);
            }
        }
    }

    IEnumerator HandleInteraction()
    {
        isInteracting = true;
        currentProgress = 0f;

        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(true);
            progressBar.value = 0f;
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
            LoadSceneWithLoadingScreen();
        }

        isInteracting = false;
        if (progressBar != null)
            progressBar.gameObject.SetActive(false);
    }

    public void LoadSceneWithLoadingScreen()
    {
        var playerCoins = player.GetComponent<PlayerCoins>()?.GetCoins() ?? 0;
        UpgradeManager.Instance.SetCoins(UpgradeManager.Instance.GetCoins() + playerCoins);
        UpgradeManager.Instance.SetCurrentLevel(currentLevel + 1);

        if (!string.IsNullOrEmpty(targetSceneName))
        {
            LoadingScreen.sceneName = targetSceneName;
            SceneManager.LoadScene("LoadingScreen");
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (targetScene != null)
        {
            targetSceneName = targetScene.name;
        }
    }
#endif

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerInRange = false;
    }
}
