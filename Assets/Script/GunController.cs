using UnityEngine;
using System.Collections;

public class GunController : MonoBehaviour
{
    private Transform player;
    public float offsetDistance = 1.5f;

    public GameObject bulletPrefab;
    public Transform firePoint;

    public float damage = 1.0f;

    public int bulletsPerShot = 1;
    public float fireRate = 0.2f;          
    public float deviation = 5f;            
    public float bulletSpeed = 10f;
    public float bulletLifeTime = 2f;
    public bool isAutomatic = false;        

    private float nextFireTime = 0f;

    public int ammoCapacity = 10;
    public int maxAmmo = 30;       
    public float reloadTime = 1.5f;

    private int currentAmmo;
    private int currentReserve;
    private bool isReloading = false;

    public Animator gunAnimator;
    private string shootAnimationTrigger = "Shoot";
    private string reloadAnimationTrigger = "Reload";
    public AnimatorOverrideController overrideController;

    public AudioSource audioSource;     
    public AudioClip[] idleSounds;
    public AudioClip[] reloadSounds;
    public AudioClip[] shootSounds;
    public Transform spotlight;


    private string id;

    private AmmoUI ammoUI;

    public void PlayIdleSound()
    {
        PlayRandomSound(idleSounds);
    }

    public void PlayReloadSound()
    {
        PlayRandomSound(reloadSounds);
    }

    public void PlayShootSound()
    {
        PlayRandomSound(shootSounds);
    }

    private void PlayRandomSound(AudioClip[] clips)
    {
        if (audioSource == null || clips == null || clips.Length == 0) return;

        int index = Random.Range(0, clips.Length);
        audioSource.pitch = Random.Range(0.95f, 1.05f); 
        audioSource.PlayOneShot(clips[index]);
    }

    public void UpdateAmmoUI()
    {
        if (ammoUI != null)
            ammoUI.UpdateAmmo(currentAmmo, currentReserve);
    }

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        currentAmmo = ammoCapacity;
        currentReserve = maxAmmo;

        if (overrideController != null)
        {
            gunAnimator.runtimeAnimatorController = overrideController;
        }

        ammoUI = AmmoUI.Instance;
        UpdateAmmoUI();
    }

    void Update()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0f;

        Vector3 direction = (mouseWorldPosition - player.position).normalized;
        transform.position = player.position + direction * offsetDistance;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        if (direction.x < 0)
            transform.localScale = new Vector3(1, -1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);

        if (isReloading) return;

        bool canFire = Time.time >= nextFireTime;
        bool shootInput = isAutomatic ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);

        if (shootInput && canFire && currentAmmo > 0)
        {
            Shoot(direction);
            nextFireTime = Time.time + fireRate;
        }

        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < ammoCapacity && currentReserve > 0)
        {
            StartCoroutine(Reload());
        }

        if (spotlight != null)
        {
            float spotlightAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            spotlight.rotation = Quaternion.Euler(0f, 0f, spotlightAngle);
            spotlight.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }
    }

    void Shoot(Vector2 direction)
    {
        if (gunAnimator != null)
        {
            gunAnimator.SetTrigger(shootAnimationTrigger);
            //Debug.Log("Triggered shoot animation");
        }

        for (int i = 0; i < bulletsPerShot; i++)
        {
            float angleOffset = Random.Range(-deviation, deviation);
            Quaternion rotation = Quaternion.Euler(0, 0, angleOffset);
            Vector2 finalDirection = rotation * direction;

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.setDamage(damage);
            bulletScript.SetSpeed(bulletSpeed); 
            bulletScript.SetLifetime(bulletLifeTime);
            bulletScript.SetDirection(finalDirection);
        }
        currentAmmo--;
        UpdateAmmoUI();
    }

    IEnumerator Reload()
    {
        isReloading = true;
        gunAnimator.SetTrigger(reloadAnimationTrigger);

        if (spotlight != null)
            spotlight.gameObject.SetActive(false);

        yield return new WaitForSeconds(reloadTime);

        int ammoNeeded = ammoCapacity - currentAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, currentReserve);

        currentAmmo += ammoToReload;
        currentReserve -= ammoToReload;

        isReloading = false;
        UpdateAmmoUI();

        if (spotlight != null)
            spotlight.gameObject.SetActive(true);
    }
}