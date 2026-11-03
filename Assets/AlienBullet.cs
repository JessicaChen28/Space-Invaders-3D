using UnityEngine;

public class AlienBullet : MonoBehaviour
{
    const float Speed = 0.1f;
    const float KillY = -10f;

    void FixedUpdate()
    {
        Vector3 position = transform.position;
        position.y -= Speed;
        transform.position = position;
        if (position.y < KillY)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.TryGetComponent(out Spaceship ship))
        {
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
            Destroy(coll.gameObject);
        }
    }
}