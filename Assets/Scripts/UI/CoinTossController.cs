using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CoinTossController : MonoBehaviour
{
    [Header("3D Coin Object")]
    public GameObject coin3D;
    public Camera coinRenderCamera;
    public RawImage coinRawImage;

    [Header("UI")]
    public TextMeshProUGUI resultText;
    public Button playButton;

    [Header("Canvas Position")]
    public Vector2 startPosition = new Vector2(500, 300);
    public Vector2 endPosition = new Vector2(0, 0);
    public float tossDuration = 2f;
    public float spinSpeed = 720f;
    public float sizeStart = 100f;
    public float sizeEnd = 400f;

    [Header("Team UI Preview")]
    public Image player1PreviewImage;                 // 동전 왼쪽 이미지
    public Image player2PreviewImage;                 // 동전 오른쪽 이미지
    public Sprite[] teamSprites;                      // 팀 아이콘 (TeamSelect 순서와 동일)
    public TextMeshProUGUI player1TeamNameText;       // 1P 팀 이름 텍스트
    public TextMeshProUGUI player2TeamNameText;       // 2P 팀 이름 텍스트

    private bool player1IsLeft;
    private RectTransform imageRect;
    private RenderTexture renderTexture;

    private void Start()
    {
        if (coinRawImage != null)
            imageRect = coinRawImage.GetComponent<RectTransform>();

        SetupRenderTexture();

        // ★ 선택한 팀에 따라 이미지 + 텍스트 세팅
        SetupTeamPreviewUI();

        if (playButton != null)
        {
            playButton.gameObject.SetActive(false);
            playButton.onClick.AddListener(OnPlayButtonClicked);
        }

        StartCoroutine(Auto3DCoinToss());
    }

    private void SetupRenderTexture()
    {
        renderTexture = new RenderTexture(512, 512, 24);
        renderTexture.antiAliasing = 4;

        if (coinRenderCamera != null)
        {
            coinRenderCamera.targetTexture = renderTexture;
            coinRenderCamera.clearFlags = CameraClearFlags.SolidColor;
            coinRenderCamera.backgroundColor = new Color(0, 0, 0, 0);
        }

        if (coinRawImage != null)
            coinRawImage.texture = renderTexture;
    }

    // ============================
    //      팀 UI 미리보기 세팅
    // ============================
    private void SetupTeamPreviewUI()
    {
        if (PlayerManager.Instance == null)
        {
            Debug.LogError("[CoinToss] PlayerManager.Instance 가 없습니다. TeamSelectScene을 통해 들어와야 합니다.");
            return;
        }

        int p1Index = PlayerManager.Instance.player1TeamIndex;
        int p2Index = PlayerManager.Instance.player2TeamIndex;

        Sprite p1Sprite = GetTeamSprite(p1Index);
        Sprite p2Sprite = GetTeamSprite(p2Index);

        // 1P 이미지
        if (player1PreviewImage != null)
        {
            player1PreviewImage.sprite = p1Sprite;
            player1PreviewImage.enabled = (p1Sprite != null);

            // 혹시 스케일이 꼬여 있으면 초기화
            player1PreviewImage.rectTransform.localScale = new Vector3(1f, 1f, 1f);
        }

        // 2P 이미지 (좌우 반전)
        if (player2PreviewImage != null)
        {
            player2PreviewImage.sprite = p2Sprite;
            player2PreviewImage.enabled = (p2Sprite != null);

            // ★ 2P 좌우 반전
            player2PreviewImage.rectTransform.localScale = new Vector3(-1f, 1f, 1f);
        }

        // 팀 이름 텍스트 세팅
        string p1TeamName = PlayerManager.Instance.player1Team;   // TeamSelect에서 넣어둔 문자열
        string p2TeamName = PlayerManager.Instance.player2Team;

        if (player1TeamNameText != null)
        {
            player1TeamNameText.text = string.IsNullOrEmpty(p1TeamName) ? "Player 1" : p1TeamName;
        }

        if (player2TeamNameText != null)
        {
            player2TeamNameText.text = string.IsNullOrEmpty(p2TeamName) ? "Player 2" : p2TeamName;
        }
    }

    private Sprite GetTeamSprite(int index)
    {
        if (teamSprites == null || teamSprites.Length == 0)
            return null;

        if (index < 0 || index >= teamSprites.Length)
            return null;

        return teamSprites[index];
    }

    // ============================
    //      동전 자동 던지기
    // ============================
    private IEnumerator Auto3DCoinToss()
    {
        yield return new WaitForSeconds(0.5f);

        player1IsLeft = Random.Range(0, 2) == 0;

        if (imageRect != null)
        {
            imageRect.anchoredPosition = startPosition;
            imageRect.sizeDelta = new Vector2(sizeStart, sizeStart);
        }

        float elapsedTime = 0f;

        while (elapsedTime < tossDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / tossDuration;

            if (imageRect != null)
            {
                Vector2 currentPos = Vector2.Lerp(startPosition, endPosition, t);
                imageRect.anchoredPosition = currentPos;

                float currentSize = Mathf.Lerp(sizeStart, sizeEnd, t);
                imageRect.sizeDelta = new Vector2(currentSize, currentSize);
            }

            if (coin3D != null)
            {
                float rotation = spinSpeed * elapsedTime;
                coin3D.transform.rotation = Quaternion.Euler(0, rotation, 0);
            }

            yield return null;
        }

        float finalRotation = player1IsLeft ? 0f : 180f;
        if (coin3D != null)
            coin3D.transform.rotation = Quaternion.Euler(0, finalRotation, 0);

        if (PlayerManager.Instance != null)
            PlayerManager.Instance.SetCoinTossResult(player1IsLeft);

        ShowResult();
    }

    private void ShowResult()
    {
        if (resultText != null)
            resultText.text = player1IsLeft ? "1P First" : "2P First";

        if (playButton != null)
            playButton.gameObject.SetActive(true);
    }

    private void OnPlayButtonClicked()
    {
        SceneManager.LoadScene("MainScene");
    }

    private void OnDestroy()
    {
        if (renderTexture != null)
            renderTexture.Release();
    }
}
