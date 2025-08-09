using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class HealthController : MonoBehaviour
{
    public Image fillImage;

    public float maxHealth = 10000f;
    public float currentHealth;

    public float decayRate = 5f; 
    public float timePass = 0;
    public float factor = 0.03f;

    public Light2D playerLight;
    public float maxLightIntensity = 1f;
    public float minLightIntensity = 0.0f;

    public float maxLightRadius = 2f;
    public float minLightRadius = 0f;

    public Animator animator;
    public float deathDelay = 2f; 

    private bool isDead = false;

    public AudioSource audioSource;
    public AudioClip[] deathSounds;

    private void PlayRandomSound(AudioClip[] clips)
    {
        if (audioSource == null || clips == null || clips.Length == 0) return;
        int index = Random.Range(0, clips.Length);
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(clips[index]);
    }

    public void PlayDeathSound() => PlayRandomSound(deathSounds);

#if UNITY_EDITOR
    public SceneAsset mainMenuScene; 
#endif

    [SerializeField] private string mainMenuSceneName; 

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead) return;
        currentHealth -= decayRate * Time.deltaTime * (1.0f + factor * timePass);
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        timePass+= Time.deltaTime;

        UpdateUI();
    }

    void UpdateUI()
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = currentHealth / maxHealth;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();
        UpdateLightIntensity();

        if (currentHealth <= 0 && !isDead)
        {
            TriggerDeath();
        }
    }

    private void TriggerDeath()
    {
        isDead = true;
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }
        Invoke(nameof(ReturnToMainMenuAfterDelay),deathDelay);
    }

    private void ReturnToMainMenuAfterDelay()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();
        UpdateLightIntensity();
    }

    private void UpdateLightIntensity()
    {
        if (playerLight != null)
        {
            float healthPercent = currentHealth / maxHealth;
            playerLight.intensity = Mathf.Lerp(minLightIntensity, maxLightIntensity, healthPercent);

            playerLight.pointLightOuterRadius = Mathf.Lerp(minLightRadius, maxLightRadius, healthPercent);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (mainMenuScene != null)
        {
            mainMenuSceneName = mainMenuScene.name;
        }
    }
#endif
}
