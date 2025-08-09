using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed = 10f;
    private float lifeTime = 2f;

    private Vector2 direction;
    private float damage = 1.0f;

    public void setDamage(float x) {
        damage = x;
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        Destroy(gameObject, lifeTime);
    }

    public void SetSpeed(float s)
    {
        speed = s;
    }

    public void SetLifetime(float time)
    {
        lifeTime = time;
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log(other.CompareTag("Enemy"));
        if (other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            Destroy(gameObject); 
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }

}
