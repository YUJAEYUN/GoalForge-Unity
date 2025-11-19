using UnityEngine;

public class Stadium2D : MonoBehaviour
{
    [Header("경기장 스프라이트")]
    [SerializeField] private Sprite pitchSprite;

    [Header("경기장 크기 (기획서 기준)")]
    [SerializeField] private float fieldWidth = 24f;  // 24m
    [SerializeField] private float fieldHeight = 12f; // 12m

    [Header("골대 설정")]
    [SerializeField] private float goalWidth = 3.2f;  // 3.2m
    [SerializeField] private float goalHeight = 1.8f; // 1.8m

    void Start()
    {
        CreateStadium();
    }
    
    void CreateStadium()
    {
        CreatePitchBackground();

        CreateGround();

        CreateWalls();

        CreateGoals();
    }

    void CreatePitchBackground()
    {
        GameObject pitch = new GameObject("PitchBackground");
        pitch.transform.parent = transform;
        pitch.transform.localPosition = Vector3.zero;

        SpriteRenderer sr = pitch.AddComponent<SpriteRenderer>();

        if (pitchSprite == null)
        {
            pitchSprite = Resources.Load<Sprite>("Sprites/Pitch1");
            if (pitchSprite == null)
            {
                Debug.LogWarning("Pitch sprite not found! Please assign it in the Inspector or place Pitch1.png in Resources/Sprites/");
                return;
            }
        }

        sr.sprite = pitchSprite;
        sr.sortingOrder = -10;

        float spriteWidth = pitchSprite.bounds.size.x;
        float spriteHeight = pitchSprite.bounds.size.y;
        float scaleX = fieldWidth / spriteWidth;
        float scaleY = fieldHeight / spriteHeight;
        pitch.transform.localScale = new Vector3(scaleX, scaleY, 1f);
    }

    void CreateGround()
    {
    }

    void CreateWalls()
    {
        CreateWall("WallLeft", new Vector2(-9f, 0f), new Vector2(1f, fieldHeight));

        CreateWall("WallRight", new Vector2(9f, 0f), new Vector2(1f, fieldHeight));

        CreateWall("WallTop", new Vector2(0f, 4.7f), new Vector2(fieldWidth, 1f));

        CreateWall("WallBottom", new Vector2(0f, -6.5f), new Vector2(fieldWidth, 1f));
    }

    void CreateWall(string name, Vector2 position, Vector2 size)
    {
        GameObject wall = new GameObject(name);
        wall.transform.parent = transform;
        wall.transform.localPosition = position;

        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer != -1)
            wall.layer = groundLayer;

        BoxCollider2D collider = wall.AddComponent<BoxCollider2D>();
        collider.size = size;
    }
    
    void CreateGoals()
    {
        CreateGoal("GoalRight", new Vector2(8f, 0f), 1, 90f);

        CreateGoal("GoalLeft", new Vector2(-8f, 0f), 2, 90f);
    }

    void CreateGoal(string name, Vector2 position, int goalOwner, float rotationZ = 0f)
    {
        GameObject goal = new GameObject(name);
        goal.transform.parent = transform;
        goal.transform.localPosition = position;
        goal.transform.localRotation = Quaternion.Euler(0f, 0f, rotationZ);

        int goalLayer = LayerMask.NameToLayer("GoalPost");
        if (goalLayer != -1)
            goal.layer = goalLayer;

        try
        {
            goal.tag = name;
        }
        catch (UnityException)
        {
            Debug.LogWarning($"Tag '{name}' not found. Please add it in Tag Manager.");
        }

        BoxCollider2D collider = goal.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(goalWidth, 1f); // 가로 3.2m, 세로 1m (얇게)
        collider.isTrigger = true;

        GoalPost goalPost = goal.AddComponent<GoalPost>();
        var field = typeof(GoalPost).GetField("goalOwner", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
            field.SetValue(goalPost, goalOwner);
    }
}

