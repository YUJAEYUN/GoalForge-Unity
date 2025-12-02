using UnityEngine;
using UnityEngine.SceneManagement;

public class SoccerCursor : MonoBehaviour
{
    public static SoccerCursor Instance;

    [Header("커서 이미지")]
    public Texture2D normalCursorTexture;    // 기본 축구공
    public Texture2D hoverCursorTexture;     // 버튼 위에서 사용할 이미지
    public Vector2 hotspot = Vector2.zero;

    [Header("메인 게임 씬 이름")]
    public string mainGameSceneName = "MainGame";   // 실제 메인 게임 씬 이름으로 변경

    private bool isHover = false;    // 버튼 위에 있는지 여부

    void Awake()
    {
        // 싱글톤 처리
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        UpdateCursorByScene(SceneManager.GetActiveScene());
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateCursorByScene(scene);
    }

    void Update()
    {
        // 메인게임 씬이면 커서 숨기고 아무것도 안 함
        if (SceneManager.GetActiveScene().name == mainGameSceneName)
            return;

        // 버튼 위/아닌 경우에 따라 커서 이미지 변경
        Texture2D tex = normalCursorTexture;

        if (isHover && hoverCursorTexture != null)
        {
            tex = hoverCursorTexture;
        }

        Cursor.visible = true;
        Cursor.SetCursor(tex, hotspot, CursorMode.Auto);
    }

    void UpdateCursorByScene(Scene scene)
    {
        if (scene.name == mainGameSceneName)
        {
            // 메인 게임 씬: 커서 숨김
            Cursor.visible = false;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            // 그 외 씬: 기본 커서부터 세팅
            Cursor.visible = true;
            Cursor.SetCursor(normalCursorTexture, hotspot, CursorMode.Auto);
        }
    }

    // 버튼에서 호출할 함수
    public void SetHover(bool hover)
    {
        isHover = hover;
    }
}
