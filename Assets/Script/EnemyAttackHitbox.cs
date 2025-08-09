using UnityEngine;

public class EnemyAttackHitbox : MonoBehaviour
{
    public float damage = 10f;
    private bool hasHit = false;

    void setDamage(float _damage) {
        damage=_damage;
    }

    void OnEnable()
    {
        hasHit = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        if (other.CompareTag("Player"))
        {
            HealthController player = other.GetComponent<HealthController>();
            if (player != null)
            {
                player.TakeDamage(damage);
                hasHit = true;
            }
        }
    }
}
