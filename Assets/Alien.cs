using UnityEngine;

public class Alien : MonoBehaviour
{
    [Header("Inscribed")]
    public GameObject alienBulletPrefab; // Assign your AlienBullet prefab here in the Inspector

    [HideInInspector]
    public AlienFleet fleet;

    public float fireRateMin = 2f;
    public float fireRateMax = 5f;
    private float nextFireTime;

    void Start()
    {
        SetRandomFireTime();
    }

    void Update()
    {
        // Fire bullets at random intervals
        if (Time.time >= nextFireTime)
        {
            FireBullet();
            SetRandomFireTime();
        }
    }

    void SetRandomFireTime()
    {
        nextFireTime = Time.time + Random.Range(fireRateMin, fireRateMax);
    }

    void FireBullet()
    {
        if (alienBulletPrefab != null)
        {
            // Instantiate the alien bullet at the alien's current position
            Instantiate(alienBulletPrefab, transform.position, Quaternion.identity);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Dart") || collision.gameObject.name.Contains("Dart"))
        {
            Destroy(gameObject);
            Destroy(collision.gameObject);
        }
    }
}