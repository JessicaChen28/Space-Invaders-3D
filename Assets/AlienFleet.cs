using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class AlienFleet : MonoBehaviour
{
    [Header("Inscribed")]
    public GameObject alienPrefab;
    public AlienCounter alienCounter;
    public TextMeshProUGUI gameOverText;   // drag your (inactive) "Game Over" TMP text here

    [Header("Dynamic")]
    public List<GameObject> aliens = new List<GameObject>();

    const int Columns = 10;
    const int Rows = 3;
    const float BaseSpeed = 0.1f;
    const float MaxSpeedBoost = 1.5f;   // last alien moves at BaseSpeed * (1 + 1.5)
    const float LeftBound = -16f;
    const float RightBound = 16f;
    const float DropDistance = 0.5f;
    const float LoseY = 1f;             // TODO: set to just above your spaceship's Y position

    int direction = 1;
    int startingCount;
    bool gameOver = false;

    void Start()
    {
        Time.timeScale = 1f;   // in case a previous game froze time
        for (int i = 0; i < Columns; i++)
        {
            for (int j = 0; j < Rows; j++)
            {
                GameObject alien = Instantiate(alienPrefab);
                alien.transform.position = new Vector3(2 * i - 12, 10 - 2 * j, 0);
                alien.GetComponent<Alien>().fleet = this;
                aliens.Add(alien);
            }
        }
        startingCount = aliens.Count;
        alienCounter.SetCount(aliens.Count);
    }

    public void RemoveAlien(GameObject alien)
    {
        aliens.Remove(alien);
        alienCounter.SetCount(aliens.Count);
    }

    void FixedUpdate()
    {
        if (gameOver) return;

        if (aliens.Count == 0)
        {
            EndGame("YOU WIN!");
            return;
        }

        float destroyedFraction = 1f - (float)aliens.Count / startingCount;
        float speed = BaseSpeed * (1f + MaxSpeedBoost * destroyedFraction);

        float left = Mathf.Infinity;
        float right = Mathf.NegativeInfinity;
        float lowest = Mathf.Infinity;
        foreach (GameObject alien in aliens)
        {
            Vector3 position = alien.transform.position;
            position.x += direction * speed;
            alien.transform.position = position;

            left = Mathf.Min(left, position.x);
            right = Mathf.Max(right, position.x);
            lowest = Mathf.Min(lowest, position.y);
        }

        if ((direction > 0 && right >= RightBound) || (direction < 0 && left <= LeftBound))
        {
            direction = -direction;
            foreach (GameObject alien in aliens)
            {
                alien.transform.position += Vector3.down * DropDistance;
            }
            lowest -= DropDistance;
        }

        if (lowest <= LoseY)
        {
            EndGame("GAME OVER");
        }
    }

    // Single place where every win/loss ends up
    public void EndGame(string message)
    {
        if (gameOver) return;   // ignore repeat calls (e.g. bullet and alien hit in the same frame)
        gameOver = true;
        Time.timeScale = 0f;
        if (gameOverText != null)
        {
            gameOverText.text = message;
            gameOverText.gameObject.SetActive(true);
        }
    }
}