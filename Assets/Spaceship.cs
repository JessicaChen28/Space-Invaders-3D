using UnityEngine;
using UnityEngine.InputSystem;

public class Spaceship : MonoBehaviour
{
    [Header("Inscribed")]
    public GameObject dartPrefab;

    public static float shotCooldown = .05f;
    public float lastShot = 0;
    public float speed = 10f;

    public float minX = -16f;
    public float maxX = 16f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    { 
        Vector3 position = transform.position;
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            position.x += speed * Time.deltaTime;
        }
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            position.x -= speed * Time.deltaTime;
        }
        position.x = Mathf.Clamp(position.x, minX, maxX);
        transform.position = position;
        if (Keyboard.current.spaceKey.wasPressedThisFrame && Time.time - lastShot > shotCooldown)
        {
            GameObject bullet = Instantiate<GameObject>(dartPrefab);
            bullet.transform.position = position;
            lastShot = Time.time;
        }
    }

}


