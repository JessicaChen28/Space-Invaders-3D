using UnityEngine;

public class AlienBullet : MonoBehaviour
{
    public float speed = 8f;

    void Update()
    {
        // Move straight down toward the player's Y level (-6.83)
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        // Clean up bullet if it goes off-screen below the player
        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }
}