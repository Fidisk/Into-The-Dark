using UnityEngine;

public class Coin : MonoBehaviour
{
    public float pickupRadius = 0.5f;
    public int coinValue = 1;
    public float healthValue = 10;
    public float attractSpeed = 5f;

    private Transform player;

    public AudioClip pickupSound;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= pickupRadius)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, attractSpeed * Time.deltaTime);

            if (distance <= 0.1f)
            {
                if (player != null)
                {
                    player.GetComponent<PlayerCoins>()?.AddCoins(coinValue);
                    player.GetComponent<HealthController>()?.Heal(healthValue);
                }

                if (pickupSound != null)
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position, 1f);

                Destroy(gameObject);
            }
        }
    }
}
