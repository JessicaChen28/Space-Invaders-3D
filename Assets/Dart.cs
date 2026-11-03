using UnityEngine;

public class Dart : MonoBehaviour
{
    [Header("Inscribed")]
    static float speed = .3f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Darts destroy alien bullets (both objects disappear)
    void OnCollisionEnter(Collision collision)
    {
        HitBullet(collision.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        HitBullet(other.gameObject);
    }

    void HitBullet(GameObject other)
    {
        if (other.GetComponentInParent<AlienBullet>() != null)
        {
            Destroy(other.GetComponentInParent<AlienBullet>().gameObject);
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        Vector3 position = transform.position;
        position.y += speed;
        transform.position = position;
        if(position.y > 10)
        {
            Destroy(gameObject);
        }
    }

}
