using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 2f;
    public float stopDistance = 0.5f;
    public float attackRange = 0.6f;
    public float attackCooldown = 1.5f;
    public float damage = 50f;
    public float hurtDuration = 0.5f;
    public float timeToDamage = 0.3f;
    public float maxHealth = 100f;

    public GameObject attackHitbox;
    public float hitboxDuration = 0.1f;

    public float aggroRange = 5f; 
    public bool flashlightOnEnemy = false; 

    public float wanderRadius = 3f; 
    public float wanderInterval = 2f; 
    public float maxWanderRange = 6f;

    private Vector3 startPosition;

    private bool isHurt = false;
    private bool isDead = false;
    private Animator animator;
    private float health;
    private Vector2 direction;
    private float lastAttackTime = -9999f;
    private Vector3 wanderTarget;
    private float wanderTimer;

    public AudioSource audioSource;
    public AudioClip[] idleSounds;
    public AudioClip[] walkSounds;
    public AudioClip[] attackSounds;
    public AudioClip[] hurtSounds;
    public AudioClip[] deathSounds;

    public GameObject coinPrefab;
    public int minCoins = 1;
    public int maxCoins = 5;
    public float coinSpread = 1f;

    private float fixedZ;

    private void PlayRandomSound(AudioClip[] clips)
    {
        if (audioSource == null || clips == null || clips.Length == 0) return;
        int index = Random.Range(0, clips.Length);
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(clips[index]);
    }

    public void PlayIdleSound() => PlayRandomSound(idleSounds);
    public void PlayWalkSound() => PlayRandomSound(walkSounds);
    public void PlayAttackSound() => PlayRandomSound(attackSounds);
    public void PlayHurtSound() => PlayRandomSound(hurtSounds);
    public void PlayDeathSound() => PlayRandomSound(deathSounds);

    void Start()
    {
        health = maxHealth;
        animator = GetComponent<Animator>();

        startPosition = transform.position;
        fixedZ = transform.position.z; 

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        if (attackHitbox != null)
        {
            EnemyAttackHitbox hitbox = attackHitbox.GetComponent<EnemyAttackHitbox>();
            if (hitbox != null)
                hitbox.damage = damage;
        }

        wanderTarget = transform.position;
        wanderTimer = wanderInterval;
    }

    private bool aggro = false;

    void Update()
    {
        if (isHurt || isDead || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if ((distance <= aggroRange) || flashlightOnEnemy) aggro = true;

        if (aggro)
        {
            ChasePlayer(distance);
        }
        else
        {
            Wander();
        }

        if (direction.x > 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
    }

    void ChasePlayer(float distance)
    {
        direction = (player.position - transform.position).normalized;

        if (distance > stopDistance)
        {
            Vector3 newPos = transform.position + (Vector3)(direction * moveSpeed * Time.deltaTime);
            newPos.z = fixedZ; 
            transform.position = newPos;
        }

        if (distance <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }
    }

    void Wander()
    {
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0f)
        {
            Vector3 randomOffset = (Vector3)Random.insideUnitCircle * wanderRadius;
            Vector3 potentialTarget = transform.position + randomOffset;

            if (Vector3.Distance(startPosition, potentialTarget) > maxWanderRange)
            {
                Vector3 dirToCenter = (startPosition - transform.position).normalized;
                potentialTarget = startPosition + dirToCenter * (maxWanderRange - 0.5f);
            }

            potentialTarget.z = fixedZ;
            wanderTarget = potentialTarget;
            wanderTimer = wanderInterval;
        }

        Vector3 movePos = Vector2.MoveTowards(transform.position, wanderTarget, moveSpeed * 0.5f * Time.deltaTime);
        movePos.z = fixedZ; 
        transform.position = movePos;
    }

    void Attack()
    {
        animator.SetTrigger("Attack");
        lastAttackTime = Time.time;

        if (attackHitbox != null)
        {
            attackHitbox.SetActive(true);
            Invoke(nameof(DisableHitbox), hitboxDuration);
        }
    }

    void DisableHitbox()
    {
        if (attackHitbox != null)
            attackHitbox.SetActive(false);
    }

    public void TakeDamage(float damage)
    {
        aggro = true;
        if (health <= 0 || isDead) return;

        health -= damage;

        if (health <= 0)
        {
            isDead = true;
            Die();
            return;
        }

        if (isHurt) return;

        isHurt = true;
        animator.SetTrigger("Hurt");
        Invoke(nameof(RecoverFromHurt), hurtDuration);
    }

    private void RecoverFromHurt() => isHurt = false;

    private void Die()
    {
        isHurt = true;
        animator.SetTrigger("Death");
        Invoke(nameof(DisableEnemy), 1.0f);
    }

    private void DisableEnemy()
    {
        DropCoins();
        Destroy(gameObject);
    }

    private void DropCoins()
    {
        if (coinPrefab == null) return;

        int coinCount = Random.Range(minCoins, maxCoins + 1);
        for (int i = 0; i < coinCount; i++)
        {
            Vector2 spawnOffset = Random.insideUnitCircle * coinSpread;
            Instantiate(coinPrefab, transform.position + (Vector3)spawnOffset, Quaternion.identity);
        }
    }
}
