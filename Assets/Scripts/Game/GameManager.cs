using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;   // ★ 국기 UI Image 때문에 추가

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private float matchDuration = 60f;

    [Header("Player Spawn")]
    [SerializeField] private Vector2 player1StartPos = new Vector2(-3f, 3f);
    [SerializeField] private Vector2 player2StartPos = new Vector2(3f, -3f);

    [Header("Team Prefabs (TeamSelect 순서와 동일)")]
    [SerializeField] private GameObject[] teamPrefabs;

    [Header("Ball")]
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Vector2 ballStartPos = new Vector2(0f, 0f);

    [Header("Scoreboard UI (Flag Images)")]
    [SerializeField] private Image leftFlagImage;   // 스코어보드 왼쪽 국기 UI
    [SerializeField] private Image rightFlagImage;  // 스코어보드 오른쪽 국기 UI
    [SerializeField] private Sprite[] teamFlagSprites; // 팀별 국기 스프라이트 (teamPrefabs 순서와 동일)

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
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        InitializeGame();
    }

    void Update()
    {
        if (!isGameActive || isPaused) return;

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f && !isSuddenDeath)
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

        // 플레이어 생성 직후, 양쪽 모두 중앙을 바라보게 설정
        SetAllPlayersFacingCenter();

        // ★ 선택한 팀의 국기를 스코어보드에 표시
        SetupScoreboardFlags();

        SetupCamera();
        SetupBackground();
        SetupItemSpawner();
    }

    void SpawnPlayers()
    {
        if (PlayerManager.Instance == null)
        {
            Debug.LogError("[GameManager] PlayerManager.Instance 가 없습니다. TeamSelectScene을 통해 들어와야 합니다.");
            return;
        }

        int p1Index = PlayerManager.Instance.player1TeamIndex;
        int p2Index = PlayerManager.Instance.player2TeamIndex;

        Debug.Log($"[GameManager] >>> SpawnPlayers. P1 index={p1Index}, P2 index={p2Index}");

        if (teamPrefabs == null || teamPrefabs.Length == 0)
        {
            Debug.LogError("[GameManager] teamPrefabs 배열이 비어있습니다.");
            return;
        }

        // Player1
        if (IsValidTeamIndex(p1Index))
        {
            Vector3 pos1 = new Vector3(player1StartPos.x, player1StartPos.y, 0f);
            player1Instance = Instantiate(teamPrefabs[p1Index], pos1, Quaternion.identity);

            PlayerController pc1 = player1Instance.GetComponent<PlayerController>();
            if (pc1 != null) pc1.SetPlayerNumber(1);

            AddSpriteToPlayer(player1Instance, Color.blue, "Player1");
            Debug.Log($"[GameManager] Player1 spawned with {teamPrefabs[p1Index].name} at {pos1}");
        }
        else
        {
            Debug.LogError("[GameManager] Player1 팀 인덱스가 잘못되었거나 프리팹이 비어있습니다.");
        }

        // Player2
        if (IsValidTeamIndex(p2Index))
        {
            Vector3 pos2 = new Vector3(player2StartPos.x, player2StartPos.y, 0f);
            player2Instance = Instantiate(teamPrefabs[p2Index], pos2, Quaternion.identity);

            PlayerController pc2 = player2Instance.GetComponent<PlayerController>();
            if (pc2 != null) pc2.SetPlayerNumber(2);

            AddSpriteToPlayer(player2Instance, Color.red, "Player2");
            Debug.Log($"[GameManager] Player2 spawned with {teamPrefabs[p2Index].name} at {pos2}");
        }
        else
        {
            Debug.LogError("[GameManager] Player2 팀 인덱스가 잘못되었거나 프리팹이 비어있습니다.");
        }
    }

    bool IsValidTeamIndex(int index)
    {
        return (index >= 0 && index < teamPrefabs.Length && teamPrefabs[index] != null);
    }

    void AddSpriteToPlayer(GameObject player, Color color, string playerName)
    {
        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
        if (sr == null) sr = player.AddComponent<SpriteRenderer>();

        if (sr.sprite == null)
        {
            sr.sprite = SpriteGenerator.CreatePlayerSprite(64, color);
            sr.sortingOrder = 1;
            Debug.Log($"[GameManager] {playerName} sprite generated.");
        }
    }

    void SpawnBall()
    {
        if (ballPrefab == null)
        {
            Debug.LogError("[GameManager] ballPrefab 이 할당되지 않았습니다.");
            return;
        }

        Vector3 ballPos = new Vector3(ballStartPos.x, ballStartPos.y, 0f);

        if (PlayerManager.Instance != null)
        {
            ballPos.x = PlayerManager.Instance.player1IsLeftSide ? -6f : 6f;
            Debug.Log($"[GameManager] Coin Toss applied. ballPos.x = {ballPos.x}");
        }

        ballInstance = Instantiate(ballPrefab, ballPos, Quaternion.identity);

        SpriteRenderer sr = ballInstance.GetComponent<SpriteRenderer>();
        if (sr == null) sr = ballInstance.AddComponent<SpriteRenderer>();
        if (sr.sprite == null)
        {
            sr.sprite = SpriteGenerator.CreateSoccerBallSprite(32);
            sr.sortingOrder = 2;
            Debug.Log("[GameManager] Ball sprite generated.");
        }

        Debug.Log($"[GameManager] Ball spawned at {ballPos}");
    }

    void SetupCamera()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            CameraFollow follow = cam.GetComponent<CameraFollow>();
            if (follow == null)
                follow = cam.gameObject.AddComponent<CameraFollow>();

            if (ballInstance != null)
                follow.target = ballInstance.transform;
        }
    }

    void SetupBackground()
    {
        Sprite grassSprite = Resources.Load<Sprite>("Sprites/GrassBackground");
        if (grassSprite == null)
        {
            Debug.LogWarning("[GameManager] GrassBackground sprite not found in Resources/Sprites/");
            return;
        }

        GameObject group = new GameObject("BackgroundGroup");

        float tileSize = grassSprite.bounds.size.x;
        int gridSize = 20;
        float offset = -(gridSize * tileSize) / 2f;

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                GameObject tile = new GameObject($"Tile_{x}_{y}");
                tile.transform.SetParent(group.transform);

                SpriteRenderer sr = tile.AddComponent<SpriteRenderer>();
                sr.sprite = grassSprite;
                sr.sortingOrder = -100;

                tile.transform.position = new Vector3(offset + x * tileSize, offset + y * tileSize, 10f);
            }
        }
    }

    void SetupItemSpawner()
    {
        ItemSpawner spawner = FindObjectOfType<ItemSpawner>();
        if (spawner == null)
        {
            GameObject obj = new GameObject("ItemSpawner");
            spawner = obj.AddComponent<ItemSpawner>();
        }

        GameObject[] loadedItems = Resources.LoadAll<GameObject>("Items");
        if (loadedItems != null && loadedItems.Length > 0)
        {
            spawner.Setup(new System.Collections.Generic.List<GameObject>(loadedItems));
        }
        else
        {
            Debug.LogWarning("[GameManager] No item prefabs found in Resources/Items");
        }
    }

    // ==========================
    //  스코어보드 국기 설정
    // ==========================
    void SetupScoreboardFlags()
    {
        if (PlayerManager.Instance == null) return;

        int p1Index = PlayerManager.Instance.player1TeamIndex;
        int p2Index = PlayerManager.Instance.player2TeamIndex;

        // teamFlagSprites 배열도 teamPrefabs와 같은 순서로 넣어두었다고 가정
        if (IsValidFlagIndex(p1Index) && leftFlagImage != null)
        {
            leftFlagImage.sprite = teamFlagSprites[p1Index];
        }

        if (IsValidFlagIndex(p2Index) && rightFlagImage != null)
        {
            rightFlagImage.sprite = teamFlagSprites[p2Index];
        }
    }

    bool IsValidFlagIndex(int index)
    {
        return (index >= 0 && index < teamFlagSprites.Length && teamFlagSprites[index] != null);
    }

    // ==========================
    //  가운데 바라보는 기능
    // ==========================

    void SetAllPlayersFacingCenter()
    {
        SetPlayerFacingCenter(player1Instance);
        SetPlayerFacingCenter(player2Instance);
    }

    void SetPlayerFacingCenter(GameObject player)
    {
        if (player == null) return;

        Transform visual = player.transform.Find("Visual");
        if (visual == null)
        {
            visual = player.transform;
        }

        Vector3 scale = visual.localScale;

        if (player.transform.position.x < 0f)
        {
            scale.x = Mathf.Abs(scale.x);
        }
        else if (player.transform.position.x > 0f)
        {
            scale.x = -Mathf.Abs(scale.x);
        }

        visual.localScale = scale;
    }

    // ==========================

    public void OnGoalScored(int scoringPlayer)
    {
        if (!isGameActive) return;

        if (scoringPlayer == 1) player1Score++;
        else if (scoringPlayer == 2) player2Score++;

        Camera cam = Camera.main;
        if (cam != null)
        {
            CameraFollow follow = cam.GetComponent<CameraFollow>();
            if (follow != null)
            {
                follow.TriggerShake(0.5f, 0.3f);
            }
        }

        if (isSuddenDeath)
        {
            EndGame();
            return;
        }

        if (GameUI.Instance != null)
        {
            GameUI.Instance.ShowGoalText();
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayGoalSound();
        }

        StartCoroutine(GoalSequence());
    }

    System.Collections.IEnumerator GoalSequence()
    {
        Time.timeScale = 0.05f;
        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 1f;

        ResetPositions();
    }

    void ResetPositions()
    {
        if (player1Instance != null)
            player1Instance.transform.position = player1StartPos;

        if (player2Instance != null)
            player2Instance.transform.position = player2StartPos;

        // 위치 리셋 후 다시 중앙 바라보게
        SetAllPlayersFacingCenter();

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
            Debug.Log("[GameManager] Sudden Death!");
        }
        else
        {
            EndGame();
        }
    }

    void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void QuitToTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScene");
    }

    void EndGame()
    {
        isGameActive = false;

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayWhistleSound();

        Debug.Log($"[GameManager] Game Over! P1={player1Score}, P2={player2Score}");

        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.lastP1Score = player1Score;
            PlayerManager.Instance.lastP2Score = player2Score;
        }

        SceneManager.LoadScene("ResultScene");
    }

    public int GetPlayer1Score() => player1Score;
    public int GetPlayer2Score() => player2Score;
    public float GetRemainingTime() => remainingTime;
    public bool IsGameActive() => isGameActive;
    public bool IsPaused() => isPaused;
    public bool IsSuddenDeath() => isSuddenDeath;
}
