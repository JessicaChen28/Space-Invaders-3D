using UnityEngine;
using UnityEngine.InputSystem;

public class Spaceship : MonoBehaviour
{
    [Header("Inscribed")]
    public GameObject dartPrefab;

    public static float shotCooldown = 0.05f;
    public float lastShot = 0;
    public float speed = 10f;
    
    [Header("Movement Boundaries")]
    public float minX = -16f;
    public float maxX = 16f;

    void Update()
    {
        if (GameManager.State != GameState.Playing) return;

        Vector3 position = transform.position;

        if (Keyboard.current.rightArrowKey.isPressed)
        {
            position.x += speed * Time.deltaTime;
        }
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            position.x -= speed * Time.deltaTime;
        }

        // Clamp position securely
        position.x = Mathf.Clamp(position.x, minX, maxX);
        transform.position = position;

        // Shooting logic
        if (Keyboard.current.spaceKey.wasPressedThisFrame && Time.time - lastShot > shotCooldown)
        {
            GameObject bullet = Instantiate<GameObject>(dartPrefab);
            bullet.transform.position = transform.position;
            lastShot = Time.time;
        }
    }
}