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
        nextFireTime = Time.time + Random.Range(fireRateMin, fireRateMax) / GameManager.FireRateScale;
    }

    void FireBullet()
    {
        if (alienBulletPrefab != null)
        {
            // Instantiate the alien bullet at the alien's current position
            GameObject bullet = Instantiate(alienBulletPrefab, transform.position, Quaternion.identity);

            // Bullets pass through aliens so they aren't pushed around or stopped by the fleet
            Collider[] bulletColliders = bullet.GetComponentsInChildren<Collider>();
            foreach (Alien other in FindObjectsByType<Alien>(FindObjectsSortMode.None))
            {
                foreach (Collider ac in other.GetComponentsInChildren<Collider>())
                    foreach (Collider bc in bulletColliders)
                        Physics.IgnoreCollision(ac, bc);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponentInParent<Dart>() != null)
        {
            Destroy(gameObject);
            Destroy(collision.gameObject);
        }
    }
}