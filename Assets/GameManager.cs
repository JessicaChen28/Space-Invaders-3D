using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum GameState { Start, Playing, GameOver }

// Creates itself at runtime, so it needs no scene setup.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static GameState State = GameState.Start;
    public static bool Hard = false;

    const int EasyShield = 5;

    public static float BaseAlienSpeed { get { return Hard ? 0.2f : 0.1f; } }
    public static float MaxSpeedMultiplier { get { return Hard ? 3.5f : 2.5f; } }
    public static float FireRateScale { get { return Hard ? 2f : 0.5f; } }

    public static int Shield = EasyShield;

    Texture2D background;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Create()
    {
        if (Instance != null) return;
        GameObject go = new GameObject("GameManager");
        go.AddComponent<GameManager>();
        DontDestroyOnLoad(go);
    }

    void Awake()
    {
        Instance = this;
        background = Resources.Load<Texture2D>("StartBackground");
        SceneManager.sceneLoaded += OnSceneLoaded;
        ShowStartScreen();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ShowStartScreen();
    }

    void ShowStartScreen()
    {
        State = GameState.Start;
        Time.timeScale = 0f;
    }

    void StartGame(bool hard)
    {
        Hard = hard;
        Shield = hard ? 1 : EasyShield; // hard mode: a single hit ends the game
        State = GameState.Playing;
        Time.timeScale = 1f;
    }

    public static void GameOver()
    {
        if (State != GameState.Playing) return;
        State = GameState.GameOver;
        Time.timeScale = 0f;
    }

    // Called when an alien bullet hits the spaceship
    public static void PlayerHit()
    {
        if (State != GameState.Playing) return;
        Shield--;
        if (Shield <= 0) GameOver();
    }

    void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        if (State == GameState.Start)
        {
            if (kb.digit1Key.wasPressedThisFrame) StartGame(false);
            if (kb.digit2Key.wasPressedThisFrame) StartGame(true);
        }
        else if (State == GameState.GameOver && kb.rKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void OnGUI()
    {
        if (State == GameState.Start) DrawStart();
        else if (State == GameState.Playing) DrawHud();
        else DrawGameOver();
    }

    GUIStyle Style(int size, Color color)
    {
        GUIStyle s = new GUIStyle(GUI.skin.label) { fontSize = size, alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
        s.normal.textColor = color;
        return s;
    }

    void DrawStart()
    {
        if (background != null)
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), background, ScaleMode.ScaleAndCrop);

        float w = Screen.width, h = Screen.height;
        GUI.Label(new Rect(0, h * 0.15f, w, h * 0.2f), "SPACE INVADERS 3D", Style(Mathf.RoundToInt(h * 0.09f), Color.white));
        GUI.Label(new Rect(0, h * 0.33f, w, h * 0.08f), "Select a level   (arrows move, space shoots)", Style(Mathf.RoundToInt(h * 0.035f), Color.white));

        GUIStyle button = new GUIStyle(GUI.skin.button) { fontSize = Mathf.RoundToInt(h * 0.035f) };
        float bw = w * 0.28f, bh = h * 0.12f;
        if (GUI.Button(new Rect(w / 2 - bw / 2, h * 0.47f, bw, bh), "EASY  (5 shields)  [1]", button)) StartGame(false);
        if (GUI.Button(new Rect(w / 2 - bw / 2, h * 0.62f, bw, bh), "HARD  (faster, no shield)  [2]", button)) StartGame(true);
    }

    void DrawHud()
    {
        string text = Hard ? "HARD" : "Shield: " + Shield;
        GUIStyle s = Style(Mathf.RoundToInt(Screen.height * 0.04f), Hard ? Color.red : Color.cyan);
        s.alignment = TextAnchor.UpperRight;
        GUI.Label(new Rect(0, 10, Screen.width - 15, 60), text, s);
    }

    void DrawGameOver()
    {
        float w = Screen.width, h = Screen.height;
        Color old = GUI.color;
        GUI.color = new Color(0, 0, 0, 0.75f);
        GUI.DrawTexture(new Rect(0, 0, w, h), Texture2D.whiteTexture);
        GUI.color = old;

        GUI.Label(new Rect(0, 0, w, h), "GAME OVER", Style(Mathf.RoundToInt(h * 0.12f), Color.red));
        GUI.Label(new Rect(0, h * 0.62f, w, h * 0.08f), "Press R to return to the start screen", Style(Mathf.RoundToInt(h * 0.035f), Color.white));
    }
}
