using UnityEngine;

public class Alien : MonoBehaviour
{
    [Header("Inscribed")]
    public GameObject bulletPrefab;
    public AlienFleet fleet;   // set by AlienFleet when it spawns this alien

    const float BulletSpawnRate = 0.002f;
    const float ShotCooldown = 0.1f;

    float lastShot = 0f;

    void FixedUpdate()
    {
        if (Random.value < BulletSpawnRate && Time.time - lastShot > ShotCooldown)
        {
            Instantiate(bulletPrefab, transform.position, bulletPrefab.transform.rotation);
            lastShot = Time.time;
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        // An alien touching the ship is an immediate loss
        if (coll.gameObject.TryGetComponent(out Spaceship _))
        {
            fleet.EndGame("GAME OVER");
            return;
        }

        fleet.RemoveAlien(gameObject);
        Destroy(gameObject);
        Destroy(coll.gameObject);
    }
}