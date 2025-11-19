using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game Settings")]
    [SerializeField] private float matchDuration = 60f; // 60초
    
    [Header("Players")]
    [SerializeField] private GameObject player1Prefab;
    [SerializeField] private GameObject player2Prefab;
    [SerializeField] private Vector2 player1StartPos = new Vector2(-3f, 3f);  // 왼쪽 위
    [SerializeField] private Vector2 player2StartPos = new Vector2(3f, -3f);  // 오른쪽 아래

    [Header("Ball")]
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Vector2 ballStartPos = new Vector2(0f, 0f);  // 중앙
    
    private int player1Score = 0;
    private int player2Score = 0;
    private float remainingTime;
    private bool isGameActive = false;
    private bool isPaused = false;
    private bool isSuddenDeath = false;
    
    private GameObject player1Instance;
    private GameObject player2Instance;
    private GameObject ballInstance;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        InitializeGame();
    }
    
    void Update()
    {
        if (!isGameActive || isPaused) return;
        
        remainingTime -= Time.deltaTime;
        
        if (remainingTime <= 0 && !isSuddenDeath)
        {
            EndRegularTime();
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    
    void InitializeGame()
    {
        remainingTime = matchDuration;
        player1Score = 0;
        player2Score = 0;
        isGameActive = true;
        isSuddenDeath = false;
        
        SpawnPlayers();
        SpawnBall();
    }
    
    void SpawnPlayers()
    {
        if (player1Prefab != null)
        {
            Vector3 pos1 = new Vector3(player1StartPos.x, player1StartPos.y, 0f);
            player1Instance = Instantiate(player1Prefab, pos1, Quaternion.identity);

            AddSpriteToPlayer(player1Instance, Color.red, "Player1");
            Debug.Log($"Player1 spawned at {pos1}");
        }
        else
        {
            Debug.LogError("Player1 Prefab is not assigned!");
        }

        if (player2Prefab != null)
        {
            Vector3 pos2 = new Vector3(player2StartPos.x, player2StartPos.y, 0f);
            player2Instance = Instantiate(player2Prefab, pos2, Quaternion.identity);

            AddSpriteToPlayer(player2Instance, Color.blue, "Player2");
            Debug.Log($"Player2 spawned at {pos2}");
        }
        else
        {
            Debug.LogError("Player2 Prefab is not assigned!");
        }
    }

    void SpawnBall()
    {
        if (ballPrefab != null)
        {
            Vector3 ballPos = new Vector3(ballStartPos.x, ballStartPos.y, 0f);
            ballInstance = Instantiate(ballPrefab, ballPos, Quaternion.identity);

            AddSpriteToBall(ballInstance);
            Debug.Log($"Ball spawned at {ballPos}");
        }
        else
        {
            Debug.LogError("Ball Prefab is not assigned!");
        }
    }

    void AddSpriteToPlayer(GameObject player, Color color, string playerName)
    {
        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            sr = player.AddComponent<SpriteRenderer>();
        }

        if (sr.sprite == null)
        {
            sr.sprite = SpriteGenerator.CreatePlayerSprite(64, color);
            sr.sortingOrder = 1;
            Debug.Log($"{playerName} sprite generated!");
        }
    }

    void AddSpriteToBall(GameObject ball)
    {
        SpriteRenderer sr = ball.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            sr = ball.AddComponent<SpriteRenderer>();
        }

        if (sr.sprite == null)
        {
            sr.sprite = SpriteGenerator.CreateSoccerBallSprite(32);
            sr.sortingOrder = 2;
            Debug.Log("Ball sprite generated!");
        }
    }
    
    public void OnGoalScored(int scoringPlayer)
    {
        if (!isGameActive) return;
        
        if (scoringPlayer == 1)
        {
            player1Score++;
        }
        else if (scoringPlayer == 2)
        {
            player2Score++;
        }
        
        if (isSuddenDeath)
        {
            EndGame();
            return;
        }
        
        ResetPositions();
    }
    
    void ResetPositions()
    {
        if (player1Instance != null)
            player1Instance.transform.position = player1StartPos;
        if (player2Instance != null)
            player2Instance.transform.position = player2StartPos;
        
        if (ballInstance != null)
        {
            Ball ball = ballInstance.GetComponent<Ball>();
            if (ball != null)
                ball.ResetPosition(ballStartPos);
        }
    }
    
    void EndRegularTime()
    {
        if (player1Score == player2Score)
        {
            isSuddenDeath = true;
            Debug.Log("Sudden Death!");
        }
        else
        {
            EndGame();
        }
    }
    
    void EndGame()
    {
        isGameActive = false;
        Debug.Log($"Game Over! Player1: {player1Score}, Player2: {player2Score}");
    }
    
    void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void QuitToTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScene");
    }
    
    public int GetPlayer1Score() => player1Score;
    public int GetPlayer2Score() => player2Score;
    public float GetRemainingTime() => remainingTime;
    public bool IsGameActive() => isGameActive;
    public bool IsPaused() => isPaused;
    public bool IsSuddenDeath() => isSuddenDeath;
}

