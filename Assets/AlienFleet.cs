using UnityEngine;
using System.Collections.Generic;

public class AlienFleet : MonoBehaviour
{
    [Header("Inscribed")]
    public GameObject alienPrefab;
    public AlienCounter alienCounter;

    public List<GameObject> aliens = new List<GameObject> { };

    float baseSpeed = 0.1f; 
    float speed = 0.1f;
    float leftBound = -16f;
    float rightBound = 16f;
    float lowerBound = -6.83f; // Match your spaceship's exact Y position (-6.83)
    float downTime = 0.1f;
    bool movingDown = false;
    int displayedCount = -1;
    int initialAlienCount = 30; 

    void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                GameObject alien = Instantiate<GameObject>(alienPrefab);
                alien.transform.position = new Vector3(2 * i - 12, 10 - 2 * j, 0);
                aliens.Add(alien);
                alien.GetComponent<Alien>().fleet = this;
            }
        }

        initialAlienCount = aliens.Count;
    }

    void Update()
    {
        aliens.RemoveAll(alien => alien == null);

        if (alienCounter == null) alienCounter = FindFirstObjectByType<AlienCounter>();
        if (alienCounter != null && aliens.Count != displayedCount)
        {
            displayedCount = aliens.Count;
            alienCounter.SetCount(displayedCount);
        }
        baseSpeed = GameManager.BaseAlienSpeed; // easy/hard level

        if (initialAlienCount > 0 && aliens.Count == 0)
        {
            GameManager.Win();
            return;
        }

        // Speed up remaining aliens as they get destroyed
        if (initialAlienCount > 0 && aliens.Count > 0)
        {
            float proportionRemaining = (float)aliens.Count / initialAlienCount;
            float speedMultiplier = Mathf.Lerp(GameManager.MaxSpeedMultiplier, 1.0f, proportionRemaining);
            float currentDirection = Mathf.Sign(speed);
            speed = baseSpeed * speedMultiplier * currentDirection;
        }
    }

    void FixedUpdate()
    {
        foreach (GameObject alien in aliens)
        {
            if (alien == null) continue;

            Vector3 position = alien.transform.position;
            if (!movingDown)
            {
                position.x += speed;
            }
            else
            {
                position.y -= Mathf.Abs(speed);
                
                // Trigger game over when aliens reach the spaceship's level
                if (position.y <= lowerBound)
                {
                    GameManager.GameOver();
                }
            }
            alien.transform.position = position;
        }

        float left = float.MaxValue, right = float.MinValue;
        foreach (GameObject alien in aliens)
        {
            if (alien == null) continue;

            Vector3 position = alien.transform.position;
            if (position.x > right) right = position.x;
            if (position.x < left) left = position.x;
        }

        if ((speed > 0 && right >= rightBound) || (speed < 0 && left <= leftBound))
        {
            speed = -speed;
            movingDown = true;
            Invoke("startSidewaysMotion", downTime);
        }
    }

    void startSidewaysMotion()
    {
        movingDown = false;
    }
}