using UnityEngine;

public class Alien : MonoBehaviour
{
    [Header("Inscribed")]
    public GameObject alienBulletPrefab; // Assign your AlienBullet prefab here in Inspector or instantiate via resources/fleet

    [HideInInspector]
    public AlienFleet fleet;

    public float fireRateMin = 2f;
    public float fireRateMax = 5f;
    private float nextFireTime;

    void Start()
    {
        // Set a random initial delay for each alien so they don't all fire at once
        SetRandomFireTime();
    }

    void Update()
    {
        // Fire bullets at random intervals if the game isn't paused
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
            GameObject bullet = Instantiate(alienBulletPrefab, transform.position, Quaternion.identity);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if hit by player's bullet (Dart)
        if (collision.gameObject.CompareTag("Dart") || collision.gameObject.name.Contains("Dart"))
        {
            Destroy(gameObject);
            Destroy(collision.gameObject);
        }
    }
}